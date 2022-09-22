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
    private CarAccessory[] debugDatabase;
    [SerializeField]
    private InventoryUI inventory;
    private int currentWindow = 0;

    public void OnAddRandomAccessory()
    {
        PlayerInventory.OnAddObject(debugDatabase[UnityEngine.Random.Range(0, debugDatabase.Length )]);
        inventory.OnChangeWindow((CarAccessoryType)currentWindow);
    }
    public void OnRemoveRandomAccessory()
    {
        PlayerInventory.OnRemoveObject(UnityEngine.Random.Range(0, PlayerInventory.Objects.Count));
        inventory.OnChangeWindow((CarAccessoryType)currentWindow);
        //carManager.OnRemoveAccessory((CarAccessoryType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CarAccessoryType)).Length - 1));
    }
    public void OnChangeFilter()
    {
        currentWindow = (currentWindow+1) % (Enum.GetValues(typeof(CarAccessoryType)).Length-1);
        inventory.OnChangeWindow((CarAccessoryType)currentWindow);
        Debug.Log("Changed to " + ((CarAccessoryType)currentWindow).ToString());
    }
    public void OnOpen()
    {
        inventory.gameObject.SetActive(true);
        inventory.OnOpen();
        currentWindow = 0;
    }
    public void OnClose()
    {
        inventory.gameObject.SetActive(false);
    }


}
