using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] Transform pointC;
    public CurveFollower curveFollower;



    public Vector3 a { get { return pointA.position; } set { pointA.position = value; } }
    public Vector3 b { get { return pointB.position; } set { pointB.position = value; } }
    public Vector3 c { get { return pointC.position; } set { pointC.position = value; } }
    public CurveFollower follower { get { return curveFollower; } }
    public Vector3 GetCurvePosition(float t)
    {
        if (t > 1) t = 1;
        if (t < 0) t = 0;
        return (1 - t) * (1 - t) * pointA.position + 2 * (1 - t) * t * pointB.position + t * t * pointC.position;
    }

    public float GetLength()
    {
        float length = 0;
        for (int i = 0; i < 10; i++)
        {
            var positionA = GetCurvePosition(0.1f * i);
            var positionB = GetCurvePosition(0.1f * (i + 1));

            length += Vector3.Distance(positionA, positionB);
        }

        return length;
    }

    [ContextMenu("Create Points")]
    void CreatePoints()
    {
        pointA = new GameObject().transform;
        pointA.name = "Point A";
        pointA.parent = transform;
        pointA.position = transform.position;

        pointB = new GameObject().transform;
        pointB.name = "Point B";
        pointB.parent = transform;
        pointB.position = transform.position;

        pointC = new GameObject().transform;
        pointC.name = "Point C";
        pointC.parent = transform;
        pointC.position = transform.position;
    }

    public void SetBInMiddleAlongDirection(Vector3 direction, float minAngle)
    {
        var ac = (pointC.position - pointA.position) * 0.5f;
        var angle = Vector3.Angle(ac, direction);

        //If Angle is close to 90, Cos() would result in 0. In Else, lenght is calculated through ac.magitude/cos() and it would get too far away 
        if (Mathf.Abs(90 - angle) <= minAngle)
        {
            pointB.position = pointA.position + direction * ac.magnitude;
        }
        else
        {
            var length = Mathf.Abs(ac.magnitude / Mathf.Cos(angle * Mathf.Deg2Rad));
            pointB.position = pointA.position + direction.normalized * length;
        }
    }

    public void SetBInMiddleAlongDirection(Vector3 direction)
    {
        SetBInMiddleAlongDirection(direction, 15f);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pointA.position, 0.2f);
        Gizmos.DrawSphere(pointB.position, 0.2f);
        Gizmos.DrawSphere(pointC.position, 0.2f);

        Gizmos.DrawLine(pointA.position, pointC.position);
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawLine(pointB.position, pointC.position);

        Gizmos.color = Color.green;
        for (int i = 0; i < 10; i++)
        {
            var positionA = GetCurvePosition(0.1f * i);
            var positionB = GetCurvePosition(0.1f * (i + 1));

            Gizmos.DrawLine(positionA, positionB);
        }
    }

}
