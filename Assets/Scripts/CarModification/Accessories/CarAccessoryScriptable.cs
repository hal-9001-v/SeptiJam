using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CarAccessory", menuName = "It Works Somehow/Car Accessory", order = 1)]
public class CarAccessoryScriptable : ScriptableObject
{
    public string AccessoryName;

    public CarAccessoryInPositionInfo[] AccessoryPositionInfo;

    public Sprite AccessoryIcon;
    public Material[] MeshMaterials; //Quizás esto no haga falta, pero por si acaso
    public Mesh AccessoryMesh;
    public Transform OriginalTransform;
    public WheelSize WheelSize;
    public Mesh AccessoryMeshChasisVariant;


}

