using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

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
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded; 
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded; 
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
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
        gameData = dataHandler.Load(selectedProfileId);

        // If there is no data then dont load anything;
        if (gameData == null)
        {
            Debug.Log("No Data was found, New Game Needs to be started before data can be loaded");
            return;
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
        dataHandler.Save(gameData, selectedProfileId);
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

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }
 
    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        LoadGame();
    }
}