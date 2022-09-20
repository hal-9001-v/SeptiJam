using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PassiveInteractable : MonoBehaviour
{
    [Header("Settings")]
    public bool ready = true;

    public bool onlyOnce;
    [HideInInspector] public bool done;


    [SerializeField] UnityEvent passiveEvent;
    public Action<Interactor> passiveCallback;

    public void Interact(Interactor interactor)
    {
        if (onlyOnce && done) return;

        passiveEvent.Invoke();

        if (passiveCallback != null)
        {
            passiveCallback.Invoke(interactor);
        }

        done = true;
    }

}
