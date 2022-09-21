using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataFileManager
{
    static public PlayerData playerData;
    static public CarData carData;
    static public InventoryData inventoryData;

    static string dataPath = "//SomehowItWorks.json";

    public static void LoadData()
    {
        var fullPath = Application.dataPath + dataPath;
        if (File.Exists(fullPath))
        {
            var gameData = JsonUtility.FromJson<GameData>(File.ReadAllText(fullPath));

            playerData = gameData.playerData;
            carData = gameData.carData;
            inventoryData = gameData.inventoryData;


            Debug.Log("Reading from " + fullPath);
        }
        else
        {
            playerData = new PlayerData();
            carData = new CarData();
            inventoryData = new InventoryData();

            Debug.Log("No Data in " + fullPath);
            SaveData();
        }

    }

    public static void SaveData()
    {
        if (playerData == null)
        {
            playerData = new PlayerData();
            Debug.LogWarning("No player Data in DataFileManager!");
        }

        if (carData == null)
        {
            carData = new CarData();
            Debug.LogWarning("No Car Data in DataFileManager!");
        }

        if (inventoryData == null)
        {
            inventoryData = new InventoryData();
            Debug.LogWarning("No Inventory Data in DataFileManager");
        }

        GameData gameData = new GameData();

        gameData.playerData = playerData;
        gameData.carData = carData;
        gameData.inventoryData = inventoryData;

        var fullPath = Application.dataPath + dataPath;

        File.WriteAllText(fullPath, JsonUtility.ToJson(gameData));
        Debug.Log("Saving to " + fullPath);

    }


    // Put your Data classes down here!!! Add them in the LoadData(), SaveData() and as static members of this class
    [Serializable]
    class GameData
    {
        public PlayerData playerData;
        public CarData carData;
        public InventoryData inventoryData;

    }

    [Serializable]
    public class PlayerData
    {
        public string checkPointId;

        public PlayerData()
        {

        }

        public PlayerData(string checkPointId)
        {
            this.checkPointId = checkPointId;
        }
    }

    [Serializable]
    public class InventoryData
    {
        public int itemCount = 0;

        public InventoryData()
        {

        }

        public InventoryData(int itemCount)
        {
            this.itemCount = itemCount;
        }
    }

    [Serializable]
    public class CarData
    {
        public CarData()
        {

        }
    }
}
