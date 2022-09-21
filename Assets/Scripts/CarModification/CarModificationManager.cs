using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarModificationManager : MonoBehaviour
{
    public Action<CarModifier> OnModifierAdded;
    public Action<CarModifier> OnModifierRemoved;

    [SerializeField]
    private List<CarAttribute> carAttributes;

    private List<CarModifier> carModifiers;

    private Dictionary<CarVarsType, CarAttribute> attributeDictionary;
    private Dictionary<CarAccessoryType, CarAccesory> accesoriesAssigned;

    //References
    [SerializeField]
    private Car myCar;

    private void Awake()
    {

        carModifiers = new List<CarModifier>();

        accesoriesAssigned = new Dictionary<CarAccessoryType, CarAccesory>();
        for (int i = 0; i < Enum.GetValues(typeof(CarAccessoryType)).Length; i++)
        {
            accesoriesAssigned.Add((CarAccessoryType)i, null);
        }

        attributeDictionary = new Dictionary<CarVarsType, CarAttribute>();
        for (int i = 0; i < carAttributes.Count; i++)
        {
            if (!attributeDictionary.TryAdd(carAttributes[i].ParameterType, carAttributes[i]))
            {
                Debug.LogError("Error duplicated attribute!! " + carAttributes[i].ParameterType.ToString());
                carAttributes.RemoveAt(i);
            }
            else
            {
                OnModifierAdded += carAttributes[i].OnModifierAdded;
                OnModifierRemoved += carAttributes[i].OnModifierRemoved;
            }
        }
     
    }
    private void Start()
    {
        UpdateCarInformation();
    }
    private void Update()
    {
        Debug.Log("hola " + accesoriesAssigned.Count);
    }
    public float GetAttributeValue(CarVarsType parameterType)
    {
        return attributeDictionary[parameterType].CurrentValue;
    }
    public bool OnAddAccesory(CarAccesory accessory, CarAccessoryType typeOfAccessory)
    {
        if (accesoriesAssigned[typeOfAccessory])
        {
            Debug.Log("Position occupied");
            return false;
        }
        accesoriesAssigned[typeOfAccessory] = accessory;

        CarModifier[] modifiers = accessory.ModifiersInPosition(typeOfAccessory);

        for (int i = 0; i < modifiers.Length; i++)
        {
            OnModifierAdded?.Invoke(modifiers[i]);
        }
        carModifiers.AddRange(modifiers);
        accesoriesAssigned[typeOfAccessory] = accessory;
        UpdateCarInformation();
        Debug.Log(accessory.AccesoryInformation.AccessoryName + " Added");
        return true;

    }
    public CarAccesory OnRemoveAccessory(CarAccessoryType typeOfAccessory)
    {
        if (!accesoriesAssigned[typeOfAccessory])
        {
            Debug.Log("No accessory to remove");
            return null;
        }

        CarModifier[] modifiers = accesoriesAssigned[typeOfAccessory].ModifiersInPosition(typeOfAccessory);

        for (int i = 0; i < modifiers.Length; i++)
        {
            OnModifierRemoved?.Invoke(modifiers[i]);
            carModifiers.Remove(modifiers[i]);
        }
        CarAccesory result = accesoriesAssigned[typeOfAccessory];
        Debug.Log(accesoriesAssigned[typeOfAccessory].AccesoryInformation.AccessoryName + " Removed");
        accesoriesAssigned[typeOfAccessory] = null;
        UpdateCarInformation();
        return result;
    }
    //Update the real information for the physics
    private void UpdateCarInformation()
    {
        myCar.carModifierInfo.carMass = attributeDictionary[CarVarsType.Weight].CurrentValue;
        myCar.carModifierInfo.motorForce = attributeDictionary[CarVarsType.MotorForce].CurrentValue;
        myCar.carModifierInfo.steerAngle = attributeDictionary[CarVarsType.SteerAngle].CurrentValue;
        myCar.UdpateCarDebug();
    }

}
