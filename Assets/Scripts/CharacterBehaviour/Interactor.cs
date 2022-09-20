using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    PassiveInteractable[] passives;

    private void Awake()
    {
        UpdatePassiveInteractables();
    }

    public void InteractWithPassives(float range)
    {
        if (passives == null) return;

        foreach (var passive in passives)
        {
            if (Vector3.Distance(passive.transform.position, transform.position) < range)
            {
                passive.Interact(this);
            }
        }
    }

    public void UpdatePassiveInteractables()
    {
        passives = FindObjectsOfType<PassiveInteractable>();
    }

}
