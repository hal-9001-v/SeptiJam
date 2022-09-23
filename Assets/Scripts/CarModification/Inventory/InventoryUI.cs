using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{

    public Action OnUpdateInformation;

    private InventorySlot[] inventorySlots;
    List<CarAccessory> currentDisplay;

    private void Awake()
    {
        inventorySlots = new InventorySlot[transform.childCount];
        for(int i =0; i< transform.childCount; i++)
        {
            inventorySlots[i] = transform.GetChild(i).GetComponent<InventorySlot>();
            inventorySlots[i].Init(this);
        }
        OnUpdateInformation += OnUpdate;
        currentDisplay = new List<CarAccessory>();
    }
    public void OnOpen()
    {
        OnClear();
        OnChangeWindow(CarAccessoryType.None);
    }
    public void OnChangeWindow(CarAccessoryType objectFilter)
    {
        OnClear();
        if(objectFilter == CarAccessoryType.None)
        {
            for (int i = 0; i < PlayerInventory.Objects.Count; i++)
            {
                inventorySlots[i].OnAddAccesory(PlayerInventory.Objects[i], objectFilter);
            }
            currentDisplay = PlayerInventory.Objects;
        }
        else
        {
            List<CarAccessory> filteredAccesories = PlayerInventory.FiltredPositionOfAccesory(objectFilter);
            for (int i = 0; i < filteredAccesories.Count; i++)
            {
                inventorySlots[i].OnAddAccesory(filteredAccesories[i], objectFilter);
            }
            currentDisplay = filteredAccesories;

        }
       
      
    }
    public void OnChangeWindow(int i)
    {
        CarAccessoryType objectsFilter = (CarAccessoryType)i;
        OnChangeWindow(objectsFilter);


    }
    private void OnClear()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].OnClearWindow();
        }
    }
    public void OnUpdate()
    {
        for (int i = 0; i < currentDisplay.Count; i++)
        {
            inventorySlots[i].OnUpdate();
        }
    }
}
