using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotsMenu : MonoBehaviour
{
    #region FIELDS
    private SaveSlot[] saveSlots;
    private bool isLoadingGame = false;
    #endregion

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    private void Start()
    {
        ActivateMenu(true);
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // Disable all buttons
        DisableSaveSlots();

        // Update the selected profile id to be used for data persistence
        DataManager.Instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
        
        if(!isLoadingGame)
        {
            // Create a new game, which will initialize our data to a clean slate
            DataManager.Instance.NewGame();
        }

        // Load the scene, which will in turn save the game because of OnSceneUnloaded() in the DataManager
        SceneManager.LoadSceneAsync("Prologue");
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        // Set mode
        this.isLoadingGame = isLoadingGame;

        // Load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataManager.Instance.GetAllProfilesGameData();

        // Loop through each save slot in the UI and set the content appropriately
        foreach(SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);

            // If loading a game, set it to not be interactable
            if(profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            } else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    public void DisableSaveSlots()
    {
        // Disable other save slots
        foreach(SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }

        // TODO - Disable back button
    }
}
