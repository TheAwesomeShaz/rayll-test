using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    //private FileDataHandler dataHandler;
    private FirebaseDataHandler firebaseDataHandler;

    private string selectedProfileId = "";

    public static DataPersistenceManager Instance { get; private set; }


    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("More than one Data Persistence Manager found, Destroying newest One");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        firebaseDataHandler = new FirebaseDataHandler();
        //selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        StartCoroutine(LoadSelectedProfileIdCoR(OnSelectedProfileIdLoaded));
    }

    private void OnSelectedProfileIdLoaded(string profileId)
    {
        selectedProfileId = profileId;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded; 
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded; 
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        StartCoroutine(LoadGame());
    }

    public void OnLoadGameComplete(GameData data)
    {
        gameData = data;
    }

    public IEnumerator LoadGame()
    {
        // Load Any saved data from anywhere using data handler
        yield return StartCoroutine(firebaseDataHandler.Load(selectedProfileId, OnLoadGameComplete));

        // If there is no data then dont load anything;
        if (gameData == null)
        {
            Debug.Log("No Data was found, New Game Needs to be started before data can be loaded");
            yield break; 
        }

        // Push loaded data to local scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

    }
    public void SaveGame()
    {
        // If there is no data to save
        if(gameData == null)
        {
            Debug.LogWarning("No Data was found, a new game needs to be started before data can be saved");
            return;
        }

        // Pass data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // Save Data file using data handler
        //dataHandler.Save(gameData, selectedProfileId);
        firebaseDataHandler.Save(gameData, selectedProfileId);
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
    public bool HasGameData()
    {
        return gameData != null;
    }

    public IEnumerator GetAllGameDataProfiles(Action<Dictionary<string, GameData>> onLoadComplete)
    {
        yield return StartCoroutine(firebaseDataHandler.LoadAllProfilesCoR(onLoadComplete));
    }
 
    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadSelectedProfileIdCoR(Action<string> onLoadComplete)
    {
        yield return StartCoroutine(firebaseDataHandler.GetMostRecentlyUpdatedProfileIdCoR(onLoadComplete));
    }
}