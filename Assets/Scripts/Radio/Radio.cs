using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{

    [SerializeField] List<RadioClip> radioClips;
    [SerializeField] AudioSource audioSource;

    int currentIndex = 0;

    private void Awake()
    {
        //radioClips = new List<RadioClip>();

        PlayerInput input = new PlayerInput();
        SetInput(input);
        input.Enable();

        StartPlaying();
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
        audioSource.clip = radioClips[currentIndex].clip;
        audioSource.Play();
    }

    [ContextMenu("Stop Radio")]
    public void StopPlaying()
    {
        audioSource.Stop();

    }

    public void PlayClip(RadioClip clip)
    {
        audioSource.clip = clip.clip;
        audioSource.volume = clip.volume;
        audioSource.Play();
    }



}
