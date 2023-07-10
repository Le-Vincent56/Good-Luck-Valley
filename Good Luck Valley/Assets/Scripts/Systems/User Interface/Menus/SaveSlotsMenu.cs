using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    #region REFERENCES
    private Button startButton;
    private Button deleteButton;
    [SerializeField] private ConfirmationPopupMenu deleteConfirmationMenu;
    private SaveSlot[] saveSlots;
    private SaveSlot selectedSaveSlot;
    #endregion

    #region FIELDS
    #endregion

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
        selectedSaveSlot = null;
    }

    private void Start()
    {
        startButton = GameObject.Find("Start").GetComponent<Button>();
        deleteButton = GameObject.Find("Delete").GetComponent<Button>();

        startButton.interactable = false;
        deleteButton.interactable = false;

        ActivateMenu();
    }

    /// <summary>
    /// Select the first button
    /// </summary>
    /// <param name="firstSelectedButton">The first button to be selected</param>
    public void SetFirstSelected(Button firstSelectedButton)
    {
        firstSelectedButton.Select();
    }

    /// <summary>
    /// Select a Save Slot and change profile IDs
    /// </summary>
    /// <param name="saveSlot">The save slot being clicked</param>
    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // Check if there was a previously selected save slot
        if(selectedSaveSlot != null)
        {
            // Deselect the last save slot
            selectedSaveSlot.Selected = false;

            // Deselect the button
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        // Select the new save slot
        saveSlot.Selected = true;
        saveSlot.gameObject.GetComponent<Button>().Select();
        selectedSaveSlot = saveSlot;

        // Update the selected profile id to be used for data persistence
        DataManager.Instance.ChangeSelectedProfileID(saveSlot.GetProfileID());

        // Load the game to update the profile
        DataManager.Instance.LoadGame();

        // Enable buttons
        if(saveSlot.HasData)
        {
            // Only enable delete button if there is data to delete
            deleteButton.interactable = true;
        }
        startButton.interactable = true;
        
    }

    /// <summary>
    /// Activate the menu
    /// </summary>
    public void ActivateMenu()
    {
        // Load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataManager.Instance.GetAllProfilesGameData();

        // Loop through each save slot in the UI and set the content appropriately
        foreach(SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);
        }
    }

    /// <summary>
    /// Disable the other save slots
    /// </summary>
    public void DisableSaveSlots()
    {
        // Disable other save slots
        foreach(SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
    }

    /// <summary>
    /// Load the selected save slot
    /// </summary>
    public void StartSave()
    {
        if (!selectedSaveSlot.HasData)
        {
            // Create a new game, which will initialize our data to a clean slate
            DataManager.Instance.NewGame();

            SaveGameAndLoadScene("Prologue");
        }
        else
        {
            // Change to the selected profile ID
            DataManager.Instance.ChangeSelectedProfileID(selectedSaveSlot.GetProfileID());

            // Load the game to update data
            DataManager.Instance.LoadGame();

            // Save the game before loading a new scene
            SaveGameAndLoadScene(DataManager.Instance.Level);
        }
    }

    /// <summary>
    /// Delete the selected save slot
    /// </summary>
    public void DeleteSave()
    {
        Debug.Log("Huh");
        // Activate the confirmation menu
        deleteConfirmationMenu.ActivateMenu(
                "Are you sure you want to delete this save?",
                // Function to execute if we confirm
                () =>
                {
                    Debug.Log("Huh");
                    // Delete the data associated with the selected save slot's profile ID
                    DataManager.Instance.DeleteProfileData(selectedSaveSlot.GetProfileID());

                    if(!DataManager.Instance.HasGameData())
                    {
                        // Reload the menu
                        ActivateMenu();

                        // Go back to main menu
                        SceneManager.LoadSceneAsync("Main Menu");

                    } else
                    {
                        // Reload the menu
                        ActivateMenu();
                    }
                },
                // Function to execute if we cancel
                () =>
                {
                    Debug.Log("Other Huh");
                    // Reload the menu without further action
                    ActivateMenu();
                }
            );
    }

    /// <summary>
    /// Save the game and load a scene
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    private void SaveGameAndLoadScene(string sceneToLoad)
    {
        // Save the game before loading a new scene
        DataManager.Instance.SaveGame();

        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
