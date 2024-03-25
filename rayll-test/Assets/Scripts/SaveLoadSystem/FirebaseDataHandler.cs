using Firebase;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseDataHandler 
{
    private DatabaseReference dbReference;
    public FirebaseDataHandler()
    {
        // Initialize FirebaseApp if not already initialized
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogError($"Failed to initialize Firebase: {task.Exception}");
                }
            });
        }
        Debug.Log($"Firebase Initialized Successfully");
        // FirebaseApp is already initialized, get reference to FirebaseDatabase
        InitializeDatabaseReference();
    }

    private void InitializeDatabaseReference()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Save(GameData gameData, string profileId)
    {
        if(profileId == null)
        {
            return;
        }

        string json = JsonUtility.ToJson(gameData);
        dbReference.Child("users").Child(profileId).SetRawJsonValueAsync(json);
        Debug.Log($"SAVING GAME BEFORE QUITTING COMPLETE");
    }

    public IEnumerator LoadCoR(string profileId, Action<GameData> OnLoadComplete)
    {
        if(profileId == null)
        {
            yield break;
        }

        var serverData = dbReference.Child("users").Child(profileId).GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        //Debug.Log(jsonData);

        if (jsonData != null)
        {
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonData);
            OnLoadComplete?.Invoke(loadedData);
        }
        else
        {
            Debug.Log($"Failed to load data for profile:{profileId}");
            OnLoadComplete?.Invoke(null);
        }
    }

    public IEnumerator GetMostRecentlyUpdatedProfileIdCoR(Action<string> onComplete)
    {
        string mostRecentProfileId = null;
        DateTime mostRecentDateTime = DateTime.MinValue;

        IEnumerator LoadAllProfilesRoutine = LoadAllProfilesCoR(gameDataProfiles =>
        {
            if (gameDataProfiles == null)
            {
                onComplete?.Invoke(null);
                return;
            }

            // Loop over profiles to find the most recent one
            foreach (KeyValuePair<string, GameData> pair in gameDataProfiles)
            {
                string profileId = pair.Key;
                GameData gameData = pair.Value;

                // Skip if gameData is null
                if (gameData == null)
                    continue;

                // Convert lastUpdated to DateTime
                DateTime profileLastUpdated = DateTime.FromBinary(gameData.lastUpdated);

                if (profileLastUpdated > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                    mostRecentDateTime = profileLastUpdated;
                }
            }
            onComplete?.Invoke(mostRecentProfileId);
        });

        while (LoadAllProfilesRoutine.MoveNext())
        {
            yield return LoadAllProfilesRoutine.Current;
        }
    }

    public IEnumerator LoadAllProfilesCoR(Action<Dictionary<string, GameData>> onComplete)
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        var getDataTask = dbReference.GetValueAsync();
        yield return new WaitUntil(() => getDataTask.IsCompleted);

        if (getDataTask.Exception != null)
        {
            Debug.LogError($"Failed to load profiles from Firebase: {getDataTask.Exception}");
            onComplete?.Invoke(null);
            yield break;
        }

        DataSnapshot dataSnapshot = getDataTask.Result;

        foreach (DataSnapshot profileSnapshot in dataSnapshot.Child("users").Children)
        {
            string profileId = profileSnapshot.Key;
            GameData profileData = JsonUtility.FromJson<GameData>(profileSnapshot.GetRawJsonValue());
            profileDictionary.Add(profileId, profileData);
        }
        Debug.Log("Loaded all profiles from Firebase.");
        onComplete?.Invoke(profileDictionary);
    }
}
