using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour,IDataPersistence
{
    public event Action<string> OnUpdateCoinsUI;

    private const string SceneName = "MainMenu";
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform[] coinTransforms;

    //TODO: remove SerializeField and make score private after debugging is done
    [SerializeField] private int coinsCollected;
    private void Start()
    {
        playerController.OnCoinCollected += PlayerController_OnCoinCollected;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DataPersistenceManager.Instance.SaveGame();
            SceneManager.LoadSceneAsync(SceneName);
        }
    }

    private void PlayerController_OnCoinCollected(int coinValue)
    {
        coinsCollected+=coinValue;
        OnUpdateCoinsUI?.Invoke(coinsCollected.ToString());
    }

    public void LoadData(GameData data)
    {
        coinsCollected = 0;
        foreach (KeyValuePair<string,bool> pair in data.coinsCollected)
        {
            if (pair.Value)
            {
                coinsCollected++;
            }
        }

        OnUpdateCoinsUI?.Invoke(coinsCollected.ToString());
    }

    public void SaveData(GameData data)
    {
        // we are saving the coins from the coin Controllers itself by populating a dictionary
        // so dont need to implement here
    }
}
