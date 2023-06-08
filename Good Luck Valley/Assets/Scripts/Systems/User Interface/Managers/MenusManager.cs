using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenusManager : MonoBehaviour
{
    #region REFERENCES
    private GameObject exitCheck;
    #endregion

    #region FIELDS
    private bool checkQuit;
    #endregion

    #region PROPERTIES
    #endregion

    public void Start()
    {
        exitCheck = GameObject.Find("ExitCheck");
        exitCheck.SetActive(false);
        checkQuit = true;
    }

    /// <summary>
    /// OnClick event for the Play Button
    /// </summary>
    public void OnNavMainMenu()
    {
        // Switch scenes to the Tutorial Tilemap
        SceneManager.LoadScene("Main Menu");
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OnNavSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void OnNavJournal()
    {
        SceneManager.LoadScene("Journal");
    }

    public void OnNavCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void Exit()
    {
        if (checkQuit)
        {
            exitCheck.SetActive(true);
            checkQuit = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
