using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionInteractable))]
public class PickableItem : MonoBehaviour
{

    CollisionInteractable collisionInteractable => GetComponent<CollisionInteractable>();
    public Action<ItemCollector> collectedCallback;

    private void Awake()
    {
        collisionInteractable.enterTriggerCallback += (interactor) =>
        {
            var collector = interactor.GetComponent<ItemCollector>();

            if (collector)
            {
                if (collectedCallback != null)
                {
                    collectedCallback.Invoke(collector);
                }
            }
        };
    }

    public void StandardHide()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }

}
