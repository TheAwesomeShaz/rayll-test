using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private Button backButton;
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

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
        if (isLoadingGame)
        {
            DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            SaveGameAndLoadScene();  
        }
        // SaveSlot has data and you want to overwrite a saved game
        else if(saveSlot.HasData)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a new game will overwrite the data in current slot. Are you sure?",
                //TODO: (There is a better way to do this instead of arrow functions fix in refactor)
                // On Selecting Yes 
                () =>
                {
                    DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    DataPersistenceManager.Instance.NewGame();
                    SaveGameAndLoadScene();
                },
                // On Selecting No
                () =>
                {
                    this.ActivateMenu(isLoadingGame);
                }
                );
        }
        // Save slot is empty and you are starting new game
        else
        {
            DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            DataPersistenceManager.Instance.NewGame();
            SaveGameAndLoadScene();
        }
    }


    private void SaveGameAndLoadScene()
    {
        

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
        backButton.interactable = true;

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
