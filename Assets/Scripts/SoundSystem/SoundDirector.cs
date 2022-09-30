using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDirector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] [Range(0, 1)] float defaulFallInTime = 0.05f;
    [SerializeField] [Range(0, 1)] float defaulFallOffTime = 0.1f;

    [Header("Profiles")]
    [SerializeField] AudioProfileHolder[] audioProfiles;

    Dictionary<SoundInfo, Coroutine> soundCoroutines;

    private void Awake()
    {
        soundCoroutines = new Dictionary<SoundInfo, Coroutine>();
    }

    [ContextMenu("Update Profiles")]
    void UpdateAudioProfiles()
    {
        var enumValues = Enum.GetValues(typeof(AudioProfile));

        audioProfiles = new AudioProfileHolder[enumValues.Length];

        for (int i = 0; i < audioProfiles.Length; i++)
        {
            audioProfiles[i].audioProfile = (AudioProfile)enumValues.GetValue(i);

        }

    }

    public AudioSource GetAudioSource(AudioProfile profile)
    {
        foreach (var audioProfile in audioProfiles)
        {
            if (audioProfile.audioProfile == profile)
                return audioProfile.audioSource;
        }

        Debug.LogWarning("No such profile!");
        return null;
    }

    public void PlaySound(SoundInfo soundInfo)
    {
        float duration = soundInfo.audioClip.length;

        PlaySound(soundInfo, duration, duration * defaulFallInTime, duration * defaulFallOffTime, 0);
    }

    public void StopSound(SoundInfo soundInfo)
    {
        if (soundCoroutines.TryGetValue(soundInfo, out var coroutine))
        {            
            StopCoroutine(coroutine);
        }
    }

    public void PlaySound(SoundInfo soundInfo, float duration, float delay)
    {

        PlaySound(soundInfo, duration, duration * defaulFallInTime, duration * defaulFallOffTime, delay);
    }

    public void PlaySound(SoundInfo soundInfo, float duration, float fallInTime, float fallOffTime, float delay)
    {
        soundInfo.source.clip = soundInfo.audioClip;
        soundInfo.source.volume = soundInfo.volume;
        soundInfo.source.pitch = soundInfo.pitch;
        soundInfo.source.loop = soundInfo.loop;

        StopSound(soundInfo);

        if ((fallInTime + fallOffTime) < duration)
        {
            soundCoroutines[soundInfo] = StartCoroutine(PlaySoundOvertime(soundInfo, duration, fallInTime, fallOffTime, delay));
        }
        else
        {
            Debug.LogWarning("FallIn and FallOff are greater than duration!");
        }
    }

    public void PauseSound(SoundInfo soundInfo)
    {
        soundInfo.source.Pause();
    }

    public void ResumeSound(SoundInfo soundInfo)
    {
        //PlaceHolder!
    }


    IEnumerator PlaySoundOvertime(SoundInfo soundInfo, float duration, float fallInTime, float fallOffTime, float delay)
    {
        yield return new WaitForSeconds(delay);
        soundInfo.source.Play();

        yield return StartCoroutine(Fall(soundInfo, true, fallInTime));

        if (soundInfo.loop == false)
        {
            yield return new WaitForSeconds(duration - fallInTime - fallOffTime);
            yield return StartCoroutine(Fall(soundInfo, false, fallOffTime));

            soundInfo.source.Stop();
        }
    }

    IEnumerator Fall(SoundInfo soundInfo, bool fallIn, float fallTime)
    {
        float startingValue = 0;
        float targetValue = 0;
        if (fallIn)
        {
            targetValue = soundInfo.source.volume;
        }
        else
        {
            startingValue = soundInfo.source.volume;
        }

        float elapsedTime = 0;

        while (elapsedTime < fallTime)
        {
            elapsedTime += Time.deltaTime * soundInfo.timeScale;
            soundInfo.source.volume = FloatLerp(startingValue, targetValue, transitionCurve.Evaluate(elapsedTime / fallTime));

            yield return null;
        }
    }

    static float FloatLerp(float a, float b, float t)
    {
        if (t > 1) t = 1;
        else if (t < 0) t = 0;

        return a * (1 - t) + b * t;
    }

    [Serializable]
    struct AudioProfileHolder
    {
        public AudioProfile audioProfile;

        public AudioSource audioSource;
    }

}
