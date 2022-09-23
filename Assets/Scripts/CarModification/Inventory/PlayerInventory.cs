using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    //Singleton
    private static PlayerInventory instance;
    [SerializeField] //TODO: This is for debug purposes, remove serialized field later
    private List<CarAccessory> playerInventory;

    public static List<CarAccessory> Objects => instance.playerInventory;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //playerInventory = new List<CarAccessory>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void OnAddObject(CarAccessory accesory)
    {
        if (Objects.Contains(accesory))
        {
            return;
        }
        instance.playerInventory.Add(accesory);
    }

    public static CarAccessory OnRemoveObject(int index)
    {
        if (index >= instance.playerInventory.Count)
        {
            return null;
        }
        CarAccessory result = instance.playerInventory[index];
        instance.playerInventory.RemoveAt(index);
        
        return result;

    }

    public static List<CarAccessory> FiltredPositionOfAccesory(CarAccessoryType accessoryPosition)
    {
        return instance.playerInventory.FindAll((accessory) => accessory.CanGoThere(accessoryPosition));
    }
}
