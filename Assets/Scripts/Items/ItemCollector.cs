using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class ItemCollector : MonoBehaviour
{
    Radio radio => FindObjectOfType<Radio>();

    public void AddClip(RadioClip radioClip)
    {
        radio.AddClip(radioClip);
    }

    public void AddCarPart(CarAccessory accesory)
    {
        PlayerInventory.OnAddObject(accesory);
        Debug.LogWarning("Collected!");
    }
}
