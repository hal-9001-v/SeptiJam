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
    public Action<Car> speedCallback;


    private void Awake()
    {
        interactable.enterTriggerCallback += CheckSpeed;
    }

    void CheckSpeed(Interactor interactor)
    {
        var car = interactor.GetComponent<Car>();

        if (car)
        {
            if (car.GetCurrentSpeed >= requiredSpeed && car.GetCurrentWeight >= requiredWeight)
            {

                speedEvent.Invoke();

                if (speedCallback != null)
                {
                    speedCallback.Invoke(car);
                }
            }
        }
    }
}
