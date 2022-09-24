using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAccessory : MonoBehaviour
{
    [SerializeField]
    private CarAccessoryScriptable accesoryInformation;

    public CarAccessoryScriptable AccesoryInformation => accesoryInformation;

    public CarAccessoryType currentPosition;

    private Dictionary<CarAccessoryType, CarAccessoryInPositionInfo> informationPerPositionMap;

    

    private void Awake()
    {
        informationPerPositionMap = new Dictionary<CarAccessoryType, CarAccessoryInPositionInfo>();
        for (int i = 0; i < accesoryInformation.AccessoryPositionInfo.Length; i++)
        {
            if (!informationPerPositionMap.TryAdd(accesoryInformation.AccessoryPositionInfo[i].TypeOfTheAccesory, accesoryInformation.AccessoryPositionInfo[i]))
            {
                Debug.LogError("Duplicated position information in accesory " + accesoryInformation.AccessoryName + " : " + accesoryInformation.AccessoryPositionInfo[i].TypeOfTheAccesory);
             }
        }
    }
    public bool CanGoThere(CarAccessoryType accessoryType)
    {
        return informationPerPositionMap.ContainsKey(accessoryType);
    }
    public CarModifier[] ModifiersInPosition(CarAccessoryType accessoryType)
    {
        if(informationPerPositionMap.TryGetValue(accessoryType, out CarAccessoryInPositionInfo value))
        {
            return value.ModificationsInThisType;
        }
        return new CarModifier[0];
    }
}
public enum CarAccessoryType
{
    None,
    Engine,
    Wheels,
    Pipe,
    BodyWork,
    SteeringWheel,
    Gas
}
[Serializable]
public struct CarAccesoryPosition
{
    public CarAccessoryType AccessoryType;
    public Vector3 position; //O un transform 
}
public enum AccessoryOrientation
{
    XPositive,
    YPositive,
    ZPositive,
    XNegative,
    YNegative,
    ZNegative
}
[Serializable]
public struct CarAccessoryInPositionInfo
{
    public CarAccessoryType TypeOfTheAccesory;
    public CarModifier[] ModificationsInThisType;
    public AccessoryOrientation ForwardInPosition;

}