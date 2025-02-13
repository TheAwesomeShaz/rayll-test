using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private Button backButton;
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private GameObject saveSlotsUI;


    private SaveSlot[] mSaveSlots;
    private const string GameSceneName = "Game";
    private bool mIsLoadingGame = false;
    private SaveSlot mCurrentClickedSaveSlot;
    private Dictionary<string, GameData> mGameDataProfiles;

    private void Awake()
    {
        loadingUI.SetActive(false);
        mSaveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnClickSaveSlot(SaveSlot saveSlot)
    {
        mCurrentClickedSaveSlot = saveSlot;
        DisableMenuButtons();
        if (mIsLoadingGame)
        {
            DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId(), LoadSavedGame);
        }
        // SaveSlot has data and you want to overwrite a saved game
        else if(saveSlot.HasData)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a new game will overwrite the data in current slot. Are you sure?",
                // On Selecting Yes
                OnYesSelected,
                // On Selecting No
                OnNoSelected
                );
        }
        // Save slot is empty and you are starting new game
        else
        {
            DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId(), LoadNewGame);
        }
    }

    private void OnYesSelected()
    {
        DataPersistenceManager.Instance.ChangeSelectedProfileId(mCurrentClickedSaveSlot.GetProfileId(), LoadNewGame);
    }
    private void OnNoSelected()
    {
        this.ActivateMenu(mIsLoadingGame);
    }
    private void LoadSavedGame()
    {
        SaveGameAndLoadScene();
    }
    private void LoadNewGame()
    {
        DataPersistenceManager.Instance.NewGame();
        SaveGameAndLoadScene();
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
      
        this.mIsLoadingGame = isLoadingGame;
        gameObject.SetActive(true);
        loadingUI.SetActive(true);
        saveSlotsUI.SetActive(false);

        StartCoroutine(LoadAllProfilesCoR());
    }
    private void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in mSaveSlots)
        {
            saveSlot.SetInteractible(false);
        }
        backButton.interactable = false;
    }
    public IEnumerator LoadAllProfilesCoR()
    {
        yield return StartCoroutine(DataPersistenceManager.Instance.GetAllGameDataProfilesCoR(OnGetAllGameDataProfilesCompleted));
    }
    public void OnGetAllGameDataProfilesCompleted(Dictionary<string, GameData> profiles)
    {

        // TODO: Set Loading UI InActive
        loadingUI.SetActive(false);
        saveSlotsUI.SetActive(true);
        backButton.interactable = true;

        mGameDataProfiles = profiles;

        GameObject firstSelected = backButton.gameObject;

        //loop through save slots and set content accordingly
        foreach (SaveSlot saveSlot in mSaveSlots)
        {
            GameData profileData = null;
            mGameDataProfiles.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            if (profileData == null && mIsLoadingGame)
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
}
