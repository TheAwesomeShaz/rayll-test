using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour,IDataPersistence
{
    public event Action<string> OnUpdateCoinsUI;

    [SerializeField] private PlayerController playerController;

    private int mCoinsCollected;
    private const string MenuSceneName = "MainMenu";

    private void Start()
    {
        playerController.OnCoinCollected += PlayerController_OnCoinCollected;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DataPersistenceManager.Instance.SaveGame();
            SceneManager.LoadSceneAsync(MenuSceneName);
        }
    }

    private void PlayerController_OnCoinCollected(int coinValue)
    {
        mCoinsCollected+=coinValue;
        OnUpdateCoinsUI?.Invoke(mCoinsCollected.ToString());
    }

    public void LoadData(GameData data)
    {
        mCoinsCollected = 0;
        foreach (KeyValuePair<string,bool> pair in data.coinsCollected)
        {
            if (pair.Value)
            {
                mCoinsCollected++;
            }
        }

        OnUpdateCoinsUI?.Invoke(mCoinsCollected.ToString());
    }

    public void SaveData(GameData data)
    {
        // we are saving the coins from the coin Controllers itself by populating a dictionary
        // so dont need to implement here
    }
}
