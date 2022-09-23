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

    [SerializeField] UnityEvent enterEvent;
    [SerializeField] UnityEvent stayEvent;
    [SerializeField] UnityEvent exitEvent;

    public Action<Interactor> enterCallback;
    public Action<Interactor> stayCallback;
    public Action<Interactor> exitCallback;

    List<Interactor> insideInteractors;

    Interactor[] interactors;

    public int interactorCount { get; private set; }

    private void Awake()
    {
        insideInteractors = new List<Interactor>();
        UpdateInteractors();
    }

    private void Update()
    {
        CheckEnterInteractors();
        CheckInsideInteractors();
    }

    void CheckEnterInteractors()
    {
        if (onlyOnce && done) return;


        foreach (var interactor in interactors)
        {
            if (insideInteractors.Contains(interactor)) continue;

            if (Vector3.Distance(transform.position, interactor.transform.position) <= range)
            {
                insideInteractors.Add(interactor);
                enterEvent.Invoke();


                if (enterCallback != null)
                {
                    enterCallback.Invoke(interactor);
                }

                done = true;
                if (onlyOnce)
                    break;
            }
        }
    }

    void CheckInsideInteractors()
    {
        for (int i = 0; i < insideInteractors.Count; i++)
        {
            if (Vector3.Distance(insideInteractors[i].transform.position, transform.position) > range)
            {
                exitEvent.Invoke();

                if (exitCallback != null)
                {
                    exitCallback.Invoke(insideInteractors[i]);
                }

                insideInteractors.RemoveAt(i);
            }
            else
            {
                stayEvent.Invoke();

                if (stayCallback != null)
                {
                    stayCallback.Invoke(insideInteractors[i]);
                }
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
