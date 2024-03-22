using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button loadGameButton;

    private const string GameSceneName = "Game";

    private void Start()
    {
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueButton.interactable = false;
            loadGameButton.interactable = false;
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
