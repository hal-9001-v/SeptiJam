using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CarAttribute
{
    //Tocar esto para nivelar 
    private static Dictionary<CarVarsType, float> CAR_VAR_MULTIPLIERS = new Dictionary<CarVarsType, float>() {
        { CarVarsType.MotorForce, 750 },
        { CarVarsType.SteerAngle, 10 },
        { CarVarsType.Weight, 500 }
    };

    public CarVarsType ParameterType;
    public float BaseValue;
    private List<CarModifier> currentModifiers = new List<CarModifier>();

    [HideInInspector]
    public float CurrentValue => ApplyModifiers();

    public float ApplyModifiers()
    {
        float currentValue = BaseValue;
        for (int i = 0; i < currentModifiers.Count; i++)
        {
            currentValue += currentModifiers[i].ModificationValue * CAR_VAR_MULTIPLIERS[ParameterType];
        }
        return currentValue;
    }
    public void OnModifierAdded(CarModifier modifier)
    {
        if (modifier.ParameterType == ParameterType)
        {
            currentModifiers.Add(modifier);
        }
    }
    public void OnModifierRemoved(CarModifier modifier)
    {
        if (modifier.ParameterType == ParameterType)
        {
            currentModifiers.Remove(modifier);
        }
    }
    public void ClearModifiers()
    {
        currentModifiers.Clear();
    }
}