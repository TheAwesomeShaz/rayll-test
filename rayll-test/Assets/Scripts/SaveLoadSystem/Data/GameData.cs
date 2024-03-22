using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public long lastUpdated;
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public SerializableDictionary<string, bool> coinsCollected;


    // If new Game Data is created this will be the default values
    public GameData()
    {
        playerPosition = Vector3.zero;
        playerRotation = Vector3.zero;
        coinsCollected = new SerializableDictionary<string, bool>();
    }

    public int GetPercentageCompleted()
    {
        int totalCoinsCollected = 0;
        foreach (bool collected in coinsCollected.Values)
        {
            if (collected)
            {
                totalCoinsCollected++;
            }
        }

        int percentageCompleted = -1;
        if(coinsCollected.Count != 0)
        {
            percentageCompleted = (totalCoinsCollected * 100 / coinsCollected.Count);
        }
        return percentageCompleted;

    }

}
