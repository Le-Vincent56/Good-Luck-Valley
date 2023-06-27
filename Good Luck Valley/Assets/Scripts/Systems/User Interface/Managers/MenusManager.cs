using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Linq;

public class MenusManager : MonoBehaviour
{
    #region REFERENCES
    private GameObject confirmationCheck;
    private GameObject deleteConfirmation;
    private GameObject confirmationCheck2;
    private GameObject startButton;
    private GameObject deleteButton;
    private SpriteRenderer fadeSquare;
    private static GameObject[] navButtons;
    private static GameObject[] textInputs;
    private static Slider[] sliders;
    private static Dropdown resDropdown;
    private static Settings settings;
    private static Toggle fullscreenToggle;
    private Button newGameButton;
    private Toggle subtitlesToggle;
    private Button continueButton;
    private Button loadGameButton;
    private Button settingsButton;
    private Button creditsButton;
    private Button exitGameButton;
    private PauseMenu pauseMenu;
    #endregion

    #region FIELDS
    private bool checkQuit;
    [SerializeField] static int previousScene;
    [SerializeField] static int currentScene;
    private int selectedSave;
    private Color startColor;
    private Color deleteColor;
    private bool fadeIn;
    private bool fadeOut;
    private int sceneLoadNum;
    private static bool checkButtons;
    private static bool disableCalls;
    private static bool[] accessibilityTools;
    private static bool isFullscreen = true;
    private static float brightness;
    private static Vector2 resValues;
    private static bool subtitlesEnabled;
    private static bool settingsSaved;
    #endregion

    #region PROPERTIES
    public int CurrentScene { get { return currentScene; } }
    public bool SettingsSaved { get { return settingsSaved; } set { settingsSaved = value; } } 
    #endregion

    public void Awake()
    {
        // Check which scene is loaded
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            #region RETRIEVE BUTTONS
            newGameButton = GameObject.Find("New Game").GetComponent<Button>();
            continueButton = GameObject.Find("Continue").GetComponent<Button>();
            loadGameButton = GameObject.Find("Load Game").GetComponent<Button>();
            settingsButton = GameObject.Find("Settings").GetComponent<Button>();
            creditsButton = GameObject.Find("Credits").GetComponent<Button>();
            exitGameButton = GameObject.Find("Exit Game").GetComponent<Button>();
            #endregion

            DisableButtonsDependingOnData();
        }

        // Get the current scene
        currentScene = SceneManager.GetActiveScene().buildIndex;

        // Get the settings reference
        settings = GameObject.Find("MenusManager").GetComponent<Settings>();

        // Initial value used to disable exit check
        settingsSaved = true;

        // Fades between scenes
        #region FADING BETWEEN SCENES
        // Get reference to the square used for fading
        fadeSquare = GameObject.Find("Fade").GetComponent<SpriteRenderer>();

        // Set fading in to true so that the square will turn transparent
        fadeIn = true;
        // Set fading out to false so that the square won't turn transparent
        fadeOut = false;
        // Set the initial values of the square's color, black with full transparency
        fadeSquare.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        if (currentScene == 0)
        {
            // If the scene is 0, title screen, then set fade in to false
            // cuz we shouldn't fade in when loading the title screen
            fadeIn = false;
        }
        #endregion

