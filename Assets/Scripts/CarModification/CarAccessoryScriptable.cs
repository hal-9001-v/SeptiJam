using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CarAccessory", menuName = "It Works Somehow/Car Accessory", order = 1)]
public class CarAccessoryScriptable : ScriptableObject
{
    public string AccessoryName;

    public CarAccessoryInPositionInfo[] AccessoryPositionInfo;
    public GameObject AccessoryPrefab; //Quiz�s esto no haga falta, pero por si acaso

    //TODO: Diccionario con tipo de posicion y su informacion y un get desde ese diccionario
}

