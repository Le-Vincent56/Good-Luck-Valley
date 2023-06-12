using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
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
    private DevTools devTools;
    #endregion

    #region FIELDS
    private bool checkQuit;
    static int previousScene;
    static int currentScene;
    private int selectedSave;
    private Color saveColor;
    private Color deleteColor;
    private bool fadeIn;
    private bool fadeOut;
    private int sceneLoadNum;
    private bool checkButtons;
    #endregion

    #region PROPERTIES
    public int CurrentScene { get { return currentScene; } }
    #endregion

    public void Start()
    {
        // Get the current scene
        currentScene = SceneManager.GetActiveScene().buildIndex;

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
            navButtons = new GameObject[4];
            sliders = new Slider[6];
            textInputs = new GameObject[6];
            for (int i = 0; i < 4; i++)
            {
                navButtons[i] = GameObject.Find("Button" + i);
            }

            navButtons[0].GetComponent<Button>().interactable = false;

            for (int i = 0; i < 6; i++)
            {
                textInputs[i] = GameObject.Find("TextInput" + i);
                sliders[i] = GameObject.Find("Slider" + i).GetComponent<Slider>();

                // Setting default values, change once we add actual functionality
                textInputs[i].GetComponent<TMP_InputField>().text = "50";
                sliders[i].value = 50;
            }

            devTools = GameObject.Find("Dev Tools").GetComponent<DevTools>();
        }
        #endregion

        #region FADING BETWEEN SCENES
        if (currentScene != 0)
        {
            fadeSquare = GameObject.Find("Fade").GetComponent<SpriteRenderer>();
            fadeIn = true;
            fadeOut = false;
            fadeSquare.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
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
                // Check if the previous scene isn't 1, main menu (you cant make a save if you are entering from the main menu)
                if (previousScene != 1)
                {
                    // If not, make save button interactable and have full transparency
                    saveButton.GetComponent<Image>().color = new Color(saveColor.r, saveColor.g, saveColor.b, 1f);
                    saveButton.GetComponent<Button>().interactable = true;
                }

                // Make delete button interactable and have full transparency
                deleteButton.GetComponent<Image>().color = new Color(deleteColor.r, deleteColor.g, deleteColor.b, 1f);
                deleteButton.GetComponent<Button>().interactable = true;

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
        #endregion
    }

    /// <summary>
    /// Navigates to the main menu scene
    /// </summary>
    public void NavMainMenu()
    {
        // Switch scenes to the Tutorial Tilemap
        Debug.Log("WHAT TF");
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Main Menu");
    }

    /// <summary>
    /// Navigates to the save files scene
    /// </summary>
    public void NavSaveFiles()
    {
        Debug.Log("KILL YOURSELF");
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

    /// <summary>
    /// Confirms what the user wants to do, changes functionality depending on the scene it is being called in
    /// </summary>
    /// <param name="confirmCheckNum"> The number associated with the confirmation check box that is being called (input in inspector)</param>
    public void ConfirmationCheck(int confirmCheckNum)
    {
        if (checkQuit)
        {
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

    public void Back()
    {
        fadeOut = true;
        sceneLoadNum = previousScene;
    }

    public void SelectSave(int saveNum)
    {
        Debug.Log("Save Selected:" + saveNum);
        selectedSave = saveNum;
    }

    private void FadeIn()
    {
        if (fadeIn)
        {
            fadeSquare.color = new Color(0, 0, 0, fadeSquare.color.a - 0.005f);
            if (fadeSquare.color.a <= 0)
            {
                fadeIn = false;
            }
        }
    }

    private void FadeOut()
    {
        if (fadeOut)
        {
            Debug.Log("Fadeing out");
            fadeSquare.color = new Color(0, 0, 0, fadeSquare.color.a + 0.005f);
            if (fadeSquare.color.a >= 1)
            {
                Debug.Log("Fade Done");
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
            else if (sceneLoadNum == 6)
            {
                NavCredits();
            }
        }
    }

    public void CheckFade(int sceneToLoad)
    {
        fadeOut = true;
        sceneLoadNum = sceneToLoad;
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
}
