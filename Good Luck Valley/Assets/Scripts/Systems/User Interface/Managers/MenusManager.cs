using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class MenusManager : MonoBehaviour
{
    #region REFERENCES
    private GameObject confirmationCheck;
    private GameObject confirmationCheck2;
    private GameObject saveButton;
    private GameObject deleteButton;
    private SpriteRenderer fadeSquare;
    private GameObject[] navButtons;
    private GameObject[] textInputs;
    private Slider[] sliders;
    private Dropdown resDropdown;
    private Settings settings;
    private Toggle fullscreenToggle;
    private Toggle subtitlesToggle;
    private Button continueButton;
    private Button loadGameButton;
    private Button settingsButton;
    private Button journalButton;
    private Button creditsButton;
    private Button exitGameButton;
    #endregion

    #region FIELDS
    private bool checkQuit;
    [SerializeField] static int previousScene;
    [SerializeField] static int currentScene;
    private int selectedSave;
    private Color saveColor;
    private Color deleteColor;
    private bool fadeIn;
    private bool fadeOut;
    private int sceneLoadNum;
    private bool checkButtons;
    private bool disableCalls;
    private bool[] accessibilityTools;
    private bool isFullscreen = true;
    private float brightness;
    private Vector2 resValues;
    private bool subtitlesEnabled;
    private bool settingsSaved;
    #endregion

    #region PROPERTIES
    public int CurrentScene { get { return currentScene; } }
    #endregion

    public void Start()
    {
        // Check which scene is loaded
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            #region RETRIEVE BUTTONS
            continueButton = GameObject.Find("Continue").GetComponent<Button>();
            loadGameButton = GameObject.Find("Load Game").GetComponent<Button>();
            settingsButton = GameObject.Find("Settings").GetComponent<Button>();
            journalButton = GameObject.Find("Journal").GetComponent<Button>();
            creditsButton = GameObject.Find("Credits").GetComponent<Button>();
            exitGameButton = GameObject.Find("Exit Game").GetComponent<Button>();
            #endregion

            // Check if DataManager has data
            if (!DataManager.Instance.HasGameData())
            {
                GameObject.Find("Start Text").GetComponent<Text>().text = "New Game";
                loadGameButton.interactable = false;
            } else
            {
                GameObject.Find("Start Text").GetComponent<Text>().text = "Continue";
            }
        }

        

        // Get the current scene
        currentScene = SceneManager.GetActiveScene().buildIndex;

        settings = GameObject.Find("MenusManager").GetComponent<Settings>();
        settingsSaved = true;

        #region FADING BETWEEN SCENES
        if (currentScene != 0)
        {
            fadeSquare = GameObject.Find("Fade").GetComponent<SpriteRenderer>();
            fadeIn = true;
            fadeOut = false;
            fadeSquare.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }
        else if (currentScene == 0)
        {
            fadeSquare = GameObject.Find("Fade").GetComponent<SpriteRenderer>();
            fadeSquare.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            fadeIn = false;
            fadeOut = false;
        }
        #endregion

        if (fadeIn == false && fadeOut == false)
        {
            fadeSquare.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, settings.Brightness);
        }

        #region CONFIRMATION CHECKS
        // Check if the scene is one that contains a confirmation check
        if (currentScene == 1 || currentScene == 2 || currentScene == 4)
        {
            // Find confirmation check in scene
            confirmationCheck = GameObject.Find("ConfirmationCheck");
            // If so, set it to be inactive
            confirmationCheck.SetActive(false);

            // Needs to be set to true for the confirmation checks to pop up
            checkQuit = true;
        }

        // Check if the scene is one that contains a second confirmation check
        if (currentScene == 2 || currentScene == 4)
        {
            // Find second confirmation check
            confirmationCheck2 = GameObject.Find("ConfirmationCheck2");
            // If so, set it to be inactive
            confirmationCheck2.SetActive(false);

            // Needs to be set to true for the confirmation checks to pop up
            checkQuit = true;
        }
        #endregion

        #region SAVE FILES SCENE
        // Check if the scene is the SaveFiles scene
        if (currentScene == 2)
        {
            // Save Button
            // Find save button in scene
            saveButton = GameObject.Find("Save");
            // Save the color to a variable
            saveColor = saveButton.GetComponent<Image>().color;
            // Set to to not be interactable because no save is selected yet
            saveButton.GetComponent<Button>().interactable = false;
            // Set it to be half transparency
            saveButton.GetComponent<Image>().color = new Color(saveColor.r, saveColor.g, saveColor.b, 0.5f);

            if (previousScene == 1) 
            {
                saveButton.GetComponentInChildren<Text>().text = "Start";
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

        #region SETTINGS SCENE
        if (currentScene == 4)
        { 
            disableCalls = true;

            navButtons = new GameObject[4];
            sliders = new Slider[6];
            textInputs = new GameObject[6];
            accessibilityTools = new bool[5];
            resDropdown = GameObject.Find("Dropdown").GetComponent<Dropdown>();
            fullscreenToggle = GameObject.Find("FullscreenToggle").GetComponent<Toggle>();
            subtitlesToggle = GameObject.Find("SubtitlesToggle").GetComponent<Toggle>(); 
            resDropdown.value = settings.ResOption;
            fullscreenToggle.isOn = settings.IsFullscreen;

            for (int i = 0; i < 4; i++)
            {
                navButtons[i] = GameObject.Find("Button" + i);
            }

            // Button Values for future reference:
            // Normal: FFFFFF
            // Highlighted: 808080
            // Pressed: 9A9A9A
            // Selected: 808080
            // Disabled: A6A6A6
            navButtons[3].GetComponent<Button>().interactable = false;

            for (int i = 0; i < 6; i++)
            {
                textInputs[i] = GameObject.Find("TextInput" + i);
                sliders[i] = GameObject.Find("Slider" + i).GetComponent<Slider>();

                // Setting default values, change once we add actual functionality
                textInputs[i].GetComponent<TMP_InputField>().text = settings.Brightness.ToString();
                sliders[i].value = settings.Brightness;
            }

            accessibilityTools[0] = settings.ThrowIndicatorShown;
            accessibilityTools[1] = settings.InfiniteShroomsOn;
            accessibilityTools[2] = settings.ShroomDurationOn;
            accessibilityTools[3] = settings.InstantThrowOn;
            accessibilityTools[4] = settings.NoClipOn; 

            for (int i = 0;i < 5; i++) 
            {
                GameObject.Find("Toggle" + i).GetComponent<Toggle>().isOn = accessibilityTools[i];
            }
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
                saveButton.GetComponent<Image>().color = new Color(saveColor.r, saveColor.g, saveColor.b, 1f);
                saveButton.GetComponent<Button>().interactable = true;

                // Check if either confirmation check is active
                if (confirmationCheck.activeSelf == true || confirmationCheck2.activeSelf == true)
                {
                    // If so, make save button half transparency and non-interactable
                    saveButton.GetComponent<Image>().color = new Color(saveColor.r, saveColor.g, saveColor.b, 0.5f);
                    saveButton.GetComponent<Button>().interactable = false;

                    // Make delete button half transparency and non-interactable
                    deleteButton.GetComponent<Image>().color = new Color(deleteColor.r, deleteColor.g, deleteColor.b, 0.5f);
                    deleteButton.GetComponent<Button>().interactable = false;
                }
            }
        }
        #endregion

        #region SETTINGS SCENE HANDLING
        if (currentScene == 4)
        {
            if (checkButtons)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (navButtons[i].GetComponent<Button>().IsInteractable() == false)
                    {
                        navButtons[i].transform.parent.SetAsLastSibling();
                    }
                }

                checkButtons = false;
            }
        }
        #endregion
    }

    #region NAVIGATING SCENES
    /// <summary>
    /// Navigates to the main menu scene
    /// </summary>
    public void NavMainMenu()
    {
        // Switch scenes to the Tutorial Tilemap
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Main Menu");
    }

    /// <summary>
    /// Navigates to the save files scene
    /// </summary>
    public void NavSaveFiles()
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("SaveFiles");
    }

    /// <summary>
    /// Navigates to the settings scene
    /// </summary>
    public void NavSettings()
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Settings");
    }

    /// <summary>
    /// Navigates to the journal scene
    /// </summary>
    public void NavJournal()
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Journal");
    }

    /// <summary>
    /// Navigates to the credits scene
    /// </summary>
    public void NavCredits()
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
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
            if (confirmCheckNum == 1 && (confirmationCheck2 == null || confirmationCheck2.activeSelf == false))
            {
                confirmationCheck.SetActive(true);
            }
            else if (confirmCheckNum == 2 && (confirmationCheck == null || confirmationCheck.activeSelf == false))
            {
                confirmationCheck2.SetActive(true);
            }
            checkQuit = false;
        }
        else
        {
            switch (currentScene)
            {
                case 1:
                    Debug.Log("Close Game");
                    Application.Quit();
                    break;

                case 2:
                    if (confirmCheckNum == 1)
                    {
                        Debug.Log("Save Overwritten.");
                        checkQuit = true;
                        confirmationCheck.SetActive(false);
                    }
                    else if (confirmCheckNum == 2)
                    {
                        Debug.Log("Save Deleted.");
                        checkQuit = true;
                        confirmationCheck2.SetActive(false);
                    }
                    break;

                case 4:
                    if (confirmCheckNum == 1)
                    {
                        Debug.Log("Returning to " + previousScene);
                        settingsSaved = true;
                        Back();
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
                confirmationCheck2.SetActive(false);
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
        Debug.Log("Save Selected:" + saveNum);
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

    public void OnContinueClicked()
    {
        // Disable all other buttons to prevent accidental clicking
        DisableAllButtons();

        // If there is not already data, create a new game, which will initialize our game data
        if(!DataManager.Instance.HasGameData())
        {
            DataManager.Instance.NewGame();
        }

        // Load the gameplay scene - this will also save the game because of DataManager.OnSceneUnloaded()
        CheckFade(6);
    }

    private void FadeOut()
    {
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
            if (sceneLoadNum == 2)
            {
                NavSaveFiles();
            }
            else if (sceneLoadNum == 1)
            {
                NavMainMenu();
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
            else if (sceneLoadNum == 6)
            {
                SceneManager.LoadScene("Prologue");
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
        string[] resolution = new string[2];
        resolution = resDropdown.options[resDropdown.value].text.Split('x');
        int[] resolutionValues = new int[resolution.Length];
        int.TryParse(resolution[0], out resolutionValues[0]);
        int.TryParse(resolution[1], out resolutionValues[1]);

        resValues = new Vector2(resolutionValues[0], resolutionValues[1]);
        Debug.Log("Settings Not Saved");
        settingsSaved = false;
    }    

    public void SetFullscreen() 
    {
        if (!disableCalls)
        {
            isFullscreen = fullscreenToggle.isOn;
            Debug.Log("Settings Not Saved");
            settingsSaved = false;
        }
    }

    public void ChangeBrightness(int type)
    {
        switch (type)
        {
            case 0:
                brightness = 1 - int.Parse(textInputs[5].GetComponent<TMP_InputField>().text) / 100f;
                if (brightness < .95f)
                {
                    fadeSquare.color = new Color(0, 0, 0, brightness); 
                    settings.Brightness = brightness;
                } 
                break;

            case 1:
                brightness = 1 - sliders[5].value / 100f;
                if (brightness < .95f)
                {
                    fadeSquare.color = new Color(0, 0, 0, brightness);
                    settings.Brightness = brightness;
                }
                break;
        }
        Debug.Log("Settings Not Saved");
        settingsSaved = false;
    }

    public void SetButton(int button)
    {
        checkButtons = true;
        for (int i = 0; i < 4; i++)
        {
            navButtons[i].GetComponent<Button>().interactable = true;
        }
        navButtons[button].GetComponent<Button>().interactable = false;
    }

    public void AdjustSlider(int index)
    {
        if (int.TryParse(textInputs[index].GetComponent<TMP_InputField>().text, out int result))
        {
            sliders[index].value = result;
        }
    }

    public void AdjustInputField(int index)
    {
        textInputs[index].GetComponent<TMP_InputField>().text = sliders[index].value.ToString();
    }

    public void ToggleAccessibilityTool(int index)
    {
        if (!disableCalls)
        {
            accessibilityTools[index] = !accessibilityTools[index];
            Debug.Log("Settings Not Saved");
            settingsSaved = false;
        }
    }

    public void ToggleSubtitles()
    {
        subtitlesEnabled = subtitlesToggle.isOn;
        Debug.Log("Settings Not Saved");
        settingsSaved = false;
    }

    public void ApplySettings()
    {
        settings.UpdateSettings = true;
        settingsSaved = true;

        #region ACCESSIBILITY
        settings.ThrowIndicatorShown = accessibilityTools[0];
        settings.InfiniteShroomsOn = accessibilityTools[1];
        settings.ShroomDurationOn = accessibilityTools[2];
        settings.InstantThrowOn = accessibilityTools[3];
        settings.NoClipOn = accessibilityTools[4];
        #endregion

        #region DISPLAY
        settings.Brightness = brightness; 
        Screen.SetResolution((int)resValues.x, (int)resValues.y, isFullscreen); 
        Screen.fullScreen = isFullscreen;
        settings.SubtitlesEnabled = subtitlesEnabled;
        settings.ResOption = resDropdown.value;
        settings.IsFullscreen = fullscreenToggle.isOn;
        #endregion

        #region CONTROLS
        #endregion

        #region AUDIO
        #endregion
    }
#endregion

    /// <summary>
    /// Disable all buttons
    /// </summary>
    public void DisableAllButtons()
    {
        continueButton.interactable = false;
        loadGameButton.interactable = false;
        settingsButton.interactable = false;
        journalButton.interactable = false;
        creditsButton.interactable = false;
        exitGameButton.interactable = false;
    }
}
