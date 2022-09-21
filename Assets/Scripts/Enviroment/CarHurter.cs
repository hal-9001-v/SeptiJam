using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpeedChecker))]
public class CarHurter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [Range(0, 1)] float chance = 0.5f;

    SpeedChecker speedChecker => GetComponent<SpeedChecker>();



    // Start is called before the first frame update
    void Start()
    {
        speedChecker.speedCallback += Hit;
    }

    void Hit(Car car)
    {
        var random = Random.Range(0, 1);

        if (random < chance)
        {
            car.Hurt();
        }
    }

}
