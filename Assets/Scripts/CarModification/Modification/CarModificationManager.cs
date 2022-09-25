using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarModificationManager : MonoBehaviour
{

    public static CarModificationManager instance;
    public Action<CarModifier> OnModifierAdded;
    public Action<CarModifier> OnModifierRemoved;

    [SerializeField]
    private List<CarAttribute> carAttributes;

    private List<CarModifier> carModifiers;

    private Dictionary<CarVarsType, CarAttribute> attributeDictionary;
    private Dictionary<CarAccessoryType, CarAccessory> accesoriesAssigned;

    //References
    [SerializeField]
    private Car myCar;

    public static Action<CarAccessoryType, CarAccessory> OnCarModification;
    public static Action<Car> OnCarUpdate;
    public static Action<Car, CarAccessory, Car.CarModifierInfo, CarAccessoryType, CarModifier[]> OnAccesorySelectedAction;
    public static Action OnAccessoryDeselected;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        carModifiers = new List<CarModifier>();

        accesoriesAssigned = new Dictionary<CarAccessoryType, CarAccessory>();
        for (int i = 1; i < Enum.GetValues(typeof(CarAccessoryType)).Length; i++)
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
    public float GetAttributeValue(CarVarsType parameterType)
    {
        return attributeDictionary[parameterType].CurrentValue;
    }
    public bool OnAddAccesory(CarAccessory accessory, CarAccessoryType typeOfAccessory)
    {

        if (!accessory.CanGoThere(typeOfAccessory))
        {
            Debug.Log("This accessory can't go here");
            return false;
        }

        if (accesoriesAssigned[typeOfAccessory])
        {
            Debug.Log("Position occupied");
            OnRemoveAccessory(typeOfAccessory);
        }

        CarModifier[] modifiers = accessory.ModifiersInPosition(typeOfAccessory);
        accesoriesAssigned[typeOfAccessory] = accessory;
        for (int i = 0; i < modifiers.Length; i++)
        {
            OnModifierAdded?.Invoke(modifiers[i]);
        }
        carModifiers.AddRange(modifiers);
        accesoriesAssigned[typeOfAccessory] = accessory;
        UpdateCarInformation();
        Debug.Log(accessory.AccesoryInformation.AccessoryName + " Added");
        accessory.currentPosition = typeOfAccessory;
        OnCarModification?.Invoke(typeOfAccessory, accessory);
        OnCarUpdate?.Invoke(myCar);
        OnAccessoryDeselected?.Invoke();
        return true;

    }
    public CarAccessory OnRemoveAccessory(CarAccessoryType typeOfAccessory)
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

        CarAccessory result = accesoriesAssigned[typeOfAccessory];
        Debug.Log(accesoriesAssigned[typeOfAccessory].AccesoryInformation.AccessoryName + " Removed");
        accesoriesAssigned[typeOfAccessory] = null;
        UpdateCarInformation();
        result.currentPosition = CarAccessoryType.None;
        OnCarModification?.Invoke(typeOfAccessory, null);
        OnCarUpdate?.Invoke(myCar);
        return result;
    }
    public static void UpdateCar()
    {
        OnCarUpdate?.Invoke(instance.myCar);
    }

    public void OnAccesorySelected(CarAccessory accessory, CarAccessoryType type)
    {
        if (!type.Equals(CarAccessoryType.None) && accesoriesAssigned[type])
        {
            OnAccesorySelectedAction?.Invoke(myCar, accessory, myCar.carModifierInfo, type, accesoriesAssigned[type].ModifiersInPosition(type));
        }
        else
        {
            OnAccesorySelectedAction?.Invoke(myCar, accessory, myCar.carModifierInfo, type, null);
        }

    }
    public CarAttributeInformation[] GetModificatorsInformation()
    {
        CarAttributeInformation[] carInformation = new CarAttributeInformation[attributeDictionary.Count];
        for (int i = 0; i < attributeDictionary.Count; i++)
        {
            carInformation[i] = new CarAttributeInformation
            {
                ParameterType = (CarVarsType)i,
                TotalValue = attributeDictionary[(CarVarsType)i].GetTotalModificationValue()
            };
        }
        return carInformation;
    }
    public static bool IsValidCar()
    {
        return instance.accesoriesAssigned[CarAccessoryType.Wheels] && instance.accesoriesAssigned[CarAccessoryType.Engine];
    }
    //Update the real information for the physics
    private void UpdateCarInformation()
    {
        myCar.carModifierInfo.carMass = attributeDictionary[CarVarsType.Weight].CurrentValue;
        myCar.carModifierInfo.motorForce = attributeDictionary[CarVarsType.MotorForce].CurrentValue;
        myCar.carModifierInfo.steerAngle = attributeDictionary[CarVarsType.SteerAngle].CurrentValue;
        myCar.carModifierInfo.turboLength = attributeDictionary[CarVarsType.Turbo].CurrentValue;
        if (accesoriesAssigned[CarAccessoryType.Wheels])
        {
            myCar.carModifierInfo.wheelSize = accesoriesAssigned[CarAccessoryType.Wheels].AccesoryInformation.WheelSize;
            myCar.carModifierInfo.squareWheels = accesoriesAssigned[CarAccessoryType.Wheels].AccesoryInformation.IsSquareWheel;
        }
        myCar.OnChangeCarPiece();
    }

}
