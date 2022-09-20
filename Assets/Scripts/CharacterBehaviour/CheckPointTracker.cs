using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class CheckPointTracker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 respawnOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] CheckPoint currentCheckPoint;

    public Action<Vector3, Vector3> spawnCallback;
    public void SetCheckPoint(CheckPoint checkPoint)
    {
        currentCheckPoint = checkPoint;
    }

    [ContextMenu("Spawn")]
    public void Respawn()
    {
        var spawnPoint = currentCheckPoint.SpawnPoint + respawnOffset;

        //Disable CC if there is any to modify position
        var characterController = GetComponent<CharacterController>();
        if (characterController)
            characterController.enabled = false;
        
        transform.position = spawnPoint;
        transform.forward = currentCheckPoint.SpawnDirection;
        
        if (characterController)
            characterController.enabled = true;

        if (spawnCallback != null)
        {
            spawnCallback.Invoke(spawnPoint, currentCheckPoint.SpawnDirection);
        }
    }

}
