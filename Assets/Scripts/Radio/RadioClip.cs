using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "RadioClip", menuName = "RadioClip", order = 1)]
public class RadioClip :ScriptableObject
{
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1;
}
