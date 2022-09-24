using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using static DataFileManager;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    private Vector2 movementInput;
    private bool axisInUse;
    private float maxSpeed = 100;
    private bool braking;
    private Vector2 currentMaxVel;


    //Turbo
    private bool activeTurbo;
    private bool usingTurbo;
    public const float DEFAULT_WHEEL_SIZE = 0.35f;
    public const float DEFAULT_WHEEL_SIZE_MODIFIER = 0.1f;
    public const float DEFAULT_SQUARE_WHEEL_CV_MODIFIER = 1.65f;

    public const float MOTORFORCE_TURBO_MODIFIER = 1.65f;
    public const float TURBO_FINISH_DELAY = 0.2f;
    public const float TURBO_CHARGE_SPEED = 0.005f;
    public const float TURBO_DEPLETE_SPEED = 0.01f;

    public const float MAX_MOTORFORCE = 5000f;
    public const float MIN_MOTORFORCE = 500f;
    public const float MAX_STEER = 40f;
    public const float MIN_STEER = 20f;
    public const float MAX_MASS = 5000f;
    public const float MIN_MASS = 1000f;
    public const float MAX_ACCELERATION = 5f;
    public const float MIN_ACCELERATION = 0.75f;
    public const float MAX_STAR_VAL = 4;

    public const float MIN_SPEED = 20f;
    

    [Header("Car Modifiers")] public CarModifierInfo carModifierInfo;

    [Header("Spawn")] [SerializeField] Vector3 spawnOffset;
    [SerializeField] CheckPointTracker tracker;

    protected Rigidbody Rigidbody;

    [Header("Wheels")] public WheelInfo[] Wheels;


    public float GetCurrentSpeed
    {
        get
        {
            if (Rigidbody.velocity.magnitude > 1)
                return Rigidbody.velocity.magnitude;
            else
                return 0;
        }
    }

    public Vector2 GetCurrentVelocity
    {
        get
        {
            var velocity = Rigidbody.velocity;
            return new Vector2(velocity.x, velocity.z);
        }
    }

    public float GetCurrentWeight => Rigidbody.mass;

    public float GetCurrentTurbo => currentTurbo;

    public float GetTurboLength => turboLength;

    [Header("Car Physics Variables")] [SerializeField]
    Vector3 CenterOfMass;

    [SerializeField] float MotorPower = 5000f;
    [SerializeField] float SteerAngle = 35f;
    [SerializeField] float turboLength;
    [Range(0, 1)] public float KeepGrip = 1f;
    [Range(0, 15)] public float Grip = 5f;


    [HideInInspector] private float currentTurbo;
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
        tracker.spawnCallback += Spawn;
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.centerOfMass = CenterOfMass;

        // SetCarModifierInfoDefaultStats();
        OnChangeCarPiece();
        OnValidate();
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
                Debug.Log("Fuersas" + GetCurrentSpeed + " " + maxSpeed + " " + Wheels[i].WheelCollider.motorTorque);
                if (Wheels[i].Motor)
                    if (GetCurrentSpeed < maxSpeed || usingTurbo)
                    {
                        Wheels[i].WheelCollider.motorTorque = Input.Forward * MotorPower;
                    }
                    else if (!braking)
                    {
                        Wheels[i].WheelCollider.motorTorque = 0;
                    }

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

    public void HandleTurbo()
    {
        activeTurbo = true;
        StartCoroutine(DecreaseTurboCharge());
        if (currentTurbo > 0)
        {
            usingTurbo = true;
            MotorPower = carModifierInfo.motorForce * MOTORFORCE_TURBO_MODIFIER;
        }
        else
        {
            usingTurbo = false;
            MotorPower = carModifierInfo.motorForce;
        }
    }

    public void StopTurbo()
    {
        activeTurbo = false;
        StartCoroutine(StartTurboCharge());
        MotorPower = carModifierInfo.motorForce;
    }

    IEnumerator DecreaseTurboCharge()
    {
        while (currentTurbo > 0 && activeTurbo)
        {
            yield return new WaitForSeconds(0.01f);
            currentTurbo -= TURBO_DEPLETE_SPEED;
        }

        usingTurbo = false;
        StartCoroutine(Brake());
        MotorPower = carModifierInfo.motorForce;
    }

    IEnumerator Brake()
    {
        for (int i = 0; i < Wheels.Length; i++)
        {
            Wheels[i].WheelCollider.motorTorque = -carModifierInfo.motorForce * MOTORFORCE_TURBO_MODIFIER;
        }

        braking = true;
        yield return new WaitForSeconds(TURBO_FINISH_DELAY);
        braking = false;
    }

    IEnumerator StartTurboCharge()
    {
        yield return new WaitForSeconds(TURBO_FINISH_DELAY);
        while (currentTurbo < turboLength && !activeTurbo)
        {
            yield return new WaitForSeconds(0.01f);
            currentTurbo += TURBO_CHARGE_SPEED;
        }
    }


    public int GetAccelerationStars()
    {
        float normalizedMotor = ProcessAndNormalize(carModifierInfo.motorForce, MAX_MOTORFORCE, MIN_MOTORFORCE, MAX_STAR_VAL);
        float normalizedMass = ProcessAndNormalize(carModifierInfo.carMass,MAX_MASS, MIN_MASS,MAX_STAR_VAL);
        
        float wheelModifier = carModifierInfo.wheelSize == WheelSize.BIG ? 0.75f :
            carModifierInfo.wheelSize == WheelSize.SMALL ? 1.25f : 1f;

        float acceleration = (normalizedMotor * normalizedMotor / normalizedMass) * wheelModifier;
        
        float normalizedAcc = ProcessAndNormalize(acceleration, MAX_ACCELERATION, MIN_ACCELERATION, MAX_STAR_VAL);
        return Mathf.RoundToInt(normalizedAcc);
    }

    public int GetSpeedStars()
    {
        return Mathf.RoundToInt(ProcessAndNormalize(carModifierInfo.motorForce, MAX_MOTORFORCE, MIN_MOTORFORCE, MAX_STAR_VAL));
    }

    public int GetMassStars()
    {
        return Mathf.RoundToInt(ProcessAndNormalize(carModifierInfo.carMass, MAX_MASS, MIN_MASS, MAX_STAR_VAL));
    }

    public int GetSteerStars()
    {
        return Mathf.RoundToInt(ProcessAndNormalize(carModifierInfo.steerAngle, MAX_STEER, MIN_STEER, MAX_STAR_VAL));
    }

    private float NormalizeVal(float val, float max, float min)
    {
        return (val - min) / (max - min);
    }

    private float ProcessAndNormalize(float val, float max, float min, float maxVal)
    {
        float normalized = NormalizeVal(val, max, min) * maxVal;
        return normalized > maxVal ? maxVal + 1 : normalized + 1;
    }

    public float GetMaxVelocity()
    {
        // + 100km h + 20 km/h of min velocity
        maxSpeed = (carModifierInfo.motorForce - MIN_MOTORFORCE) / (MAX_MOTORFORCE - MIN_MOTORFORCE) * 100 + MIN_SPEED;
        return maxSpeed;
    }


//Call this when modifying any car attribs
    void OnChangeCarPiece()
    {
        //Car stats
        Rigidbody.mass = carModifierInfo.carMass;
        SteerAngle = carModifierInfo.steerAngle;
        CenterOfMass = carModifierInfo.centerOfMass;
        turboLength = carModifierInfo.turboLength;
        currentTurbo = turboLength;
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
        public float turboLength;

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