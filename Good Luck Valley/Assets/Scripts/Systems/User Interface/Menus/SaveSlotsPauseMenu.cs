using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotsPauseMenu : MonoBehaviour
{
    #region REFERENCES
    private Canvas pauseMenu;
    private Button saveButton;
    private Button deleteButton;
    [SerializeField] private ConfirmationPopupMenu deleteConfirmationMenu;
    [SerializeField] private ConfirmationPopupMenu overwriteConfirmationMenu;
    private SaveSlot[] saveSlots;
    private SaveSlot selectedSaveSlot;
    #endregion

    #region FIELDS
    private bool menuOpen = false;
    private float saveCloseBuffer = 0.25f;
    #endregion

    #region PROPERTIES
    public bool MenuOpen { get { return menuOpen; } set { menuOpen = value; } }
    public float CloseBuffer { get { return saveCloseBuffer; } }
    #endregion

    private void Awake()
    {
        pauseMenu = GameObject.Find("PauseUI").GetComponent<Canvas>();
        saveSlots = GetComponentsInChildren<SaveSlot>();
        selectedSaveSlot = null;

        saveButton = GameObject.Find("Profile Save Button").GetComponent<Button>();
        deleteButton = GameObject.Find("Delete Save Button").GetComponent<Button>();

        saveButton.interactable = false;
        deleteButton.interactable = false;
    }

    void Update()
    {
        // If the close buffer is set to above 0,
        // subtract by deltaTime
        if (saveCloseBuffer > 0 && !menuOpen)
        {
            saveCloseBuffer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Select the first buttno
    /// </summary>
    /// <param name="firstSelectedButton"></param>
    public void SetFirstSelected(Button firstSelectedButton)
    {
        firstSelectedButton.Select();
    }

    /// <summary>
    /// Select a Save Slot and change profile IDs
    /// </summary>
    /// <param name="saveSlot"></param>
    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // Check if there was a previously selected save slot
        if (selectedSaveSlot != null)
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
        DataManager.Instance.ChangeSelectedProfileIDSoft(saveSlot.GetProfileID());

        // Enable buttons
        if (saveSlot.HasData && DataManager.Instance.SelectedProfileID != DataManager.Instance.SoftProfileID)
        {
            // Only enable delete button if there is data to delete and the profile selected is not the current profile
            deleteButton.interactable = true;
        }
        saveButton.interactable = true;

    }

    /// <summary>
    /// Activate the menu
    /// </summary>
    public void ActivateMenu()
    {
        // Enable the gameobject
        gameObject.GetComponent<Canvas>().enabled = true;

        // Load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataManager.Instance.GetAllProfilesGameData();

        // Loop through each save slot in the UI and set the content appropriately
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);
        }

        // Set menu open
        menuOpen = true;
    }

    /// <summary>
    /// Disable the other save slots
    /// </summary>
    public void DisableSaveSlots()
    {
        // Disable other save slots
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }

        // TODO - Disable back button
    }

    /// <summary>
    /// Save to the current profile slot
    /// </summary>
    public void SaveProfile()
    {
        if (!selectedSaveSlot.HasData)
        {
            // If the current profile ID does not equal the soft profile ID, set it to the soft profile ID
            if(DataManager.Instance.SelectedProfileID != DataManager.Instance.SoftProfileID)
            {
                DataManager.Instance.ChangeSelectedProfileID(DataManager.Instance.SoftProfileID);
            }

            // Create a new save
            DataManager.Instance.NewGame();

            // Save the game
            DataManager.Instance.SaveGame();
        }
        else
        {
            // Prompt to overwrite the save if data exists
            OverwriteSave();
        }
    }

    /// <summary>
    /// Prompt to overwrite a save
    /// </summary>
    public void OverwriteSave()
    {
        // Activate the confirmation menu
        overwriteConfirmationMenu.ActivateMenu(
            "Are you sure you want to overwrite this save?",
            // Function to execute if we confirm
            () =>
            {
                // If the current profile ID does not equal the soft profile ID, set it to the soft profile ID
                if(DataManager.Instance.SelectedProfileID != DataManager.Instance.SoftProfileID)
                {
                    DataManager.Instance.ChangeSelectedProfileID(DataManager.Instance.SoftProfileID);
                }

                // Save the game
                DataManager.Instance.SaveGame();

                // TODO - wait a certain amount of time before updating

                // Reload the menu
                ActivateMenu();
            },
            // Function to execute if we cancel
            () =>
            {
                // Reload the menu
                ActivateMenu();
            });
    }

    /// <summary>
    /// Prompt to delete a save
    /// </summary>
    public void DeleteSave()
    {
        // Activate the confirmation menu
        deleteConfirmationMenu.ActivateMenu(
                "Are you sure you want to delete this save?",
                // Function to execute if we confirm
                () =>
                {
                    // Delete the data associated with the selected save slot's profile ID
                    DataManager.Instance.DeleteProfileData(selectedSaveSlot.GetProfileID());

                    if (!DataManager.Instance.HasGameData())
                    {
                        // Reload the menu
                        ActivateMenu();
                    }
                    else
                    {
                        // Reload the menu
                        ActivateMenu();
                    }
                },
                // Function to execute if we cancel
                () =>
                {
                    // Reload the menu without further action
                    ActivateMenu();
                }
            );
    }

    /// <summary>
    /// Go back into the previous menu
    /// </summary>
    public void Back()
    {
        // Deactivate the canvas
        gameObject.GetComponent<Canvas>().enabled = false;

        // Set menu open
        menuOpen = false;

        // Activate the pause menu
        pauseMenu.enabled = true;
    }

    /// <summary>
    /// Close the menu using a key
    /// </summary>
    public void CloseMenuKey()
    {
        // Check if the menu is open
        if (menuOpen)
        {
            // Close the journal UI and set menuOpen to false
            gameObject.GetComponent<Canvas>().enabled = false;
            menuOpen = false;

            pauseMenu.enabled = true;
        }
    }
}
