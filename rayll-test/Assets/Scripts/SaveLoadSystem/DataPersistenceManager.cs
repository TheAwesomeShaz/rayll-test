using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    private GameData gameData;
    List<IDataPersistence> dataPersistenceObjects;

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("More than one Data Persistence Manager found!");
        }
        Instance = this;
    }

    private void Start()
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        // Load Any saved data from anywhere using data handler
        // IF there is no data then initialize to new game;
        if (gameData == null)
        {
            Debug.Log("No Data was found, Initializing to defaults");
            NewGame();
        }
        // Push loaded data to local scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }
    public void SaveGame()
    {
        // Pass data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        // Save Data file using data handler
    }
    private void OnApplicationQuit()
    {
        SaveGame();   
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
