using System;
using UnityEngine;

public class GameManager : MonoBehaviour,IDataPersistence
{
    public event Action<string> OnUpdateCoinsUI;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform[] coins;

    //TODO: remove SerializeField and make score private after debugging is done
    [SerializeField] private int coinsCollected;
    private void Start()
    {
        playerController.OnCoinCollected += PlayerController_OnCoinCollected;
        
        //TODO: Later set the score from the database
        coinsCollected = 0;
        OnUpdateCoinsUI?.Invoke(coinsCollected.ToString());
    }

    private void PlayerController_OnCoinCollected(int coinValue)
    {
        coinsCollected+=coinValue;
        OnUpdateCoinsUI?.Invoke(coinsCollected.ToString());
    }

    public void LoadData(GameData data)
    {
        coinsCollected = data.coinsCollected;
    }

    public void SaveData(ref GameData data)
    {
        data.coinsCollected = coinsCollected;
    }
}
