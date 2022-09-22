using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    [SerializeField] Vector3 spawnOffset;

    public Action<Vector3> spawnCallback;
    public Action unspawnCallback;

    public void Spawn(Vector3 position)
    {
        if (spawnCallback != null)
        {
            spawnCallback.Invoke(position);
        }

        transform.position = position + spawnOffset;
    }

    public void Unspawn()
    {
        if (unspawnCallback != null)
        {
            unspawnCallback.Invoke();
        }
    }

}
