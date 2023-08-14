using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using HiveMind.Audio;
using HiveMind.SaveData;

namespace HiveMind.Menus
{
    public class MenusManager : MonoBehaviour
    {
        #region REFERENCES
        private GameObject confirmationCheck;
        private GameObject deleteConfirmation;
        private GameObject confirmationCheck2;
        private GameObject startButton;
        private GameObject deleteButton;
        private static GameObject[] navButtons;
        private static GameObject[] textInputs;
        private static Slider[] sliders;
        private static Dropdown resDropdown;
        private static Settings settings;
        private static Toggle fullscreenToggle;
        private Toggle subtitlesToggle;
        private Button newGameButton;
        private Button continueButton;
        private Button loadGameButton;
        private Button settingsButton;
        private Button creditsButton;
        private Button exitGameButton;
        private CanvasGroup canvasGroup;
        #endregion

        #region FIELDS
        private bool checkQuit;
        [SerializeField] static int previousScene;
        [SerializeField] static int currentScene;
        private int selectedSave;
        private Color startColor;
        private Color deleteColor;
        private bool fadeIn;
        private int sceneLoadNum;
        private static bool checkButtons;
        private static bool disableCalls;
        private static bool[] accessibilityTools;
        private static bool isFullscreen = true;
        private static float brightness;
        private static Vector2 resValues;
        private static bool subtitlesEnabled;
        private static bool settingsSaved;
        private bool navScenes;
        private bool fadingIn;
        private bool fadingOut;
        private GameObject menuParent;
        #endregion

        #region PROPERTIES
        public int CurrentScene { get { return currentScene; } }
        public bool SettingsSaved { get { return settingsSaved; } set { settingsSaved = value; } }
        #endregion

        public void Start()
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
            if (currentScene == 3)
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

                // Get references to all sliders and text inputs
                for (int i = 0; i < 6; i++)
                {
                    textInputs[i] = GameObject.Find("TextInput" + i);
                    sliders[i] = GameObject.Find("Slider" + i).GetComponent<Slider>();
                }

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

                // Default screen nav button, currently set to be audio panel
                navButtons[0].GetComponent<Button>().interactable = false;
                menuParent = navButtons[0].transform.parent.gameObject;

                // Allow on value changed events to trigger again
                disableCalls = false;
            }
            #endregion

