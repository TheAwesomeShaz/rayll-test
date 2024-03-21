using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int coinsCollected;
    // TODO: save coinsCollected,coinsRemaining,coinsPositions,playerPosition,playerCurrentTarget(NavMeshAgent)

    // If new Game Data is created this will be the default values
    public GameData()
    {
        coinsCollected = 0;
    }
}
