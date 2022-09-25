using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InventoryInGame : MonoBehaviour
{
    [SerializeField]
    private GameObject inventoryUIObject;

    [SerializeField]
    private InventoryUI inventoryUI;

    [SerializeField]
    private InputActionAsset playerActions;

    public bool inventoryOpened;


    public void OnSelectPressed()
    {
        inventoryOpened = !inventoryOpened;
        inventoryUIObject.SetActive(inventoryOpened);
        if(inventoryOpened) FindObjectOfType<Speedometer>().HideUI();
        else FindObjectOfType<Speedometer>().ShowUI();
        if (inventoryOpened)
        {
            inventoryUI.OnOpen();
        }
      
    }
}
