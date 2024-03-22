using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    private SaveSlot[] saveSlots;
    private const string GameSceneName = "Game";
    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnClickSaveSlot(SaveSlot saveSlot)
    {
        DisableMenuButtons();
        DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
        if (!isLoadingGame)
        {
            DataPersistenceManager.Instance.NewGame();
        }

        DataPersistenceManager.Instance.SaveGame();
        SceneManager.LoadSceneAsync(GameSceneName);
    }

    

    public void OnClickBackButton()
    {
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        gameObject.SetActive(true);

        this.isLoadingGame = isLoadingGame;

        // Load all profiles that exist
        Dictionary<string, GameData> gameDataProfiles = DataPersistenceManager.Instance.GetAllProfilesGameData();

        GameObject firstSelected = backButton.gameObject;

        //loop through save slots and set content accordingly
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            gameDataProfiles.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            if(profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractible(false);
            }
            else
            {
                saveSlot.SetInteractible(true);
                if (firstSelected.Equals(backButton.gameObject))
                {
                    firstSelected = saveSlot.gameObject;
                }
            }
            Button firstSelectedButton = firstSelected.GetComponent<Button>();
            SetFirstSelected(firstSelectedButton);
        }
    }
    private void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractible(false);
        }
        backButton.interactable = false;
    }
}