            // If both fade ins are false then set the fade square to have the brightness value for transparency
            if (fadeIn == false)
            {
                //Dont fade
            }
            else if (currentScene <= 5)
            {
                StartCoroutine(FadeIn());
            }
        }

        public void Update()
        {
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
            if (currentScene == 3)
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

            if (navScenes)
            {
                NavigateToScene();
            }
        }

        public void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            // Get the current scene
            currentScene = SceneManager.GetActiveScene().buildIndex;

            // Get the settings reference
            settings = GameObject.Find("MenusManager").GetComponent<Settings>();

            // Initial value used to disable exit check
            settingsSaved = true;

            // Fades between scenes
            #region FADING BETWEEN SCENES

            // Set fading in to true so that the square will turn transparent
            fadeIn = true;

            if (currentScene <= 5)
            {
                canvasGroup = GameObject.Find("Canvas").GetComponent<CanvasGroup>();

                // Set the initial values of the square's color, black with full transparency
                canvasGroup.alpha = 0f;
            }
            #endregion

            #region CONFIRMATION CHECKS
            // Check if the scene is one that contains a confirmation check
            if (currentScene == 1 || currentScene == 2 || currentScene == 3)
            {
                // Find confirmation check in scene
                confirmationCheck = GameObject.Find("ConfirmationCheck");
                confirmationCheck.SetActive(false);
                // Needs to be set to true for the confirmation checks to pop up
                checkQuit = true;
            }

            // Check if the scene is one that contains a second confirmation check
            if (currentScene == 2 || currentScene == 3)
            {
                // Find second confirmation check
                deleteConfirmation = GameObject.Find("Delete Confirmation");
                deleteConfirmation.SetActive(false);
                // Needs to be set to true for the confirmation checks to pop up
                checkQuit = true;
            }
            #endregion
        }

        #region VALUE ASSIGNING/LOADING HELPER METHODS
        private void LoadAudio()
        {
            Debug.Log("Audio Values: master; " + settings.MasterVolume + ". music; " + settings.MusicVolume + ". sfx; " + settings.SFXVolume + ". ambient; " + settings.AmbientVolume);

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
            SceneManager.LoadScene("Main Menu");
        }

        /// <summary>
        /// Navigates to the save files scene
        /// </summary>
        public void NavSaveFiles()
        {
            SceneManager.LoadScene("SaveFiles");
        }

        /// <summary>
        /// Navigates to the settings scene
        /// </summary>
        public void NavSettings()
        {
            SceneManager.LoadScene("Settings");
        }

        /// <summary>
        /// Navigates to the journal scene
        /// </summary>
        public void NavJournal()
        {
            SceneManager.LoadScene("Journal");
        }

        /// <summary>
        /// Navigates to the credits scene
        /// </summary>
        public void NavCredits()
        {
            SceneManager.LoadScene("Credits");
        }

        /// <summary>
        /// Navigates to the scene set through sceneLoadNum
        /// </summary>
        public void NavigateToScene()
        {
            previousScene = currentScene;
            switch (sceneLoadNum)
            {
                case 0:
                    break;
                case 1:
                    NavMainMenu();
                    break;
                case 2:
                    NavSaveFiles();
                    break;
                case 3:
                    NavSettings();
                    break;
                case 4:
                    NavCredits();
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    SceneManager.LoadScene(DataManager.Instance.Level);
                    break;
            }
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
                // Play the UI button sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);

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
                    case 3:
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
        #endregion

        /// <summary>
        /// Goes back to the previous menu
        /// </summary>
        public void Back()
        {
            // Play the UI button sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);

            if (settingsSaved)
            {

                if (currentScene <= 4)
                    StartCoroutine(FadeOut());
                sceneLoadNum = previousScene;
            }
            else if (currentScene == 3)
            {
                ConfirmationCheck(1);
            }
        }

        /// <summary>
        /// Selects a save
        /// </summary>
        /// <param name="saveNum"></param>
        public void SelectSave(int saveNum)
        {
            selectedSave = saveNum;
        }

        #region FADING
        /// <summary>
        /// Fades in the menu screens
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeIn()
        {
            // The menu panels in the scene (only used for the settings scene)
            List<GameObject> menuPanels = new List<GameObject>();

            // This is done in order to remove the the effect of the settings panels overlapping each other when the menu as a whole is fading out
            // Checks if the current scene is the settings scene
            if (currentScene == 3)
            {
                // Loops through all the gameObject with the tag MenuPanel
                foreach (GameObject g in GameObject.FindGameObjectsWithTag("MenuPanel"))
                {
                    // Adds them to the menuPanels list
                    menuPanels.Add(g);

                    // Sets them to inactive
                    g.SetActive(false);

                    // Checks if this menu is the menu being viewed by the user
                    if (g.transform.parent.gameObject == menuParent)
                    {
                        // If so, sets it to active
                        g.SetActive(true);
                    }
                }
            }

            // checks if the canvas group isn't null
            if (canvasGroup != null)
            {
                // Loops while its alpha is less than one
                while (canvasGroup.alpha < 1)
                {
                    // Increases its alpha
                    canvasGroup.alpha += 0.1f;
                    yield return null;
                }
            }

            // Checks if the current scene is the settings scene
            if (currentScene == 3)
            {
                // Sets all the menu panels to active
                foreach (GameObject g in menuPanels)
                {
                    g.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Fades out the menu screens
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOut()
        {
            // Checks if the scene is settings
            if (currentScene == 3)
            {
                // Loops through all the menu panels in the scene
                foreach (GameObject g in GameObject.FindGameObjectsWithTag("MenuPanel"))
                {
                    // Sets them to inactive
                    g.SetActive(false);

                    // Checks if this menu is the menu being viewed by the user
                    if (g.transform.parent.gameObject == menuParent)
                    {
                        // If so, sets it to active
                        g.SetActive(true);
                    }
                }
            }

            // Checks if the canvas group isn't null
            if (canvasGroup != null)
            {
                // Loops while the alpha is greater than 0
                while (canvasGroup.alpha > 0)
                {
                    // Decreases the alpha
                    canvasGroup.alpha -= 0.1f;
                    yield return null;
                }
                // Sets navScenes to true because this happens right before we switch scenes
                navScenes = true;
            }

            // Checks if the current scene is the settings scene
            //if (currentScene == 3)
            //{
            //    // Sets all menu panels to active
            //    foreach (GameObject g in GameObject.FindGameObjectsWithTag("MenuPanel"))
            //    {
            //        g.SetActive(true);
            //    }
            //}
        }

        /// <summary>
        /// Checks whether we should fade or not
        /// </summary>
        /// <param name="sceneToLoad"></param>
        public void CheckFade(int sceneToLoad)
        {
            if (settingsSaved)
            {
                sceneLoadNum = sceneToLoad;
                StartCoroutine(FadeOut());
            }
        }
        #endregion

        #region GAME BUTTON INPUT

        /// <summary>
        /// New game button clicked
        /// </summary>
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

        /// <summary>
        /// Continue button clicked
        /// </summary>
        public void OnContinueClicked()
        {
            // Disable all other buttons to prevent accidental clicking
            DisableAllButtons();

            // Save the game before loading a new scene
            DataManager.Instance.SaveGame();

            // Load the most recent game
            SceneManager.LoadSceneAsync(DataManager.Instance.Level);
        }
        #endregion

        #region SETTINGS INPUTS
        /// <summary>
        /// Changes the resolution when the player selects a different resolution from the dropdown menu
        /// </summary>
        public void ChangeResolution()
        {
            // Checks if we have disabled button calls
            if (!disableCalls)
            {
                // The resolution value stored in the dropdown menu 
                string[] resolution;

                // Fills the resolution array by splitting the rex valus by 'x' 
                //  This is because we label them as 1920x1080 for the user,
                //  but we need to get the individual values of 1920 and 1080 out of the string by splitting
                resolution = resDropdown.options[resDropdown.value].text.Split('x');

                // An array for storing the int parsed variables of the res strings
                int[] resolutionValues = new int[resolution.Length];

                // Attempts to parse the resolution strings into ints
                int.TryParse(resolution[0], out resolutionValues[0]);
                int.TryParse(resolution[1], out resolutionValues[1]);

                // Saves the values into the resValues variable
                resValues = new Vector2(resolutionValues[0], resolutionValues[1]);

                // Sets settings saved to false since a setting has been changed
                settingsSaved = false;
            }
        }

        /// <summary>
        /// Updates the fullscreen setting
        /// </summary>
        public void SetFullscreen()
        {
            // Checks if we have disabled button calls
            if (!disableCalls)
            {
                // Play the UI button sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICheckmark, transform.position);

                // Sets whether it the game is fullscreen using the toggle
                isFullscreen = fullscreenToggle.isOn;

                // Sets settings saved to false since a setting has been changed
                settingsSaved = false;
            }
        }

        /// <summary>
        /// Updates the brightness setting
        /// </summary>
        /// <param name="type"> Whether the interface is a text box or a slider</param>
        public void ChangeBrightness(int type)
        {
            // Checks if we have disabled button calls
            if (!disableCalls)
            {
                // CHecks the given type
                switch (type)
                {
                    // If the input type is a text input field
                    case 0:
                        // textInputs[5] is the reference to the brightness input field
                        // Parses the value in the input field into the brightness variable
                        brightness = int.Parse(textInputs[5].GetComponent<TMP_InputField>().text);
                        break;

                    // If the input type is a slider
                    case 1:
                        // Saves the slider value into the brightness variable
                        brightness = sliders[5].value;
                        break;
                }

                // Sets settings saved to false since a setting has been changed
                settingsSaved = false;
            }
        }

        /// <summary>
        /// Sets the highlighted menu button to the given button index
        /// </summary>
        /// <param name="button"> The button index that should be highlighted</param>
        public void SetButton(int button)
        {
            // Checks if we have disabled button calls
            if (!disableCalls)
            {
                // Play the UI button sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UITab, transform.position);

                // Sets check buttons to true so that the buttons are actually updated
                checkButtons = true;

                // Loops through the navButtons array
                for (int i = 0; i < navButtons.Length; i++)
                {
                    // Sets each button to interactable
                    navButtons[i].GetComponent<Button>().interactable = true;
                }
                // Sets the button at the given index to non interactable
                navButtons[button].GetComponent<Button>().interactable = false;

                // Sets the menu parent to be the given button, which will put that button's respective menu above the others
                menuParent = navButtons[button].transform.parent.gameObject;
            }
        }

        /// <summary>
        /// Adjuts a slider based on the text input value
        /// </summary>
        /// <param name="index"> The index of the slider/textInput pair we are changing </param>
        public void AdjustSlider(int index)
        {
            if (!disableCalls)
            {
                // Checks if the text inputs and sliders arrays are not empty and that the value in the textInput can be parsed into an int
                if (textInputs.Length > 0 && sliders.Length > 0 && int.TryParse(textInputs[index].GetComponent<TMP_InputField>().text, out int result))
                {
                    // Changes the value of the slider 
                    sliders[index].value = result;
                }
            }
        }

        /// <summary>
        /// Adjusts an input field based on the slider value
        /// </summary>
        /// <param name="index"> The index of the slider/textInput pair we are changing </param>
        public void AdjustInputField(int index)
        {
            if (!disableCalls)
            {
                // If the text inputs and sliders arrays arent empty
                if (textInputs.Length > 0 && sliders.Length > 0)
                {
                    // Sets the textInput text value to the slider's value
                    textInputs[index].GetComponent<TMP_InputField>().text = sliders[index].value.ToString();
                }
            }
        }

        /// <summary>
        /// Adjusts a sound input field
        /// </summary>
        /// <param name="index"></param>
        public void AdjustSoundInputField(int index)
        {
            if (!disableCalls)
            {
                // Checks if the text inputs and sliders array isnt empty
                if (textInputs.Length > 0 && sliders.Length > 0)
                {
                    // Sets the text value to the sliders value
                    textInputs[index].GetComponent<TMP_InputField>().text = sliders[index].value.ToString();
                }

                // Set live changes so they can hear the music :D
                switch (index)
                {
                    case 0:
                        AudioManager.Instance.SetMasterVolume(sliders[index].value / 100f);

                        UpdateAudioSettings();
                        break;

                    case 1:
                        AudioManager.Instance.SetMusicVolume(sliders[index].value / 100f);

                        UpdateAudioSettings();
                        break;

                    case 2:
                        AudioManager.Instance.SetSFXVolume(sliders[index].value / 100f);

                        UpdateAudioSettings();
                        break;

                    case 3:
                        AudioManager.Instance.SetAmbienceVolume(sliders[index].value / 100f);

                        UpdateAudioSettings();
                        break;

                    default:
                        Debug.LogWarning("Audio index was out of bounds");
                        break;
                }
            }
        }

        /// <summary>
        /// Toggles an accessibility tool
        /// </summary>
        /// <param name="index"> The index of the accessibility tool to toggle</param>
        public void ToggleAccessibilityTool(int index)
        {
            // Checks if we have disabled button calls
            if (!disableCalls)
            {
                // Play the UI button sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICheckmark, transform.position);

                // Sets the accessibility tool at the given index to be the opposite of itself
                accessibilityTools[index] = !accessibilityTools[index];

                // Sets settings saved to false since a setting has been changed
                settingsSaved = false;
            }
        }

        /// <summary>
        /// Updates whether the subtitles are on based on the subtitles toggle input
        /// </summary>
        public void ToggleSubtitles()
        {
            // Checks if we have disabled button calls
            if (!disableCalls)
            {
                // Updates the subtitlesEnabled value based on the subtitles toggle 
                subtitlesEnabled = subtitlesToggle.isOn;

                // Sets settings saved to false since a setting has been changed
                settingsSaved = false;
            }
        }
        #endregion

        #region APPLYING SETTINGS HELPER METHODS
        /// <summary>
        /// Updates the accessibility settings in the settings to the ones changed in the menu
        /// </summary>
        private void ApplyAccessibility()
        {
            settings.ThrowIndicatorShown = accessibilityTools[0];
            settings.InfiniteShroomsOn = accessibilityTools[1];
            settings.ShroomDurationOn = accessibilityTools[2];
            settings.InstantThrowOn = accessibilityTools[3];
            settings.NoClipOn = accessibilityTools[4];
        }

        /// <summary>
        /// Updates the display settings in the settings to the ones changed in the menu
        /// </summary>
        private void ApplyDisplay()
        {
            settings.Brightness = brightness;
            if (resValues.x != 12 && resValues.y != 34)
            {
                Screen.SetResolution((int)resValues.x, (int)resValues.y, settings.IsFullscreen);
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

        /// <summary>
        ///  Updates the audio settings in the settings to the ones changed in the menu
        /// </summary>
        private void ApplyAudio()
        {
            settings.MasterVolume = sliders[0].value;
            settings.MusicVolume = sliders[1].value;
            settings.SFXVolume = sliders[2].value;
            settings.AmbientVolume = sliders[3].value;
            settings.VoicesVolume = sliders[4].value;
            Debug.Log("Audio Values: master; " + settings.MasterVolume + ". music; " + settings.MusicVolume + ". sfx; " + settings.SFXVolume + ". ambient; " + settings.AmbientVolume);
        }
        #endregion

        #region APPLYING AND RESETTING SETTINGS
        /// <summary>
        /// Updates the values in the settings script and disables the 
        ///     'leave without saving' confirmation check
        /// </summary>
        public void ApplySettings()
        {
            // Play the UI button sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);

            // Bool in Settings.cs that lets it know if it should update the
            //  game state based on enabled settings
            settings.UpdateSettings();

            // Accessibility settings
            ApplyAccessibility();

            // Display settings
            ApplyDisplay();

            // Audio settings
            ApplyAudio();

            // Save the game
            DataManager.Instance.SaveSettings();

            // Disables confirmation check
            settingsSaved = true;
        }

        /// <summary>
        /// Updates the audio settings
        /// </summary>
        public void UpdateAudioSettings()
        {
            ApplyAudio();
            DataManager.Instance.SaveSettings();
        }
            

        /// <summary>
        /// Resets settings to their default values
        /// </summary>
        private void ResetSettings()
        {
            Debug.Log("Reset Settings");
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
        #endregion

        #region DISABLING BUTTONS
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
        #endregion

        /// <summary>
        /// Plays the button sound
        /// </summary>
        public void PlayButtonSound()
        {
            // Play the UI button sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
        }
    }
}
