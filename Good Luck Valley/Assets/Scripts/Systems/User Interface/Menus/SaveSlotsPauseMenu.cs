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
    private Button backButton;
    [SerializeField] private ConfirmationPopupMenu deleteConfirmationMenu;
    [SerializeField] private ConfirmationPopupMenu overwriteConfirmationMenu;
    private SaveSlot[] saveSlots;
    private SaveSlot selectedSaveSlot;
    private Image savingProgressPanel;
    private Text savingProgressText;
    #endregion

    #region FIELDS
    private bool menuOpen = false;
    private bool saving = false;
    [SerializeField] private float saveCloseBuffer = 0.25f;
    [SerializeField] private float savingTimerAsSeconds = 3f;
    [SerializeField] private float savingTextUpdateTimer = 2.4f;
    [SerializeField] private float fadeAmountPerTick = 0.15f;
    private bool playAnimation = false;
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
        backButton = GameObject.Find("Save Back Button").GetComponent<Button>();

        savingProgressPanel = GameObject.Find("Saving Progress Panel").GetComponent<Image>();
        savingProgressText = GameObject.Find("Saving Progress Text").GetComponent<Text>();

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
        } else if(menuOpen)
        {
            saveCloseBuffer = 0.25f;
        }
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
        SetDeleteButtonInteractable();
        saveButton.interactable = true;

    }

    /// <summary>
    /// Activate the menu
    /// </summary>
    public void ActivateMenu()
    {
        // Enable the gameobject
        gameObject.GetComponent<Canvas>().enabled = true;

        // Activate buttons
        backButton.interactable = true;
        for (int i = 0; i < saveSlots.Length; i++)
        {
            saveSlots[i].SetInteractable(true);
        }

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
        backButton.interactable = false;
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

            // Show saving in progress UI elements and set saving text update timer
            savingProgressPanel.enabled = true;
            savingProgressText.enabled = false;
            savingProgressText.text = "Saving";
            savingTextUpdateTimer = 2.4f;

            // Set a delay for saving before re-activating the menu to allow time for the new save to appear
            saving = true;
            StartCoroutine(SaveDelay());
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
        // De-activate other buttons
        saveButton.interactable = false;
        deleteButton.interactable = false;
        DisableSaveSlots();

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

                // Show saving in progress UI elements and set saving text update timer
                savingProgressPanel.enabled = true;
                savingProgressText.enabled = true;
                savingProgressText.text = "Saving";
                savingTextUpdateTimer = 2.4f;

                // Set a delay for saving before re-activating the menu to allow time for the new save to appear
                saving = true;
                StartCoroutine(SaveDelay());
            },
            // Function to execute if we cancel
            () =>
            {
                // Reload the menu
                ActivateMenu();

                // Enable buttons
                SetDeleteButtonInteractable();
                saveButton.interactable = true;
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

                    // Reload the menu without further action
                    ActivateMenu();

                    // Enable buttons
                    SetDeleteButtonInteractable();
                    saveButton.interactable = true;
                },
                // Function to execute if we cancel
                () =>
                {
                    // Reload the menu without further action
                    ActivateMenu();

                    // Enable buttons
                    SetDeleteButtonInteractable();
                    saveButton.interactable = true;
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
        if (menuOpen && !saving)
        {
            // Close the journal UI and set menuOpen to false
            gameObject.GetComponent<Canvas>().enabled = false;
            menuOpen = false;

            pauseMenu.enabled = true;
        }
    }

    /// <summary>
    /// Set the interactablity of the delete button
    /// </summary>
    public void SetDeleteButtonInteractable()
    {
        if (selectedSaveSlot.HasData && DataManager.Instance.SelectedProfileID != DataManager.Instance.SoftProfileID)
        {
            // Only enable delete button if there is data to delete and the profile selected is not the current profile
            deleteButton.interactable = true;
        }
        else
        {
            deleteButton.interactable = false;
        }
    }

    /// <summary>
    /// Create a in-progress saving UI before reloading the menu to create time for file creation
    /// </summary>
    /// <returns></returns>
    private IEnumerator SaveDelay()
    {
        // Fade in UI elements
        yield return StartCoroutine(FadeInProgressUI());

        // Start the timer for saving
        yield return StartCoroutine(SaveTimer());

        // Stop animation
        playAnimation = false;
        StopCoroutine(SavingAnimation());

        // Fade out UI elements
        yield return StartCoroutine(FadeOutProgressUI());

        // Reload the menu
        ActivateMenu();

        // Enable buttons
        SetDeleteButtonInteractable();
        saveButton.interactable = true;

        saving = false;

        yield return null;
    }

    /// <summary>
    /// Hold the saving panel and text for a given time
    /// </summary>
    /// <returns></returns>
    private IEnumerator SaveTimer()
    {
        // Play animation while waiting
        playAnimation = true;
        StartCoroutine(SavingAnimation());
        yield return new WaitForSecondsRealtime(3f);
    }

    /// <summary>
    /// Play the saving animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator SavingAnimation()
    {
        // Check if it should be playing the animation, if so, enter a loop
        while(playAnimation)
        {
            // Yield so that other code can run
            yield return null;

            // Update savingProgressText based on timer
            if (savingTextUpdateTimer > 0)
            {
                if (savingTextUpdateTimer <= 2.4f && savingTextUpdateTimer > 1.8f)
                {
                    savingProgressText.text = "Saving";
                }
                else if (savingTextUpdateTimer <= 1.8f && savingTextUpdateTimer > 1.2f)
                {
                    savingProgressText.text = "Saving.";
                }
                else if (savingTextUpdateTimer <= 1.2f && savingTextUpdateTimer > 0.6f)
                {
                    savingProgressText.text = "Saving..";
                }
                else if (savingTextUpdateTimer <= 0.6f)
                {
                    savingProgressText.text = "Saving...";
                }
            }
            else if (savingTextUpdateTimer <= 0)
            {
                // Reset timer once it hits 0
                savingTextUpdateTimer = 2.4f;
            }

            // Subtract by unscaledDeltaTime (game is paused, so deltaTime won't work because it is scaled)
            savingTextUpdateTimer -= Time.unscaledDeltaTime;
        }
    }

    /// <summary>
    /// Fade in the Saving in Progress UI
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeInProgressUI()
    {
        // While alpha values are under the desired numbers, increase them by an unscaled delta time (because we are paused)
        while(savingProgressPanel.color.a < 0.67 && savingProgressText.color.a < 1)
        {
            savingProgressPanel.color = new Color(savingProgressPanel.color.r, savingProgressPanel.color.g, savingProgressPanel.color.b, savingProgressPanel.color.a + (Time.unscaledDeltaTime * 2));
            savingProgressText.color = new Color(savingProgressText.color.r, savingProgressText.color.g, savingProgressText.color.b, savingProgressText.color.a + (Time.unscaledDeltaTime * 3f));
            yield return null;
        }
    }

    /// <summary>
    /// Fade out the Saving in Progress UI
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOutProgressUI()
    {
        // While alpha values are over the desired numbers, decrease them by an unscaled delta time (because we are paused)
        while (savingProgressPanel.color.a > 0 && savingProgressText.color.a > 0)
        {
            savingProgressPanel.color = new Color(savingProgressPanel.color.r, savingProgressPanel.color.g, savingProgressPanel.color.b, savingProgressPanel.color.a - (Time.unscaledDeltaTime * 2));
            savingProgressText.color = new Color(savingProgressText.color.r, savingProgressText.color.g, savingProgressText.color.b, savingProgressText.color.a - (Time.unscaledDeltaTime * 3f));
        }

        // Disable the UI elements once they are gone
        savingProgressPanel.enabled = false;
        savingProgressText.enabled = false;

        yield return null;
    }
}
