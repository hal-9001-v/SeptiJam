using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CounterHelper
{
    public float currentTime { private set; get; }
    public float targetTime;


    public bool Update(float deltaTime)
    {
        currentTime += deltaTime;

        if (targetTime <= currentTime)
        {
            return true;
        }

        return false;
    }

    public void Reset()
    {
        currentTime = 0;
    }

}
