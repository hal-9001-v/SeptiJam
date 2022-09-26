using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{
    [SerializeField] List<RadioClip> radioClips;
    [SerializeField] AudioSource radioSource;
    [SerializeField] AudioSource ambientSource;

    int currentIndex = 0;

    float originalAmbientVolume;
    AudioListener listener => FindObjectOfType<AudioListener>();

    private void Awake()
    {
        PlayerInput input = new PlayerInput();
        SetInput(input);
        input.Enable();

        originalAmbientVolume = ambientSource.volume;

        StartPlaying();
    }


    private void Update()
    {
        var curve = radioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
        var curveValue = curve.Evaluate(Vector3.Distance(listener.transform.position, transform.position) / radioSource.maxDistance);

        ambientSource.volume = (1 - curveValue) * originalAmbientVolume;
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
        radioSource.clip = radioClips[currentIndex].clip;
        radioSource.Play();
    }

    [ContextMenu("Stop Radio")]
    public void StopPlaying()
    {
        radioSource.Stop();

    }

    public void PlayClip(RadioClip clip)
    {
        radioSource.clip = clip.clip;
        radioSource.volume = clip.volume;
        radioSource.Play();
    }

}
