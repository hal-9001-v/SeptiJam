using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputComponent))]
public class Radio : MonoBehaviour
{
    [SerializeField] List<RadioClip> radioClips;

    [Header("Sounds")]
    [SerializeField] SoundInfo songSound;
    [SerializeField] SoundInfo cassetteStart;
    [SerializeField] SoundInfo cassetteEnd;

    [SerializeField] SoundInfo ambientSound;

    int currentIndex = 0;

    float originalAmbientVolume;
    AudioListener listener => FindObjectOfType<AudioListener>();
    InputComponent inputComponent => GetComponent<InputComponent>();

    void Start()
    {
        SetInput(inputComponent.Input);


        songSound.Initialize(gameObject);

        cassetteStart.Initialize(gameObject);
        cassetteEnd.Initialize(gameObject);

        ambientSound.Initialize(gameObject);
        originalAmbientVolume = ambientSound.source.volume;

        StartPlaying();
    }


    private void Update()
    {
        var curve = songSound.source.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
        var curveValue = curve.Evaluate(Vector3.Distance(listener.transform.position, transform.position) / songSound.source.maxDistance);

        ambientSound.source.volume = (1 - curveValue) * originalAmbientVolume;
    }

    public void SetInput(PlayerInput input)
    {
        input.Car.NextSong.performed += (ctx) =>
        {
            NextClip();
        };


        input.Car.PreviousSong.performed += (ctx) =>
        {
            PreviousClip();
        };
    }

    public void NextClip()
    {
        currentIndex++;

        if (currentIndex >= radioClips.Count)
        {
            currentIndex = 0;
        }
        PlayClip(radioClips[currentIndex]);
    }

    public void PreviousClip()
    {
        currentIndex--;

        if (currentIndex <= -1)
        {
            currentIndex = radioClips.Count - 1;
        }
        PlayClip(radioClips[currentIndex]);
    }

    public void AddClip(RadioClip clip)
    {
        radioClips.Add(clip);
    }

    [ContextMenu("Play Radio")]
    public void StartPlaying()
    {
        songSound.Play();
    }

    [ContextMenu("Stop Radio")]
    public void StopPlaying()
    {
        songSound.Stop(0);
    }

    public void PlayClip(RadioClip clip)
    {
        songSound.audioClip = clip.clip;
        songSound.volume = clip.volume;

        cassetteEnd.Play(cassetteEnd.audioClip.length, 0.1f, 0.2f, 0);
        cassetteStart.Play(cassetteStart.audioClip.length, 0.1f, 0.2f, 0.3f);
        songSound.Play(clip.clip.length, 0.2f, 0.2f, cassetteStart.audioClip.length + 0.3f);

    }

}
