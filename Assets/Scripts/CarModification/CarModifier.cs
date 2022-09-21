using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarVarsType 
{
   Weight,
   SteerAngle,
   MotorForce,
}

[Serializable]
public class CarModifier
{
    public CarVarsType ParameterType;
    //public bool IsPositiveValue;
    [Range(1,5)]
    public int ModificationValue;
}