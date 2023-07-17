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
    [SerializeField] private Image deleteProgressPanel;
    [SerializeField] private Text deleteProgressText;
    #endregion

    #region FIELDS
    private float deleteTextUpdateTimer = 2.4f;
    private bool deleting = false;
    private bool playAnimation = false;
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
    /// Delete the selected save slot
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

                    ShowDeleteOverlay();
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
    /// Save the game and load a scene
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    private void SaveGameAndLoadScene(string sceneToLoad)
    {
        // Save the game before loading a new scene
        DataManager.Instance.SaveGame();

        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    /// <summary>
    /// Show the delete overlay
    /// </summary>
    private void ShowDeleteOverlay()
    {
        // Show saving in progress UI elements and set saving text update timer
        deleteProgressPanel.enabled = true;
        deleteProgressText.enabled = true;
        deleteProgressText.text = "Deleting";
        deleteTextUpdateTimer = 2.4f;

        // Set a delay for saving before re-activating the menu to allow time for the new save to appear
        deleting = true;
        StartCoroutine(DeleteDelay());
    }

    /// <summary>
    /// Progress the delete loading screen
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeleteDelay()
    {
        // Fade in UI elements
        // Fade in UI elements
        yield return StartCoroutine(FadeInProgressUI());

        // Start the timer for saving
        yield return StartCoroutine(DeleteTimer());

        // Stop animation
        playAnimation = false;
        StopCoroutine(DeletingAnimation());

        // Fade out UI elements
        yield return StartCoroutine(FadeOutProgressUI());

        // Check if there's game data
        if (!DataManager.Instance.HasGameData())
        {
            // Reload the menu
            ActivateMenu();

            // Go back to main menu
            SceneManager.LoadSceneAsync("Main Menu");
        }
        else
        {
            // Reload the menu
            ActivateMenu();
        }

        // Enable buttons
        SetDeleteButtonInteractable();
        startButton.interactable = true;

        deleting = false;

        yield return null;
    }

    /// <summary>
    /// Hold the deleting panel and text for a given time
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeleteTimer()
    {
        // Play animation while waiting
        playAnimation = true;
        StartCoroutine(DeletingAnimation());
        yield return new WaitForSecondsRealtime(3f);
    }

    /// <summary>
    /// Play the deleting animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeletingAnimation()
    {
        // Check if it should be playing the animation, if so, enter a loop
        while (playAnimation)
        {
            // Yield so that other code can run
            yield return null;

            // Update savingProgressText based on timer
            if (deleteTextUpdateTimer > 0)
            {
                if (deleteTextUpdateTimer <= 2.4f && deleteTextUpdateTimer > 1.8f)
                {
                    deleteProgressText.text = "Deleting";
                }
                else if (deleteTextUpdateTimer <= 1.8f && deleteTextUpdateTimer > 1.2f)
                {
                    deleteProgressText.text = "Deleting.";
                }
                else if (deleteTextUpdateTimer <= 1.2f && deleteTextUpdateTimer > 0.6f)
                {
                    deleteProgressText.text = "Deleting..";
                }
                else if (deleteTextUpdateTimer <= 0.6f)
                {
                    deleteProgressText.text = "Deleting...";
                }
            }
            else if (deleteTextUpdateTimer <= 0)
            {
                // Reset timer once it hits 0
                deleteTextUpdateTimer = 2.4f;
            }

            // Subtract by unscaledDeltaTime (game is paused, so deltaTime won't work because it is scaled)
            deleteTextUpdateTimer -= Time.unscaledDeltaTime;
        }
    }

    /// <summary>
    /// Fade in the Deleting in Progress UI
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeInProgressUI()
    {
        // While alpha values are under the desired numbers, increase them by an unscaled delta time (because we are paused)
        while (deleteProgressPanel.color.a < 0.67 && deleteProgressText.color.a < 1)
        {
            deleteProgressPanel.color = new Color(deleteProgressPanel.color.r, deleteProgressPanel.color.g, deleteProgressPanel.color.b, deleteProgressPanel.color.a + (Time.unscaledDeltaTime * 2));
            deleteProgressText.color = new Color(deleteProgressText.color.r, deleteProgressText.color.g, deleteProgressText.color.b, deleteProgressText.color.a + (Time.unscaledDeltaTime * 3f));
            yield return null;
        }
    }

    /// <summary>
    /// Fade out the Deleting in Progress UI
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOutProgressUI()
    {
        // While alpha values are over the desired numbers, decrease them by an unscaled delta time (because we are paused)
        while (deleteProgressPanel.color.a > 0 && deleteProgressText.color.a > 0)
        {
            deleteProgressPanel.color = new Color(deleteProgressPanel.color.r, deleteProgressPanel.color.g, deleteProgressPanel.color.b, deleteProgressPanel.color.a - (Time.unscaledDeltaTime * 2));
            deleteProgressText.color = new Color(deleteProgressText.color.r, deleteProgressText.color.g, deleteProgressText.color.b, deleteProgressText.color.a - (Time.unscaledDeltaTime * 3f));
        }

        // Disable the UI elements once they are gone
        deleteProgressPanel.enabled = false;
        deleteProgressText.enabled = false;

        yield return null;
    }
}
