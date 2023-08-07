using HiveMind.Audio;
using HiveMind.SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HiveMind.Menus
{
    public class SettingsManager : MonoBehaviour
    {
        #region REFERENCES
        private GameObject confirmationCheck;
        private GameObject confirmationCheck2;
        private static GameObject[] navButtons;
        private static GameObject[] textInputs;
        private static Slider[] sliders;
        private static Dropdown resDropdown;
        private static Toggle fullscreenToggle;
        private static Settings settings;
        private Toggle subtitlesToggle;
        private GameObject brightnessSquare;
        #endregion

        #region FIELDS
        private bool checkQuit;
        [SerializeField] static int previousScene;
        [SerializeField] static int currentScene;
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
            // Initialize arrays for holding input components
            navButtons = new GameObject[4];
            sliders = new Slider[6];
            textInputs = new GameObject[6];

            // Intiialize array for holding values of accessibility tool toggles
            accessibilityTools = new bool[5];
        }


        // Start is called before the first frame update
        void Start()
        {
            // Get the current scene
            currentScene = SceneManager.GetActiveScene().buildIndex;

            // Get the settings reference
            settings = GameObject.Find("MenusManager").GetComponent<Settings>();

            brightnessSquare = GameObject.Find("Fade");

            // Initial value used to disable exit check
            settingsSaved = true;

            // Find confirmation check in scene
            confirmationCheck = GameObject.Find("ConfirmationCheck");
            // If so, set it to be inactive
            if (confirmationCheck != null)
            {
                confirmationCheck.SetActive(false);
            }

            // Find second confirmation check
            confirmationCheck2 = GameObject.Find("Delete Confirmation");
            // If so, set it to be inactive
            if (confirmationCheck2)
            {
                confirmationCheck2.SetActive(false);
            }

            // Needs to be set to true for the confirmation checks to pop up
            checkQuit = true;

            // Assigning references

            // Make on value change events not happen
            disableCalls = true;

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

            // Assign proper values from settings
            UpdateSettings();

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

        // Update is called once per frame
        void Update()
        {

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

        }

        #region ASSIGNING REFERENCES HELPERS
        private void LoadAudio()
        {
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

        private void UpdateSettings()
        {
            // Call load functions to assign local variables and ensure UI match values
            LoadAudio();

            LoadDisplay();

            LoadAccessibility();
        }
        #endregion

        #region CONFIRMATION CHECKS

        public void OpenConfirmationCheck(int confirmCheckNum)
        {
            // Play sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);

            // If we should open the confirmation box
            if (checkQuit)
            {
                // Checks which confirmation box we are opening
                switch (confirmCheckNum)
                {
                    case 0:
                        // Checks if the other confirmation box isn't active or is null
                        if (confirmationCheck2.activeSelf == false || confirmationCheck2 == null)
                        {
                            // Sets this confirmation box to active
                            confirmationCheck.SetActive(true);
                        }
                        break;

                    case 1:
                        // Checks if the other confirmation box isn't active or is null
                        if (confirmationCheck.activeSelf == false || confirmationCheck == null)
                        {
                            // Sets this confirmation box to active
                            confirmationCheck2.SetActive(true);
                        }
                        break;
                }

                // We should not open a confirmation box next time
                checkQuit = false;
            }
        }

        public void SaveSettings()
        {
            // Sets settingsSaved to true so that we can exit
            //  without the confirmation box appearing
            settingsSaved = true;
        }

        public void ConfirmResetSettings()
        {
            // Sets the confirmation box to inactive
            confirmationCheck2.SetActive(false);

            // Enables checkQuit so that the confirmation box appears
            //  the next time it needs to
            checkQuit = true;

            // Calls reset settings to reset to defaults
            ResetSettings();

            // Play sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
        }

        public void Back(PauseMenu pause)
        {
            if (settingsSaved)
            {
                // Close settings screen
                pause.CloseSettings();

                // Hide confirmation box
                confirmationCheck.SetActive(false);

                // Disable value changed calls
                disableCalls = true;

                // Update settings visuals
                UpdateSettings();

                // Enable value changed calls
                disableCalls = false;

                // Set checks to true
                checkButtons = true;
            }
            else
            {
                ConfirmationCheck(1);
            }

            // Play sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
        }

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
                    confirmationCheck2.SetActive(true);
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
            // Only true if a confirmation box is open
            if (!checkQuit)
            {
                // Sets check quit to true so we know to open a confirmation box next time
                checkQuit = true;

                // Disables the correspoonding confirmation box
                if (confirmCheckNum == 1)
                {
                    confirmationCheck.SetActive(false);
                }
                else if (confirmCheckNum == 2)
                {
                    confirmationCheck2.SetActive(false);
                }

                // Play sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
            }
        }
        #endregion

        #region SETTINGS INPUTS

        /// <summary>
        /// Changes the resolution when the player selects a different resolution from the dropdown menu
        /// </summary>
        public void ChangeResolution()
        {
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

                // Play sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICheckmark, transform.position);
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
                // Sets whether it the game is fullscreen using the toggle
                isFullscreen = fullscreenToggle.isOn;

                // Sets settings saved to false since a setting has been changed
                settingsSaved = false;

                // Play sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICheckmark, transform.position);
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

                // Play sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UITab, transform.position);
            }
        }

        /// <summary>
        /// Adjuts a slider based on the text input value
        /// </summary>
        /// <param name="index"> The index of the slider/textInput pair we are changing </param>
        public void AdjustSlider(int index)
        {
            // If text inputs isnt null and we havent disabled calls
            if (!disableCalls && textInputs != null)
            {
                // Checks if text inputs and sliders arent empty and that we can parse the value int he given text inputs field
                if (textInputs.Length > 0 && sliders.Length > 0 && int.TryParse(textInputs[index].GetComponent<TMP_InputField>().text, out int result))
                {
                    // Changes the value at the given index
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
            // If sliders isnt null and we havent disabled calls
            if (!disableCalls && sliders != null)
            {
                // Checks if text inputs and sliders arent empty
                if (textInputs.Length > 0 && sliders.Length > 0)
                {
                    // Changes the value at the given index
                    textInputs[index].GetComponent<TMP_InputField>().text = sliders[index].value.ToString();
                }
            }
        }

        public void AdjustSoundInputField(int index)
        {
            if (textInputs.Length > 0 && sliders.Length > 0)
            {
                textInputs[index].GetComponent<TMP_InputField>().text = sliders[index].value.ToString();
            }

            // Set live changes so they can hear the music
            switch (index)
            {
                case 0:
                    AudioManager.Instance.SetMasterVolume(sliders[index].value / 100f);
                    break;

                case 1:
                    AudioManager.Instance.SetMusicVolume(sliders[index].value / 100f);
                    break;

                case 2:
                    AudioManager.Instance.SetSFXVolume(sliders[index].value / 100f);
                    break;

                case 3:
                    AudioManager.Instance.SetAmbienceVolume(sliders[index].value / 100f);
                    break;
            }
        }

        public void ToggleAccessibilityTool(int index)
        {
            if (!disableCalls)
            {
                accessibilityTools[index] = !accessibilityTools[index];
                settingsSaved = false;

                // Play sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICheckmark, transform.position);
            }
        }

        public void ToggleSubtitles()
        {
            if (!disableCalls)
            {
                subtitlesEnabled = subtitlesToggle.isOn;
                settingsSaved = false;

                // Play sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICheckmark, transform.position);
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

        #region APPLYING AND RESETTING SETTINGS
        /// <summary>
        /// Updates the values in the settings script and disables the 
        ///     'leave without saving' confirmation check
        /// </summary>
        public void ApplySettings()
        {
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

            // Play sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);

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

            // Play sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
        }

        /// <summary>
        /// Play the button sound
        /// </summary>
        public void PlayButtonSound()
        {
            // Play sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
        }
        #endregion
    }
}
