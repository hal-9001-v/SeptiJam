using System;
using System.Collections;
using System.Collections.Generic;
using static DataFileManager;
using UnityEngine;
using UnityEngine.Rendering;

public enum WheelSize
{
    SMALL = 1,
    MEDIUM = 2,
    BIG = 3
}


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CheckPointTracker))]
public class Car : MonoBehaviour
{
    //Input
    PlayerInput input;
    private Vector2 movementInput;
    private bool axisInUse;

    [HideInInspector] public const float DEFAULT_WHEEL_SIZE = 0.35f;
    public const float DEFAULT_WHEEL_SIZE_MODIFIER = 0.1f;
    public const float DEFAULT_SQUARE_WHEEL_CV_MODIFIER = 1.65f;

    [Header("Car Modifiers")] public CarModifierInfo carModifierInfo;

    [Header("Spawn")] [SerializeField] Vector3 spawnOffset;
    [SerializeField] CheckPointTracker tracker;

    protected Rigidbody Rigidbody;

    [Header("Wheels")] public WheelInfo[] Wheels;

    public float GetCurrentSpeed
    {
        get { return Rigidbody.velocity.magnitude; }
    }

    public float GetCurrentWeight
    {
        get { return Rigidbody.mass; }
    }

    public float GetCurrentTurbo
    {
        get { return turboAmount; }
    }

    [Header("Car Physics Variables")]
    [SerializeField]
    Vector3 CenterOfMass;

    [SerializeField] float MotorPower = 5000f;
    [SerializeField] float SteerAngle = 35f;
    [SerializeField] float turboAmount;
    [Range(0, 1)] public float KeepGrip = 1f;
    [Range(0, 7)] public float Grip = 5f;


    [HideInInspector] public InputStr Input;

    bool followingCurve;
    CurveFollower curveFollower;
    float workshopSpeed = 5;
    public LayerMask groundLayer;
    public float yOffset;
    public float raycastRange;

    public struct InputStr
    {
        public float Forward;
        public float Steer;
    }

    void Awake()
    {
        StartInput();
        tracker.spawnCallback += Spawn;
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.centerOfMass = CenterOfMass;

        // SetCarModifierInfoDefaultStats();
        OnChangeCarPiece();
        OnValidate();
    }

    private void StartInput()
    {
        input = new PlayerInput();
        input.Car.MovementAxis.performed += (axis) =>
        {
            movementInput = axis.ReadValue<Vector2>();
            Input.Steer = movementInput.x;
            Input.Forward = movementInput.y;
            axisInUse = true;
        };

        input.Car.Turbo.performed += (ctx) =>
        {
            //turbo
        };
        input.Car.MovementAxis.canceled += (axis) =>
        {
            movementInput = Vector2.zero;
            axisInUse = false;
        };

        input.Enable();
    }

    void FixedUpdate()
    {
        if (followingCurve)
        {
            MoveInCurve();
        }
        else
        {

            for (int i = 0; i < Wheels.Length; i++)
            {
                if (Wheels[i].Motor)
                    Wheels[i].WheelCollider.motorTorque = Input.Forward * MotorPower;
                if (Wheels[i].Steer)
                    Wheels[i].WheelCollider.steerAngle = Input.Steer * SteerAngle;

                Wheels[i].Rotation += Wheels[i].WheelCollider.rpm / 60 * 360 * Time.fixedDeltaTime;
                Wheels[i].MeshRenderer.localRotation = Wheels[i].MeshRenderer.parent.localRotation *
                                                       Quaternion.Euler(Wheels[i].Rotation,
                                                           Wheels[i].WheelCollider.steerAngle, 0);
            }

            Rigidbody.AddForceAtPosition(transform.up * (Rigidbody.velocity.magnitude * -0.1f * Grip),
                transform.position + transform.rotation * CenterOfMass);
        }

    }
    void MoveInCurve()
    {

        if (curveFollower.t >= 1) return;
        float y = transform.position.y;
        var curvePos = curveFollower.UpdateTimeWithDistance(workshopSpeed * Time.fixedDeltaTime);
        var up = Vector3.up;
        var forward = curveFollower.direction;
        forward.y = 0;
        forward.Normalize();

        if (Physics.Raycast(curvePos, Vector3.down, out var raycastHit, raycastRange, groundLayer))
        {
            curvePos = raycastHit.point + raycastHit.normal * yOffset;
            up = raycastHit.normal;
        }

        transform.position = curvePos;
        transform.rotation = Quaternion.LookRotation(forward, up);

        Rigidbody.AddForce(curveFollower.direction * workshopSpeed, ForceMode.VelocityChange);
    }

    void Spawn(Vector3 position, Vector3 direction)
    {
        transform.position = position + spawnOffset;
        transform.forward = direction;

        Rigidbody.velocity = Vector3.zero;
    }

    public void Hurt()
    {
    }

    public float EnterCarshop(CurveFollower follower)
    {
        return FollowCurve(follower);
    }

    public float FollowCurve(CurveFollower follower)
    {
        Rigidbody.isKinematic = true;
        curveFollower = follower;
        followingCurve = true;

        return follower.length / workshopSpeed;
    }

    public void StopCurve()
    {
        Rigidbody.isKinematic = false;
        followingCurve = false;
    }

    public void LeaveCarshop()
    {
        StopCurve();

        transform.parent = null;
    }
    public CarData GetCarData()
    {
        CarData carData = new CarData();

        return carData;
    }

