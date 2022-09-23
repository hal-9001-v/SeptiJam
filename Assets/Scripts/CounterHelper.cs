using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CounterHelper
{
    public float currentTime { private set; get; }
    public float normalizedTime { get { return currentTime / targetTime; } }
    public float targetTime;


    public bool Update(float deltaTime)
    {
        currentTime += deltaTime;

        if (targetTime <= currentTime)
        {
            currentTime = targetTime;
            return true;
        }

        if (currentTime < 0) currentTime = 0;
        return false;
    }

    public void Reset()
    {
        currentTime = 0;
    }

}
