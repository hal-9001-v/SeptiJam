using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySlot : Button
{
    internal enum SlotState
    {
        Empty,
        NotUsed,
        UsedForOtherThing,
        Used,
        General
    }
    [SerializeField]
    private GameObject onSelectFrame;

    [SerializeField]
    private GameObject onAddedFrame;

    [SerializeField]
    private Color colorAdded;

    [SerializeField]
    private Color alreadyInCarColor;

    [SerializeField]
    private Image accessoryIcon;

    private CarAccessory accesoryReference;

    private SlotState slotState;

    private InventoryUI inventory;

    private CarAccessoryType currentPosition;
    public bool IsOccupied => accessoryIcon.sprite;

    internal SlotState CurrentSlotState { get => slotState; set
        {
            slotState = value;
            OnChangeState();
        }
    }
    protected override void Awake()
    {
        base.Awake();
        onClick.AddListener(OnClick);
    }
    public void Init(InventoryUI inventory)
    {
        this.inventory = inventory;

    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        onSelectFrame.SetActive(true);

    }
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        onSelectFrame.SetActive(false);
    }
    private void OnClick()
    {
        Debug.Log("Current State: " + slotState.ToString());
     
        switch (slotState)
        {
            case SlotState.NotUsed:
                CarModificationManager.instance.OnAddAccesory(accesoryReference,currentPosition);
                inventory.OnUpdateInformation?.Invoke();
                break;
            case SlotState.UsedForOtherThing:
                //nothing
                break;
            case SlotState.Used:
                CarModificationManager.instance.OnRemoveAccessory(currentPosition);
                inventory.OnUpdateInformation?.Invoke();
                break;
            case SlotState.Empty:
                return;
                //nothing
                break;
            case SlotState.General:
                //nothing
                break;
        }
        Debug.Log("Clicked on: " + accesoryReference.AccesoryInformation.AccessoryName);
    }
    public void OnClearWindow()
    {
        CurrentSlotState = SlotState.Empty;
        accesoryReference = null;
        accessoryIcon.sprite = null;
        accessoryIcon.color = new Color(0, 0, 0, 0);
    }

    private void OnChangeState()
    {
        if (currentPosition == CarAccessoryType.None)
        {
            return;
        }
        switch (slotState)
        {
            case SlotState.NotUsed:
                accessoryIcon.sprite = accesoryReference.AccesoryInformation.AccessoryIcon;
                accessoryIcon.color = Color.white;
                onAddedFrame.SetActive(false);
                break;
            case SlotState.UsedForOtherThing:
                accessoryIcon.sprite = accesoryReference.AccesoryInformation.AccessoryIcon;
                accessoryIcon.color = alreadyInCarColor;
                onAddedFrame.SetActive(false);
                break;
            case SlotState.Used:
                accessoryIcon.sprite = accesoryReference.AccesoryInformation.AccessoryIcon;
                accessoryIcon.color = colorAdded;
                onAddedFrame.SetActive(true);
                break;
            case SlotState.Empty:
                accessoryIcon.sprite = null;
                accessoryIcon.color = new Color(0, 0, 0, 0);
                onAddedFrame.SetActive(false);
                break;
        }
    }
    public void OnUpdate()
    {
        if(currentPosition == CarAccessoryType.None)
        {
            if (accesoryReference.currentPosition.Equals(CarAccessoryType.None))
            {
                CurrentSlotState = SlotState.General;
            }
            else
            {
                CurrentSlotState = SlotState.General;
                //TODO: new state when in general but used
            }
         
            return;
        }
        if(!accesoryReference.CanGoThere(currentPosition))
        {
            CurrentSlotState = SlotState.Empty;
            return;
        }
        if(accesoryReference.currentPosition == currentPosition)
        {
            CurrentSlotState = SlotState.Used;
        }
        else if(accesoryReference.currentPosition.Equals(CarAccessoryType.None))
        {
            CurrentSlotState = SlotState.NotUsed;
        }
        else
        {
           CurrentSlotState = SlotState.UsedForOtherThing;
        }
    }
    public void OnAddAccesory(CarAccessory accessory, CarAccessoryType position)
    {
        accesoryReference = accessory;
        currentPosition = position;
        accessoryIcon.sprite = accessory.AccesoryInformation.AccessoryIcon;
        accessoryIcon.color = Color.white;
        OnUpdate();
    }






}
