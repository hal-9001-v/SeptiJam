using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SoundProfile", menuName = "It Works Somehow/SoundProfile", order = 1)]
public class SoundProfile : ScriptableObject
{

    [SerializeField] AudioSource _audioSource;
    public AudioSource audioSource { get; private set; }
}
