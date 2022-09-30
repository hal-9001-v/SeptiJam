using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{

    public Action OnUpdateInformation;

    private InventorySlot[] inventorySlots;
    List<CarAccessory> currentDisplay;

    CarShop[] carShops;

    [Header("Sounds")]
    [SerializeField] SoundInfo changePieceSound;

    private void Awake()
    {
        inventorySlots = new InventorySlot[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            inventorySlots[i] = transform.GetChild(i).GetComponent<InventorySlot>();
            inventorySlots[i].Init(this);
        }
        OnUpdateInformation += OnUpdate;
        currentDisplay = new List<CarAccessory>();
        carShops = FindObjectsOfType<CarShop>();
    }

    private void Start()
    {
        changePieceSound.Initialize(gameObject);
    }

    public void OnOpen()
    {
        OnClear();
        FindObjectOfType<Speedometer>().HideUI();
        OnChangeWindow(CarAccessoryType.None);
        EventSystem.current.SetSelectedGameObject(inventorySlots[0].gameObject);
        CarModificationManager.UpdateCar();
    }
    public void OnChangeWindow(CarAccessoryType objectFilter)
    {
        OnClear();
        if (objectFilter == CarAccessoryType.None)
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
        EventSystem.current.SetSelectedGameObject(inventorySlots[0].gameObject);


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

        changePieceSound.Play();
    }
    public void CloseInventory()
    {
        //TODO: I know this is the worst way of code this
        for(int i =0; i < carShops.Length; i++)
        {
            carShops[i].StopWorkShop();
        }
    }
}
