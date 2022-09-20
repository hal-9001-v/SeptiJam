using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DistanceInteractable : MonoBehaviour
{
    [Header("Settings")]
    public bool ready = true;

    [SerializeField] [Range(1, 20)] float range = 5;

    public float Range { get { return range; } }

    public bool onlyOnce;
    [HideInInspector] public bool done;

    [SerializeField] Color color = Color.yellow;

    [SerializeField] UnityEvent stayEvent;
    public Action<Interactor> stayCallback;

    Interactor[] interactors;

    public int interactorCount { get; private set; }

    private void Awake()
    {
        UpdateInteractors();
    }

    private void Update()
    {
        CheckInteractorsInRange();
    }

    void CheckInteractorsInRange()
    {
        if (onlyOnce && done) return;

        interactorCount = 0;
        foreach (var interactor in interactors)
        {
            if (Vector3.Distance(transform.position, interactor.transform.position) <= range)
            {
                stayEvent.Invoke();

                if (stayCallback != null)
                {
                    stayCallback.Invoke(interactor);
                }

                done = true;

                interactorCount++;

                if (onlyOnce)
                    break;
            }
        }
    }

    public void UpdateInteractors()
    {
        interactors = FindObjectsOfType<Interactor>();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, range);

    }

}
