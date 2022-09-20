using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DistanceInteractable))]
public class CheckPoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 spawnPoint;
    [SerializeField] Vector3 spawnDirection = Vector3.forward;
    public Vector3 SpawnPoint { get { return transform.TransformPoint(spawnPoint); } }
    public Vector3 SpawnDirection { get { return transform.TransformDirection(spawnDirection.normalized); } }
    [SerializeField] Color color = Color.blue;

    DistanceInteractable interactable => GetComponent<DistanceInteractable>();

    private void Awake()
    {
        interactable.stayCallback += CheckInteractor;
    }

    void CheckInteractor(Interactor interactor)
    {
        var tracker = interactor.GetComponent<CheckPointTracker>();

        if (tracker)
        {
            tracker.SetCheckPoint(this);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        float size = 1f;
        Gizmos.DrawCube(SpawnPoint, Vector3.one * size);

        Gizmos.DrawLine(SpawnPoint, SpawnPoint + SpawnDirection * interactable.Range);
        Gizmos.DrawSphere(SpawnPoint + SpawnDirection * interactable.Range, size * 0.4f);
    }

}
