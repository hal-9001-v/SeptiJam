using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class DebugEditorUI : MonoBehaviour
{
    [SerializeField]
    private CarModificationManager carManager;
    [SerializeField]
    private CarAccesory[] debugDatabase;
    public void OnAddRandomAccessory()
    {
        carManager.OnAddAccesory(debugDatabase[UnityEngine.Random.Range(0, debugDatabase.Length - 1)], (CarAccessoryType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CarAccessoryType)).Length - 1));
    }
    public void OnRemoveRandomAccessory()
    {
        carManager.OnRemoveAccessory((CarAccessoryType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CarAccessoryType)).Length - 1));
    }


}