        // If both fade ins are false then set the fade square to have the brightness value for transparency
        if (fadeIn == false && fadeOut == false)
        {
            fadeSquare.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, settings.Brightness);
        }

        #region CONFIRMATION CHECKS
        // Check if the scene is one that contains a confirmation check
        if (currentScene == 1 || currentScene == 2 || currentScene == 4 || currentScene > 5)
        {
            // Find confirmation check in scene
            confirmationCheck = GameObject.Find("ConfirmationCheck");
            // If so, set it to be inactive
            confirmationCheck.SetActive(false);

            // Needs to be set to true for the confirmation checks to pop up
            checkQuit = true;
        }

        // Check if the scene is one that contains a second confirmation check
        if (currentScene == 2 || currentScene == 4 || currentScene > 5)
        {
            // Find second confirmation check
            deleteConfirmation = GameObject.Find("Delete Confirmation");
            // If so, set it to be inactive
            deleteConfirmation.SetActive(false);

            // Needs to be set to true for the confirmation checks to pop up
            checkQuit = true;
        }
        #endregion

        #region SAVE FILES SCENE
        // Check if the scene is the SaveFiles scene
        if (currentScene == 2)
        {
            // Start Button
            // Find start button in scene
            startButton = GameObject.Find("Start");
            // Save the color to a variable
            startColor = startButton.GetComponent<Image>().color;
            // Set to to not be interactable because no save is selected yet
            startButton.GetComponent<Button>().interactable = false;
            // Set it to be half transparency
            startButton.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, 0.5f);

            if (previousScene == 1) 
            {
                startButton.GetComponentInChildren<Text>().text = "Start";
            }

            // Delete Button
            // Find delete button
            deleteButton = GameObject.Find("Delete");
            // Save the color to a variable
            deleteColor = deleteButton.GetComponent<Image>().color;
            // Set it to be non-interactable cuz no save is selected
            deleteButton.GetComponent<Button>().interactable = false;
            // Set its transparency to be 50%
            deleteButton.GetComponent<Image>().color = new Color(deleteColor.r, deleteColor.g, deleteColor.b, 0.5f);
        }
        #endregion

        // Loading things for settings scene
        #region SETTINGS SCENE
        if (currentScene == 4 | currentScene > 5)
        { 
            // Make on value change events not happen
            disableCalls = true;

            // Initialize arrays for holding input components
            navButtons = new GameObject[4];
            sliders = new Slider[6];
            textInputs = new GameObject[6];

            // Intiialize array for holding values of accessibility tool toggles
            accessibilityTools = new bool[5];

            // Get references to singular input fields
            resDropdown = GameObject.Find("Dropdown").GetComponent<Dropdown>();
            fullscreenToggle = GameObject.Find("FullscreenToggle").GetComponent<Toggle>();
            subtitlesToggle = GameObject.Find("SubtitlesToggle").GetComponent<Toggle>();

            // Call load functions to assign local variables and ensure UI match values
            LoadAudio();

            LoadDisplay();

            LoadAccessibility();

            // Get references to navigation buttons and fill array
            for (int i = 0; i < 4; i++)
            {
                navButtons[i] = GameObject.Find("Button" + i);
            }

            // Button color values for future reference:
            // Normal: FFFFFF
            // Highlighted: 808080
            // Pressed: 9A9A9A
            // Selected: 808080
            // Disabled: A6A6A6

            // Default screen nav button, currently set to be accessibility panel
            navButtons[3].GetComponent<Button>().interactable = false;

            // Allow on value changed events to trigger again
            disableCalls = false;
        }
        #endregion
    }

    public void Update()
    {
        // Call Fading functions
        FadeIn();
        FadeOut();

        #region SAVE FILES SCENE HANDLING

        // Check if the current scene is 2, save files scene
        if (currentScene == 2)
        {
            // Check if a save has been selected
            if (selectedSave != 0)
            {
                // Make delete button interactable and have full transparency
                deleteButton.GetComponent<Image>().color = new Color(deleteColor.r, deleteColor.g, deleteColor.b, 1f);
                deleteButton.GetComponent<Button>().interactable = true;

                // Make save button interactable and have full transparency
                startButton.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, 1f);
                startButton.GetComponent<Button>().interactable = true;

                // Check if either confirmation check is active
                if (confirmationCheck.activeSelf == true || deleteConfirmation.activeSelf == true)
                {
                    // If so, make save button half transparency and non-interactable
                    startButton.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, 0.5f);
                    startButton.GetComponent<Button>().interactable = false;

                    // Make delete button half transparency and non-interactable
                    deleteButton.GetComponent<Image>().color = new Color(deleteColor.r, deleteColor.g, deleteColor.b, 0.5f);
                    deleteButton.GetComponent<Button>().interactable = false;
                }
            }
        }
        #endregion

        #region SETTINGS SCENE HANDLING
        // Check if scene is settings, 4
        if (currentScene == 4 || currentScene > 5)
        {
            // Check if we should update the navigation buttons visuals
            if (checkButtons)
            {
                // Loop through navButtons 
                for (int i = 0; i < navButtons.Length; i++)
                {
                    // Check if the button isn't interactable
                    if (navButtons[i].GetComponent<Button>().IsInteractable() == false)
                    {
                        // If so, then set it's panel to be at the bottom of the
                        //  hierarchy so that it will appear above everything else
                        navButtons[i].transform.parent.SetAsLastSibling();
                    }
                }
                
                // Disable navButton checks
                checkButtons = false;
            }
        }
        #endregion
    }

    #region VALUE ASSIGNING/LOADING HELPER METHODS
    private void LoadAudio()
    {
        // Get references to all sliders and text inputs
        for (int i = 0; i < 6; i++)
        {
            textInputs[i] = GameObject.Find("TextInput" + i);
            sliders[i] = GameObject.Find("Slider" + i).GetComponent<Slider>();
        }

        // Assign values for AUDIO settings input fields using values from settings
        textInputs[0].GetComponent<TMP_InputField>().text = settings.MasterVolume.ToString();
        textInputs[1].GetComponent<TMP_InputField>().text = settings.MusicVolume.ToString();
        textInputs[2].GetComponent<TMP_InputField>().text = settings.SFXVolume.ToString();
        textInputs[3].GetComponent<TMP_InputField>().text = settings.AmbientVolume.ToString();
        textInputs[4].GetComponent<TMP_InputField>().text = settings.VoicesVolume.ToString();

        // Assign values for AUDIO settings sliders using values from settings
        sliders[0].value = settings.MasterVolume;
        sliders[1].value = settings.MusicVolume;
        sliders[2].value = settings.SFXVolume;
        sliders[3].value = settings.AmbientVolume;
        sliders[4].value = settings.VoicesVolume;
    }

    private void LoadDisplay()
    {
        // Assigning values for DISPLAY settings based on values from settings
        brightness = settings.Brightness;
        resValues = settings.Resolution;
        resDropdown.value = settings.ResOption;
        fullscreenToggle.isOn = settings.IsFullscreen;
        subtitlesToggle.isOn = settings.SubtitlesEnabled;
        sliders[5].value = settings.Brightness;
        textInputs[5].GetComponent<TMP_InputField>().text = settings.Brightness.ToString();
    }

    private void LoadAccessibility()
    {
        // Assign values for ACCESSIBILITY settings using the values from settings
        accessibilityTools[0] = settings.ThrowIndicatorShown;
        accessibilityTools[1] = settings.InfiniteShroomsOn;
        accessibilityTools[2] = settings.ShroomDurationOn;
        accessibilityTools[3] = settings.InstantThrowOn;
        accessibilityTools[4] = settings.NoClipOn;

        // Iterate through accessibility toggles and assign values based on accessibilityTools
        for (int i = 0; i < 5; i++)
        {
            GameObject.Find("Toggle" + i).GetComponent<Toggle>().isOn = accessibilityTools[i];
        }
    }
    #endregion


    #region NAVIGATING SCENES
    /// <summary>
    /// Navigates to the main menu scene
    /// </summary>
    public void NavMainMenu()
    {
        // Switch scenes to the Tutorial Tilemap
        previousScene = currentScene;
        SceneManager.LoadScene("Main Menu");
    }

    /// <summary>
    /// Navigates to the save files scene
    /// </summary>
    public void NavSaveFiles()
    {
        previousScene = currentScene;
        SceneManager.LoadScene("SaveFiles");
    }

    /// <summary>
    /// Navigates to the settings scene
    /// </summary>
    public void NavSettings()
    {
        previousScene = currentScene;
        SceneManager.LoadScene("Settings");
    }

    /// <summary>
    /// Navigates to the journal scene
    /// </summary>
    public void NavJournal()
    {
        previousScene = currentScene;
        SceneManager.LoadScene("Journal");
    }

    /// <summary>
    /// Navigates to the credits scene
    /// </summary>
    public void NavCredits()
    {
        previousScene = currentScene;
        SceneManager.LoadScene("Credits");
    }
    #endregion

    #region CONFIRMATION CHECKS
    /// <summary>
    /// Confirms what the user wants to do, changes functionality depending on the scene it is being called in
    /// </summary>
    /// <param name="confirmCheckNum"> The number associated with the confirmation check box that is being called (input in inspector)</param>
    public void ConfirmationCheck(int confirmCheckNum)
    {
        if (checkQuit)
        {
            //if (previousScene == 1)
            //{
            //    Debug.Log("Loading Save");
            //}
            // Check if we are opening the first confirmation check and that the other one is either
            //  null or not active (only one should be on screen at a time)
            if (confirmCheckNum == 1 && (confirmationCheck2 == null || confirmationCheck2.activeSelf == false))
            {
                // Set the confirmation check box to be active
                confirmationCheck.SetActive(true);
            }
            // Check if we are opening the second confirmation check, and that the other one is either 
            //  null or not active (only one should be on screen at a time)
            else if (confirmCheckNum == 2 && (confirmationCheck == null || confirmationCheck.activeSelf == false))
            {
                // Set the confirmation check to be active
                deleteConfirmation.SetActive(true);
            }
            // Disable check quit so that the next time the function is called it uses the else statement
            checkQuit = false;
        }
        // Only happens if the user presses 'yes' on the confirmation box
        else
        {
            // Switch statement based on current scene
            switch (currentScene)
            {
                // If the scene is the title screen then the confirmation box should close the game
                case 1:
                    Application.Quit(); // Closes application
                    break;

                // If the scene is the save files scene
                case 2:
                    if (confirmCheckNum == 1)
                    {
                        // Overwrites the save

                        // Enables checkQuit so that the confirmation box appears
                        //  the next time it needs to
                        checkQuit = true;

                        // Hides the confirmation box
                        confirmationCheck.SetActive(false);
                    }
                    else if (confirmCheckNum == 2)
                    {
                        // Deletes save 

                        // Enables checkQuit so that the confirmation box appears
                        //  the next time it needs to
                        checkQuit = true;

                        // Hides the confirmation box
                        deleteConfirmation.SetActive(false);
                    }
                    break;

                // If the scene is settings scene
                case 4:
                    if (confirmCheckNum == 1)
                    {
                        // Sets settingsSaved to true so that we can exit
                        //  without the confirmation box appearing
                        settingsSaved = true;

                        // Calls back function to return to previous scene
                        Back();
                    }
                    else if (confirmCheckNum == 2)
                    {
                        // Sets the confirmation box to inactive
                        deleteConfirmation.SetActive(false);

                        // Enables checkQuit so that the confirmation box appears
                        //  the next time it needs to
                        checkQuit = true;

                        // Calls reset settings to reset to defaults
                        ResetSettings();
                    }
                    break;

                case 5:
                    break;
            }
        }
    }

    /// <summary>
    /// Cancels the confirmation check, 'NO' was pressed
    /// </summary>
    /// <param name="confirmCheckNum"> The number associated with the confirmation check box that is being cancelled (input in inspector)</param>
    public void Cancel(int confirmCheckNum)
    {
        if (!checkQuit)
        {
            checkQuit = true;
            if (confirmCheckNum == 1)
            {
                confirmationCheck.SetActive(false);
            }
            else if (confirmCheckNum == 2)
            {
                deleteConfirmation.SetActive(false);
            }
        }
    }
    #endregion

    public void Back()
    {
        if (settingsSaved)
        {
            fadeOut = true;
            sceneLoadNum = previousScene;
        }
        else if (currentScene == 4)
        {
            ConfirmationCheck(1);
        }
    }

    public void SelectSave(int saveNum)
    {
        selectedSave = saveNum;
    }

    #region FADING
    private void FadeIn()
    {
        if (fadeIn)
        {
            fadeSquare.color = new Color(0, 0, 0, fadeSquare.color.a - 0.05f);
            if (fadeSquare.color.a <= settings.Brightness)
            {
                fadeIn = false;
            }
        }
    }

    public void OnNewGameClicked()
    {
        // Disable all other buttons to prevent accidental clicking
        DisableAllButtons();

        // Create a new GameData class for the new game
        DataManager.Instance.NewGame();

        // Save the game before loading a new scene
        DataManager.Instance.SaveGame();

        // Load the gameplay scene - this will also save the game because of DataManager.OnSceneUnloaded()
        CheckFade(6);
    }

    public void OnContinueClicked()
    {
        // Disable all other buttons to prevent accidental clicking
        DisableAllButtons();

        // Save the game before loading a new scene
        DataManager.Instance.SaveGame();

        // Load the most recent game
        SceneManager.LoadSceneAsync(DataManager.Instance.Level);
    }

    private void FadeOut()
    {
        fadeOut = false;
        if (fadeOut)
        { 
            fadeSquare.color = new Color(0, 0, 0, fadeSquare.color.a + 0.05f);
            if (fadeSquare.color.a >= 1)
            { 
                fadeOut = false;
            }
        }
        else
        {
            if (sceneLoadNum == 1)
            {
                NavMainMenu();
            } 
            else if (sceneLoadNum == 2)
            {
                NavSaveFiles();
            }
            else if (sceneLoadNum == 3)
            {
                NavJournal();
            }
            else if (sceneLoadNum == 4)
            {
                NavSettings();
            }
            else if (sceneLoadNum == 5)
            {
                NavCredits();
            }
            else if (sceneLoadNum > 5)
            {
                previousScene = currentScene;
                Time.timeScale = 1f;
                // Load the most recent game
                SceneManager.LoadSceneAsync(DataManager.Instance.Level);
            }
        }
    }

    public void CheckFade(int sceneToLoad)
    {
        if (settingsSaved)
        {
            fadeOut = true;
            sceneLoadNum = sceneToLoad;
        }
    }
    #endregion

    #region SETTINGS INPUTS
    public void ChangeResolution()
    {
        if (!disableCalls)
        {
            string[] resolution;
            resolution = resDropdown.options[resDropdown.value].text.Split('x');
            int[] resolutionValues = new int[resolution.Length];
            int.TryParse(resolution[0], out resolutionValues[0]);
            int.TryParse(resolution[1], out resolutionValues[1]);

            resValues = new Vector2(resolutionValues[0], resolutionValues[1]);
            settingsSaved = false;
        }
    }    

    public void SetFullscreen() 
    {
        if (!disableCalls)
        {
            isFullscreen = fullscreenToggle.isOn;
            settingsSaved = false;
        }
    }

    public void ChangeBrightness(int type)
    {
        if (!disableCalls)
        {
            switch (type)
            {
                case 0:
                    brightness = int.Parse(textInputs[5].GetComponent<TMP_InputField>().text);
                    if (brightness < .95f)
                    {
                        fadeSquare.color = new Color(0, 0, 0, 1 - (brightness / 100f));
                        settings.Brightness = brightness;
                    }
                    break;

                case 1:
                    brightness = sliders[5].value;
                    if (brightness < .95f)
                    {
                        fadeSquare.color = new Color(0, 0, 0, 1 - (brightness / 100f));
                        settings.Brightness = brightness;
                    }
                    break;
            }
            settingsSaved = false;
        }
    }

    public void SetButton(int button)
    {
        checkButtons = true;
        for (int i = 0; i < navButtons.Length; i++)
        {
            navButtons[i].GetComponent<Button>().interactable = true;
        }
        navButtons[button].GetComponent<Button>().interactable = false;
    }

    public void AdjustSlider(int index)
    {

        if (textInputs.Length > 0 && sliders.Length > 0 && int.TryParse(textInputs[index].GetComponent<TMP_InputField>().text, out int result))
        {
            sliders[index].value = result;
        }
    }

    public void AdjustInputField(int index)
    {
        if (textInputs.Length > 0 && sliders.Length > 0)
        {
            textInputs[index].GetComponent<TMP_InputField>().text = sliders[index].value.ToString();
        }
    }

    public void ToggleAccessibilityTool(int index)
    {
        if (!disableCalls)
        {
            accessibilityTools[index] = !accessibilityTools[index];
            settingsSaved = false;
        }
    }

    public void ToggleSubtitles()
    {
        if (!disableCalls)
        {
            subtitlesEnabled = subtitlesToggle.isOn;
            settingsSaved = false;
        }
    }
    #endregion

    #region APPLYING SETTINGS HELPER METHODS
    private void ApplyAccessibility()
    {
        settings.ThrowIndicatorShown = accessibilityTools[0];
        settings.InfiniteShroomsOn = accessibilityTools[1];
        settings.ShroomDurationOn = accessibilityTools[2];
        settings.InstantThrowOn = accessibilityTools[3];
        settings.NoClipOn = accessibilityTools[4];
    }

    private void ApplyDisplay()
    {
        settings.Brightness = brightness;
        if (resValues.x != 12 && resValues.y != 34)
        {
            Screen.SetResolution((int)resValues.x, (int)resValues.y, isFullscreen);
        }
        else
        {
            Screen.SetResolution(1920, 1080, true);
        }
        Screen.fullScreen = isFullscreen;
        settings.SubtitlesEnabled = subtitlesEnabled;
        settings.ResOption = resDropdown.value;
        settings.IsFullscreen = fullscreenToggle.isOn;
        settings.Resolution = resValues;

    }

    private void ApplyAudio()
    {
        settings.MasterVolume = sliders[0].value;
        settings.MusicVolume = sliders[1].value;
        settings.SFXVolume = sliders[2].value;
        settings.AmbientVolume = sliders[3].value;
        settings.VoicesVolume = sliders[4].value;
    }
    #endregion

    /// <summary>
    /// Updates the values in the settings script and disables the 
    ///     'leave without saving' confirmation check
    /// </summary>
    public void ApplySettings()
    {
        // Bool in Settings.cs that lets it know if it should update the
        //  game state based on enabled settings
        settings.UpdateSettings = true;

        // Accessibility settings
        ApplyAccessibility();

        // Display settings
        ApplyDisplay();

        // Audio settings
        ApplyAudio();

        // Save the game
        DataManager.Instance.SaveGame();

        // Disables confirmation check
        settingsSaved = true;
    }

    /// <summary>
    /// Resets settings to their default values
    /// </summary>
    private void ResetSettings()
    {
        // Makes it so that on-change functions dont happen
        // when changing toggle values
        disableCalls = true;

        accessibilityTools[0] = true;  // Throw line is off
        accessibilityTools[1] = false;  // Mushrooms are limited to 3
        accessibilityTools[2] = true;   // Mushroom timer is enabled
        accessibilityTools[3] = false;  // Instant throw is disabled
        accessibilityTools[4] = false;  // No-Clip is disabled

        // Changing menu display
        fullscreenToggle.isOn = true;   // Sets fullscreen toggle to be on
        brightness = 95;                // Brightness is set to 95%
        resValues.x = 1920;             // Resolution is set to 1920x1080
        resValues.y = 1080;
        isFullscreen = true;            // Fullscreen is enabled
        subtitlesEnabled = false;       // Subtitles are disabled
        resDropdown.value = 1;          // Resolution dropdown setting is set to show
                                        // the proper current resolution

        // Sets accessibility page toggles to be accurate
        for (int i = 0; i < 5; i++)
        {
            GameObject.Find("Toggle" + i).GetComponent<Toggle>().isOn = accessibilityTools[i];
        }

        //// Makes sliders and text inputs be accurate
        //for (int i = 0; i < 5; i++)
        //{
        //    textInputs[i] = GameObject.Find("TextInput" + i);
        //    sliders[i] = GameObject.Find("Slider" + i).GetComponent<Slider>();
        //}

        textInputs[5].GetComponent<TMP_InputField>().text = brightness.ToString();
        sliders[5].value = brightness;

        sliders[0].value = 40f; // Master volume
        sliders[1].value = 30f; // Music volume
        sliders[2].value = 30f; // SFX volume
        sliders[3].value = 30f; // Ambient volume
        sliders[4].value = 40f; // Voices volume

        // Calls apply settings
        ApplySettings();

        disableCalls = false;
    }

    /// <summary>
    /// Disable all buttons
    /// </summary>
    public void DisableAllButtons()
    {
        continueButton.interactable = false;
        loadGameButton.interactable = false;
        settingsButton.interactable = false;
        creditsButton.interactable = false;
        exitGameButton.interactable = false;
    }

    public void DisableButtonsDependingOnData()
    {
        // Check if DataManager has data
        if (!DataManager.Instance.HasGameData())
        {
            // If there is no data, set the menu to reflect a new game
            newGameButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
            loadGameButton.interactable = false;
        }
        else
        {
            // If there is data, set the menu to reflect a continued game
            newGameButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
            loadGameButton.interactable = true;
        }
    }
}
