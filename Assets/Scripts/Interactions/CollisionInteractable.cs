using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CollisionInteractable : MonoBehaviour
{
    [Header("Settings")]
    public bool ready = true;

    public bool onlyOnce;
    [HideInInspector]public bool done;


    [SerializeField] UnityEvent enterTrigger;
    [SerializeField] UnityEvent stayTrigger;
    [SerializeField] UnityEvent exitTrigger;

    public Action<Interactor> enterTriggerCallback;
    public Action<Interactor> stayTriggerCallback;
    public Action<Interactor> exitTriggerCallback;

    void OnTriggerEnter(Collider other)
    {
        if (onlyOnce && done) return;

        var interactor = other.GetComponent<Interactor>();
        if (interactor)
        {
            done = true;
            enterTrigger.Invoke();

            if (enterTriggerCallback != null)
            {
                enterTriggerCallback.Invoke(interactor);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (onlyOnce && done) return;

        var interactor = other.GetComponent<Interactor>();
        if (interactor)
        {
            done = true;
            stayTrigger.Invoke();

            if (stayTriggerCallback != null)
            {
                stayTriggerCallback.Invoke(interactor);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (onlyOnce && done) return;

        var interactor = other.GetComponent<Interactor>();
        if (interactor)
        {
            done = true;
            exitTrigger.Invoke();

            if (exitTriggerCallback != null)
            {
                exitTriggerCallback.Invoke(interactor);
            }
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (onlyOnce && done) return;

        var interactor = collision.transform.GetComponent<Interactor>();
        if (interactor)
        {
            done = true;
            enterTrigger.Invoke();

            if (enterTriggerCallback != null)
            {
                enterTriggerCallback.Invoke(interactor);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (onlyOnce && done) return;

        var interactor = collision.transform.GetComponent<Interactor>();
        if (interactor)
        {
            done = true;
            stayTrigger.Invoke();

            if (stayTriggerCallback != null)
            {
                stayTriggerCallback.Invoke(interactor);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (onlyOnce && done) return;

        var interactor = collision.transform.GetComponent<Interactor>();
        if (interactor)
        {
            done = true;
            exitTrigger.Invoke();

            if (exitTriggerCallback != null)
            {
                exitTriggerCallback.Invoke(interactor);
            }
        }
    }


}
