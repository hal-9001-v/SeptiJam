using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] CheckPointTracker tracker;

    private void Awake()
    {
        tracker.spawnCallback += Spawn;
    }

    void Spawn(Vector3 position, Vector3 direction)
    {
        transform.position = position + spawnOffset;
        transform.forward = direction;
    }

}
