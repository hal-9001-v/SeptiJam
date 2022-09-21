using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataFileManager;

[RequireComponent(typeof(CheckPointTracker))]
public class GameSaver : MonoBehaviour
{
    CheckPointTracker checkPointTracker => GetComponent<CheckPointTracker>();
    Car car => FindObjectOfType<Car>();
    PlayerMovement playerMovement => FindObjectOfType<PlayerMovement>();


    // Start is called before the first frame update
    void Start()
    {
        checkPointTracker.enterCheckpointCallback += SaveGame;

        LoadGame();
    }

    void LoadGame()
    {
        DataFileManager.LoadData();

        var playerData = DataFileManager.playerData;
        checkPointTracker.SetCheckPoint(playerData.checkPointId);
        
        car.SetCarData(DataFileManager.carData);

        //inventory.SetData(DataFileManager.inventoryData)

        checkPointTracker.Respawn();
    }

    void SaveGame()
    {
        DataFileManager.playerData = new PlayerData();
        playerData.checkPointId = checkPointTracker.CurrentCheckPoint.CheckPointId;

        DataFileManager.carData = car.GetCarData();

        DataFileManager.inventoryData = new InventoryData();

        DataFileManager.SaveData();
    }
}
