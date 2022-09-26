using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CollisionInteractable))]
public class SpeedChecker : MonoBehaviour
{
    CollisionInteractable interactable => GetComponentInChildren<CollisionInteractable>();
    Rigidbody[] rigidbodies => GetComponentsInChildren<Rigidbody>();

    [Header("Settings")]
    [SerializeField] [Range(1, 5)] float requiredWeight;

    public UnityEvent speedEvent;
    public Action<Car> speedCallback;
    

    private void Awake()
    {
        interactable.enterTriggerCallback += CheckSpeed;

        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = true;
        }
    }

    void CheckSpeed(Interactor interactor)
    {
        var car = interactor.GetComponent<Car>();

        if (car)
        {
            if (car.GetMassStars() >= requiredWeight)
            {
        
                speedEvent.Invoke();
                foreach (Rigidbody r in rigidbodies)
                {
                    r.isKinematic = false;
                }
                if (speedCallback != null)
                {
                    speedCallback.Invoke(car);
                }
            }
        }
    }
}
