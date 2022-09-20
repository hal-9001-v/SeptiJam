using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CollisionInteractable))]
public class SpeedChecker : MonoBehaviour
{
    CollisionInteractable interactable => GetComponentInChildren<CollisionInteractable>();

    [Header("Settings")]
    [SerializeField] [Range(0, 200)] float requiredSpeed;
    [SerializeField] [Range(0, 200)] float requiredWeight;

    public UnityEvent speedEvent;
    public Action speedCallback;


    private void Awake()
    {
        interactable.enterTriggerCallback += (interactor) =>
        {
            var insertComponentWithSpeedHere = interactor.GetComponent<Transform>();

            if (insertComponentWithSpeedHere)
            {
                CheckSpeed(insertComponentWithSpeedHere);
            }
        };
    }

    void CheckSpeed(Transform componentWithSpeedHere)
    {
        //If(componentWithSpeed.speed >= requiredSpeed && componentWithSpeed.weight >= requiredWeight)
        if (false)
        {
            speedEvent.Invoke();

            if (speedCallback != null)
            {
                speedCallback.Invoke();
            }
        }
    }
}
