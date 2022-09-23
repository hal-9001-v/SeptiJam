using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CurvePath curvePath;

    [Range(0, 1)] public float t;

    [SerializeField] AnimationCurve curve;
    public float length
    {
        get
        {
            if (curvePath == null) return 0;

            return curvePath.GetLength();
        }
    }

    public Vector3 direction
    {
        get
        {
            if (t == 1)
            {
                return (curvePath.GetCurvePosition(1) - curvePath.GetCurvePosition(0.99f)).normalized;

            }

            return (curvePath.GetCurvePosition(t + 0.01f) - curvePath.GetCurvePosition(t)).normalized;
        }
    }

    public Vector3 SetTime(float t)
    {
        if (t < 0) t = 0;
        if (t > 1) t = 1;

        this.t = t;
        transform.position = curvePath.GetCurvePosition(curve.Evaluate(t));

        return transform.position;
    }

    public Vector3 DeltaTime(float delta)
    {
        return SetTime(t + delta);
    }

    public Vector3 UpdateTimeWithDistance(float distance)
    {
        var change = distance / length;

        return SetTime(t + change);
    }

}
