using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarVarsType 
{
   Weight,
   SteerAngle,
   MotorForce,
   Turbo
}

[Serializable]
public class CarModifier
{
    public CarVarsType ParameterType;
    [Range(1,5)]
    public int ModificationValue;
}
