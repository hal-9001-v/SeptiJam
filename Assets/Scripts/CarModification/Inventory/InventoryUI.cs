using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{

    public Action OnUpdateInformation;

    [SerializeField]
    private InventorySlot[] inventorySlots;
    List<CarAccessory> currentDisplay;

    private void Awake()
    {
        for(int i =0; i< inventorySlots.Length; i++)
        {
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
