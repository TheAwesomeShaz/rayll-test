using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject loadingUI;

    private const string GameSceneName = "Game";

    private void Awake()
    {
        DataPersistenceManager.Instance.OnLoadDataCompleted += DataPersistenceManager_OnLoadDataCompleted;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //TODO: remove the hard code in refactor
        if(loadingUI!=null)
        {
            //Enable Loading UI
            loadingUI.SetActive(true);
            mainMenuUI.SetActive(false);
        }
    }

    private void DataPersistenceManager_OnLoadDataCompleted(bool hasData)
    {
        //TODO: remove the hard code in refactor
        if (loadingUI!=null)
        {
            // DisableLoadingUI
            loadingUI.SetActive(false);
            mainMenuUI.SetActive(true);

            if (!hasData)
            {
                continueButton.interactable = false;
                loadGameButton.interactable = false;
            }
        }
    }

    public void OnClickNewGame()
    {
        saveSlotsMenu.ActivateMenu(false);
        DeactivateMenu();
    }

    public void OnClickLoadGame()
    {
        saveSlotsMenu.ActivateMenu(true);
        DeactivateMenu();
    }

    public void OnClickContinue()
    {
        DisableAllButtons();
        DataPersistenceManager.Instance.SaveGame();
        SceneManager.LoadSceneAsync(GameSceneName);
    }

    private void DisableAllButtons()
    {
        newGameButton.interactable = false;
        continueButton.interactable = false;   
    }

    public void ActivateMenu()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
}
