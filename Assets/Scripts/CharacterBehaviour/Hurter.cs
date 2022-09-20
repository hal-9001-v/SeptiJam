using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hurter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [Range(1, 10)] int damage;

    public bool canHurt = true;
    public Action hurtDone;

    private void OnTriggerEnter(Collider other)
    {
        if (canHurt == false) return;

        var health = other.GetComponent<Health>();

        if (health && health.canBeHurt)
        {
            health.Hurt(damage);

            if (hurtDone != null)
            {
                hurtDone.Invoke();
            }
        }
    }
}
