using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TutorialAnguishLotus : Interactable
{
    #region REFERENCES
    [SerializeField] private DisableScriptableObj disableEvent;
    private Tutorial tutorialManager;
    private PlayerMovement playerMovement;
    private PauseMenu pauseMenu;
    [SerializeField] GameObject fadeEffect;
    #endregion

    #region FIELDS
    private bool endLevel = false;
    [SerializeField] private float fadeTimer = 3.0f;
    #endregion

    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        remove = false;
        fadeTimer = 2.0f;
        endLevel = false;
    }

    void Update()
    {
        // Check if interactable is triggered
        if (controlTriggered)
        {
            // Interact and set variables
            Interact();
            interacting = true;

            // If the inteaction has finished, reset the variables
            if (finishedInteracting)
            {
                controlTriggered = false;
            }
        } else
        {
            // If the control is not triggered, set interacting to false
            interacting = false;
        }

        // If endlevel
        if(endLevel)
        {
            // Start the fade timer
            if(!pauseMenu.Paused)
            {
                disableEvent.Lock();
                pauseMenu.Paused = true;
                tutorialManager.ShowingDemoEndText = true;
            }

            if (fadeEffect.GetComponent<SpriteRenderer>().color.a < .6f)
            {
                fadeEffect.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, (Time.deltaTime * .5f));
            }
            else if (tutorialManager.ShowingDemoEndText)
            {
                tutorialManager.ShowingDemoEndText = false;
            }

            fadeTimer -= Time.deltaTime;

            // Once the fade timer hits 0, finish interacting
            // and go to title screen
            if (fadeTimer <= 0)
            {
                finishedInteracting = true;
            }
        }

    }

    /// <summary>
    /// Disable Lotus tutorial text and end the level
    /// </summary>
    public override void Interact()
    {
        // Fade the Tutorial Text
        tutorialManager.ShowingLotusText = false;

        // End the level
        endLevel = true;
    }

    /// <summary>
    /// Redirect Player to Title Screen
    /// </summary>

    public void OnClickTitle()
    {
        // Load Title Screen after interacting with the Lotus
        if (finishedInteracting)
        {
            SceneManager.LoadScene("Title Screen");
        }
    }
}
