using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour
{
    
    [Header("Spawn")]
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] CheckPointTracker tracker;

    protected Rigidbody Rigidbody;
    
    [Header("Wheels")]
    public WheelInfo[] Wheels;
    
    
    [Header("Car Physics Variables")]
    //TODO: change these to be modified through code and add getters & stuff
    [SerializeField] Vector3 CenterOfMass;
    [SerializeField] float MotorPower = 5000f;
    [SerializeField] float SteerAngle = 35f;

    [Range(0, 1)]
    public float KeepGrip = 1f;
    public float Grip = 5f;

    [HideInInspector]
    public CarModifierInfo carModifierInfo;

    [HideInInspector]
    public InputStr Input;
    public struct InputStr
    {
        public float Forward;
        public float Steer;
    }
        
    void Awake () {
        tracker.spawnCallback += Spawn;
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.centerOfMass = CenterOfMass;
        SetCarModifierInfoDefaultStats();
        OnValidate();
    }
    

    void Update()
    {
        for(int i = 0; i < Wheels.Length; i++)
        {
            if (Wheels[i].Motor)
                Wheels[i].WheelCollider.motorTorque = Input.Forward * MotorPower;
            if (Wheels[i].Steer)
                Wheels[i].WheelCollider.steerAngle = Input.Steer * SteerAngle;

            Wheels[i].Rotation += Wheels[i].WheelCollider   .rpm / 60 * 360 * Time.fixedDeltaTime;
            Wheels[i].MeshRenderer.localRotation = Wheels[i].MeshRenderer.parent.localRotation * Quaternion.Euler(Wheels[i].Rotation, -Wheels[i].WheelCollider.steerAngle, 0);

        }

        Rigidbody.AddForceAtPosition(transform.up * (Rigidbody.velocity.magnitude * -0.1f * Grip), transform.position + transform.rotation * CenterOfMass);
    }
    
    void Spawn(Vector3 position, Vector3 direction)
    {
        transform.position = position + spawnOffset;
        transform.forward = direction;
    }

    
    //TEMP QUICK AND DIRTY
    void SetCarModifierInfoDefaultStats()
    {
        carModifierInfo.springForce = 35000f;
        carModifierInfo.damp = 4500f;
        carModifierInfo.springLength = 1f;
        carModifierInfo.wheelRadius = 0.3f;
        
        carModifierInfo.motorForce = 5000f;
        carModifierInfo.steerAngle = 35f;
        
        carModifierInfo.carMass = 2800f;
        carModifierInfo.centerOfMass = new Vector3(0f, -2f, 0.159f);
        
        OnChangeCarPiece();
    }
    
    //Call this when modifying any car attribs
    void OnChangeCarPiece()
    {
        Rigidbody.mass = carModifierInfo.carMass;
        MotorPower = carModifierInfo.motorForce;
        SteerAngle = carModifierInfo.steerAngle;
        CenterOfMass = carModifierInfo.centerOfMass;
        ModifyWheelStats(carModifierInfo.springForce, carModifierInfo.damp, carModifierInfo.springLength, carModifierInfo.wheelRadius);
    }

    void OnValidate()
    {
        Debug.Log("Validate");
        for (int i = 0; i < Wheels.Length; i++)
        {
            //settings
            var ffriction = Wheels[i].WheelCollider.forwardFriction;
            var sfriction = Wheels[i].WheelCollider.sidewaysFriction;
            ffriction.asymptoteValue = Wheels[i].WheelCollider.forwardFriction.extremumValue * KeepGrip * 0.998f + 0.002f;
            sfriction.extremumValue = 1f;
            ffriction.extremumSlip = 1f;
            ffriction.asymptoteSlip = 2f;
            ffriction.stiffness = Grip;
            sfriction.extremumValue = 1f;
            sfriction.asymptoteValue = Wheels[i].WheelCollider.sidewaysFriction.extremumValue * KeepGrip * 0.998f + 0.002f;
            sfriction.extremumSlip = 0.5f;
            sfriction.asymptoteSlip = 1f;
            sfriction.stiffness = Grip;
            Wheels[i].WheelCollider.forwardFriction = ffriction;
            Wheels[i].WheelCollider.sidewaysFriction = sfriction;
        }
        
    }

    private void ModifyWheelStats(float spring, float damp, float springLength, float wheelRadius)    
    {
        for(int i = 0; i < Wheels.Length; i++)
        {
            var wheelColliderSuspensionSpring = Wheels[i].WheelCollider.suspensionSpring;
            
            wheelColliderSuspensionSpring.spring = spring;
            wheelColliderSuspensionSpring.damper = damp;
            wheelColliderSuspensionSpring.targetPosition = Wheels[i].WheelCollider.suspensionSpring.targetPosition;
            
            Wheels[i].WheelCollider.suspensionSpring = wheelColliderSuspensionSpring;
            Wheels[i].WheelCollider.radius = wheelRadius;
            Wheels[i].WheelCollider.suspensionDistance = springLength;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + CenterOfMass, .1f);
        Gizmos.DrawWireSphere(transform.position + CenterOfMass, .11f);
    }

    [System.Serializable]
    public struct WheelInfo
    {
        public WheelCollider WheelCollider;
        public Transform MeshRenderer;
        public bool Steer;
        public bool Motor;
        [HideInInspector]
        public float Rotation;
    }

    public struct CarModifierInfo
    {
        public float carMass; //base mass 2.800
        public float springForce; //base force 35.000
        public float damp; //base damp 4.500
        public float springLength; //base length 1
        public float wheelRadius; // base radius 0.3
        public float motorForce; // base force 5000
        public float steerAngle; // base angle 35
        public Vector3 centerOfMass; // base COM 0, -2, 0.159  WARNING: Tofu car TODO: CHANGE

    }

}
