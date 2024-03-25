using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    public event Action<bool> OnLoadDataCompleted;
    [SerializeField] private string fileName;
    
    /// <summary>
    /// Most Recently Played Game Data
    /// </summary>
    private GameData currentGameData;
    private bool hasData; 
    private List<IDataPersistence> dataPersistenceObjects;
    //private FileDataHandler dataHandler;
    private FirebaseDataHandler firebaseDataHandler;

    private string selectedProfileId = "";

    public static DataPersistenceManager Instance { get; private set; }

    private void Update()
    {
        //TODO: DEBUG REMOVE UPDATE LATER
        hasData = currentGameData != null;
    }


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
        //TODO bwlow line is also debug
        currentGameData = null;
        //dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        firebaseDataHandler = new FirebaseDataHandler();
        //selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        StartCoroutine(LoadMostRecentlyUpdatedProfileIdCoR(OnLoadProfileIdComplete));
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded; 
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded; 
    }
    private void OnApplicationQuit()
    {
        SaveGame();   
    }
    private void OnApplicationPause(bool pause)
    {
        SaveGame();
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        StartCoroutine(LoadMostRecentlyUpdatedProfileIdCoR(OnLoadProfileIdComplete));
    }

    public void NewGame()
    {
        this.currentGameData = new GameData();
    }
    public void SaveGame()
    {
        // If there is no data to save
        if(currentGameData == null)
        {
            Debug.LogWarning("No Data was found, a new game needs to be started before data can be saved");
            return;
        }

        // Pass data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(currentGameData);
        }

        currentGameData.lastUpdated = System.DateTime.Now.ToBinary();

        // Save Data file using data handler
        //dataHandler.Save(gameData, selectedProfileId);
        firebaseDataHandler.Save(currentGameData, selectedProfileId);
    }
 
    public void ChangeSelectedProfileId(string newProfileId, Action onProfileIdChangedAndGameLoaded)
    {
        this.selectedProfileId = newProfileId;
        StartCoroutine(LoadGameCoR(onProfileIdChangedAndGameLoaded));
    }

    public IEnumerator GetAllGameDataProfilesCoR(Action<Dictionary<string, GameData>> onLoadComplete)
    {
        yield return StartCoroutine(firebaseDataHandler.LoadAllProfilesCoR(onLoadComplete));
    }

    public IEnumerator LoadGameCoR(Action onLoadGameComplete = null)
    {
        // Load Any saved data from anywhere using data handler
        yield return StartCoroutine(firebaseDataHandler.LoadCoR(selectedProfileId, OnLoadGameComplete));
        onLoadGameComplete?.Invoke();
        // If there is no data then dont load anything;
        if (currentGameData == null)
        {
            Debug.Log("No Data was found, New Game Needs to be started before data can be loaded");
            OnLoadDataCompleted(currentGameData != null);
            yield break; 
        }

        // Push loaded data to local scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(currentGameData);
        }

    }
    public void OnLoadGameComplete(GameData data)
    {
        currentGameData = data;

        //TODO: remove this hard coded value if everything works fine
        OnLoadDataCompleted?.Invoke(currentGameData!=null);
    }

    private IEnumerator LoadMostRecentlyUpdatedProfileIdCoR(Action<string> onLoadComplete)
    {
        yield return StartCoroutine(firebaseDataHandler.GetMostRecentlyUpdatedProfileIdCoR(onLoadComplete));
    }
    private void OnLoadProfileIdComplete(string profileId)
    {
        selectedProfileId = profileId;
        StartCoroutine(LoadGameCoR());
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
    public bool HasCurrentGameData()
    {
        return currentGameData != null;
    }
}