using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.VisualScripting;

public class FileDataHandler
{

    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load(string profileId)
    {
        if(profileId == null)
        {
            return null;
        }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                // Load Serialized data from the file
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Deserialize the loaded data 
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error Occurred when Loading Data from file " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data, string profileId)
    {
        if(profileId == null)
        {
            return;
        }
        
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            // Create the file if it isn't created yet
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            // Save Serialized data into the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occurred when Saving Data to file "+fullPath +"\n"+e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new();

        //loop over all directory names in the data directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();

        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);

            // if theres a folder without any data file in it then skip it [This step not reqd for firebase]
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"skipping directory while loading all profiles since it doesnt contain data :{profileId}");
                continue;
            }

            // Load game data for this profile then put it in a dictionary
            GameData profileData = Load(profileId);
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogWarning($"Tried to load profile but something went wrong Profile Id:{profileId}");

            }
        }
        return profileDictionary;
    }

    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;

        Dictionary<string, GameData> gameDataProfiles = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in gameDataProfiles)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            // skip this entry if gameData is null
            if(gameData == null)
            {
                continue;
            }
            // if there was no game played before the only saved game profile is the latest profile
            if(mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            // if there was a game played before we will compare them according to date time
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(gameDataProfiles[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                // The Greatest date time value is the most recent
                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }
        return mostRecentProfileId;
    }

}
