using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SoundInfo
{
    public AudioClip audioClip;
    public AudioProfile profile;

    [Range(0, 1)] public float volume = 1;
    [Range(-2, 2)] public float pitch = 1;
    public bool loop;
    public bool playOnAwake;
    public bool shareSource;

    [HideInInspector] public float timeScale = 1;

    public AudioSource source { get; private set; }
    SoundDirector director;

    public void Initialize(GameObject owner)
    {
        director = GameObject.FindObjectOfType<SoundDirector>();

        if (director == null) Debug.LogError("No Sound Director in scene!");

        if (shareSource)
        {
            source = owner.GetComponentInChildren<AudioSource>();

            if (source == null)
            {
                source = GameObject.Instantiate(director.GetAudioSource(profile), owner.transform);
            }
        }
        else
        {
            source = GameObject.Instantiate(director.GetAudioSource(profile), owner.transform);
        }

        source.name = "Audio Source " + audioClip.name;
        source.transform.localPosition = Vector3.zero;
        source.transform.localScale = Vector3.one;
        source.transform.localRotation = Quaternion.identity;


        if (playOnAwake)
        {
            Play();
        }

    }

    public void Stop(float fallOffTime)
    {
        director.StopSound(this);
    }

    public void Play()
    {
        timeScale = 1;
        director.PlaySound(this);
    }

    public void Pause()
    {
        timeScale = 0;

        director.PauseSound(this);
    }

    public void Resume()
    {
        timeScale = 1;

        director.ResumeSound(this);
    }

    public void Play(float duration)
    {
        timeScale = 1;
        director.PlaySound(this, duration, 0);
    }

    public void Play(float duration, float fallInTime, float fallOffTime, float delay)
    {
        timeScale = 1;
        director.PlaySound(this, duration, fallOffTime, fallInTime, delay);
    }

    public void PlayScheduled(float delay)
    {
        timeScale = 1;
        director.PlaySound(this, audioClip.length, delay);
    }
}