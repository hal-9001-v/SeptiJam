using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionInteractable))]
public class KillArea : MonoBehaviour
{
    CollisionInteractable interactable => GetComponent<CollisionInteractable>();


    private void Awake()
    {
        interactable.enterTriggerCallback += CheckPlayer;
    }

    void CheckPlayer(Interactor interactor)
    {
        var tracker = interactor.GetComponent<CheckPointTracker>();

        if (tracker)
        {
            tracker.Respawn();
        }
    }
}
