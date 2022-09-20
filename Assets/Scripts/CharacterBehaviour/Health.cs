using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] [Range(1, 10)] int maxHealth;
    public int currentHealth { get; private set; }

    public Action hurtCallback;
    public Action deadCallback;

    public bool canBeHurt = true;
    public bool isAlive { get { return currentHealth > 0; } }

    public void Hurt(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;

            if (deadCallback != null)
            {
                deadCallback.Invoke();
            }
        }
        else
        {
            if (hurtCallback != null)
            {
                hurtCallback.Invoke();
            }
        }

    }
}
