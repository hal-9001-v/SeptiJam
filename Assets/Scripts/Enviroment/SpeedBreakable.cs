using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpeedChecker))]
public class SpeedBreakable : MonoBehaviour
{
    SpeedChecker speedChecker => GetComponent<SpeedChecker>();

    private void Awake()
    {
        speedChecker.speedCallback += CarHit;
    }

    void CarHit(Car car)
    {
        //Break!
       // gameObject.SetActive(false);


    }

}
