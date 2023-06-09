using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenusManager : MonoBehaviour
{
    #region REFERENCES
    private GameObject confirmationCheck;
    private GameObject confirmationCheck2;
    private GameObject saveButton;
    private GameObject deleteButton;
    private SpriteRenderer fadeSquare;
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
    #endregion

    #region PROPERTIES
    #endregion

    public void Start()
    {
        if (confirmationCheck = GameObject.Find("ConfirmationCheck"))
        {
            confirmationCheck.SetActive(false);
        }
        if (confirmationCheck2 = GameObject.Find("ConfirmationCheck2"))
        {
            confirmationCheck2.SetActive(false);
        }
        checkQuit = true;
        if (saveButton = GameObject.Find("Save"))
        {
            saveColor = saveButton.GetComponent<Image>().color;
            saveButton.GetComponent<Button>().interactable = false;
            saveButton.GetComponent<Image>().color = new Color(saveColor.r, saveColor.g, saveColor.b, 0.5f);
        }
        if (deleteButton = GameObject.Find("Delete"))
        {
            deleteColor = deleteButton.GetComponent<Image>().color;
            deleteButton.GetComponent<Button>().interactable = false;
            deleteButton.GetComponent<Image>().color = new Color(deleteColor.r, deleteColor.g, deleteColor.b, 0.5f);
        }
        currentScene = SceneManager.GetActiveScene().buildIndex;

        fadeSquare = GameObject.Find("Fade").GetComponent<SpriteRenderer>();
        Debug.Log(fadeSquare);
        fadeIn = true;
        fadeOut = false;

        if (currentScene == 0)
        {
            fadeSquare.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }
    }

    public void Update()
    {
        Debug.Log("Update Selected Save:" + selectedSave);
        FadeIn();
        FadeOut();

        if (currentScene == 2)
        {
            if (selectedSave != 0)
            {
                if (previousScene != 1)
                {
                    saveButton.GetComponent<Image>().color = new Color(saveColor.r, saveColor.g, saveColor.b, 1f);
                    saveButton.GetComponent<Button>().interactable = true;
                }

                deleteButton.GetComponent<Image>().color = new Color(deleteColor.r, deleteColor.g, deleteColor.b, 1f);
                deleteButton.GetComponent<Button>().interactable = true;
            }
            else if (selectedSave == 0 || confirmationCheck.activeSelf == true || confirmationCheck2.activeSelf == true)
            {
                saveButton.GetComponent<Image>().color = new Color(saveColor.r, saveColor.g, saveColor.b, 0.5f);
                saveButton.GetComponent<Button>().interactable = false;

                deleteButton.GetComponent<Image>().color = new Color(deleteColor.r, deleteColor.g, deleteColor.b, 0.5f);
                deleteButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    /// <summary>
    /// OnClick event for the Play Button
    /// </summary>
    public void NavMainMenu()
    {
        // Switch scenes to the Tutorial Tilemap
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Main Menu");
    }

    public void NavSaveFiles()
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("SaveFiles");
    }

    public void NavSettings()
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Settings");
    }

    public void NavJournal()
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Journal");
    }

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
            fadeSquare.color = new Color(0, 0, 0, fadeSquare.color.a - 0.01f);
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
            fadeSquare.color = new Color(0, 0, 0, fadeSquare.color.a + 0.01f);
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
}