    public void SetCarData(CarData carData)
    {
        //Game has been loaded!
    }


    //TEMP QUICK AND DIRTY
    void SetCarModifierInfoDefaultStats()
    {
        carModifierInfo.carMass = 2800f;
        carModifierInfo.motorForce = 5000f;
        carModifierInfo.steerAngle = 35f;
        carModifierInfo.centerOfMass = new Vector3(0f, -2f, 0.159f);

        carModifierInfo.springForce = 35000f;
        carModifierInfo.damp = 4500f;
        carModifierInfo.springLength = 1f;
        carModifierInfo.wheelSize = WheelSize.MEDIUM;
        carModifierInfo.squareWheels = true;
    }

    //Call this when modifying any car attribs
    void OnChangeCarPiece()
    {
        //Car stats
        Rigidbody.mass = carModifierInfo.carMass;
        SteerAngle = carModifierInfo.steerAngle;
        CenterOfMass = carModifierInfo.centerOfMass;

        if (carModifierInfo.squareWheels)
            MotorPower = carModifierInfo.motorForce * DEFAULT_SQUARE_WHEEL_CV_MODIFIER;
        else
            MotorPower = carModifierInfo.motorForce;

        //wheel stats
        float wheelRadius;
        switch (carModifierInfo.wheelSize)
        {
            case WheelSize.SMALL:
                wheelRadius = DEFAULT_WHEEL_SIZE - DEFAULT_WHEEL_SIZE_MODIFIER;
                break;
            case WheelSize.BIG:
                wheelRadius = DEFAULT_WHEEL_SIZE + DEFAULT_WHEEL_SIZE_MODIFIER;
                break;
            case WheelSize.MEDIUM:
            default:
                wheelRadius = DEFAULT_WHEEL_SIZE;
                break;
        }

        ModifyWheelStats(carModifierInfo.springForce, carModifierInfo.damp, carModifierInfo.springLength, wheelRadius,
            carModifierInfo.squareWheels);
    }

    void OnValidate()
    {
        Debug.Log("Validate");
        for (int i = 0; i < Wheels.Length; i++)
        {
            //settings
            var ffriction = Wheels[i].WheelCollider.forwardFriction;
            var sfriction = Wheels[i].WheelCollider.sidewaysFriction;
            ffriction.asymptoteValue =
                Wheels[i].WheelCollider.forwardFriction.extremumValue * KeepGrip * 0.998f + 0.002f;
            sfriction.extremumValue = 1f;
            ffriction.extremumSlip = 1f;
            ffriction.asymptoteSlip = 2f;
            ffriction.stiffness = Grip;
            sfriction.extremumValue = 1f;
            sfriction.asymptoteValue =
                Wheels[i].WheelCollider.sidewaysFriction.extremumValue * KeepGrip * 0.998f + 0.002f;
            sfriction.extremumSlip = 0.5f;
            sfriction.asymptoteSlip = 1f;
            sfriction.stiffness = Grip;
            Wheels[i].WheelCollider.forwardFriction = ffriction;
            Wheels[i].WheelCollider.sidewaysFriction = sfriction;
        }
    }

    private void ModifyWheelStats(float spring, float damp, float springLength, float wheelRadius, bool squareWheels)
    {
        for (int i = 0; i < Wheels.Length; i++)
        {
            var wheelColliderSuspensionSpring = Wheels[i].WheelCollider.suspensionSpring;

            wheelColliderSuspensionSpring.spring = spring;
            wheelColliderSuspensionSpring.damper = damp;
            wheelColliderSuspensionSpring.targetPosition = Wheels[i].WheelCollider.suspensionSpring.targetPosition;

            Wheels[i].WheelCollider.suspensionSpring = wheelColliderSuspensionSpring;
            Wheels[i].WheelCollider.radius = wheelRadius;
            Wheels[i].WheelCollider.suspensionDistance = springLength;

            Wheels[i].WheelCollider.GetComponentInChildren<BoxCollider>().enabled = squareWheels;

            if (squareWheels)
            {
                float wheelSizeModifier = 1 - DEFAULT_WHEEL_SIZE + wheelRadius;
                Debug.Log(wheelSizeModifier + " " + wheelRadius);
                Wheels[i].WheelCollider.GetComponentInChildren<BoxCollider>().size *= wheelSizeModifier;
            }
        }
    }




    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + CenterOfMass, .1f);
        Gizmos.DrawWireSphere(transform.position + CenterOfMass, .11f);
        Gizmos.DrawLine(transform.position, transform.position + yOffset * Vector3.down);
    }

    [System.Serializable]
    public struct WheelInfo
    {
        public WheelCollider WheelCollider;
        public Transform MeshRenderer;
        public bool Steer;
        public bool Motor;
        [HideInInspector] public float Rotation;
    }

    [System.Serializable]
    public struct CarModifierInfo
    {
        [Header("Car Stats")] public float carMass; //base mass 2.800
        public float motorForce; // base force 5000
        public float steerAngle; // base angle 35
        public Vector3 centerOfMass; // base COM 0, -2, 0.159  WARNING: Tofu car TODO: CHANGE

        [Header("Wheel Stats")] public float springForce; //base force 35.000
        public float damp; //base damp 4.500
        public float springLength; //base length 1
        public WheelSize wheelSize;
        public bool squareWheels;
    }


    [ContextMenu("UpdateCar")]
    public void UdpateCarDebug()
    {
        OnChangeCarPiece();
    }
}