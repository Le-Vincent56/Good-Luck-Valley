using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialAnguishLotus : Interactable
{
    private bool endLevel = false;
    [SerializeField] Tutorial tutorialManager;
    [SerializeField] float fadeTimer = 3.0f;
    private PlayerMovement playerMovement;
    private PauseMenu pauseMenu;

    // Demo effect
    [SerializeField] GameObject fadeEffect;

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
            if(!pauseMenu.paused)
            {
                playerMovement.MoveInput = Vector2.zero;
                pauseMenu.paused = true;
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
                Debug.Log("cum");
                finishedInteracting = true;
            }
        }

    }

    public override void Interact()
    {
        // Fade the Tutorial Text
        tutorialManager.ShowingLotusText = false;

        // End the level
        endLevel = true;
    }

    public void OnClickTitle()
    {
        if (finishedInteracting)
        {
            SceneManager.LoadScene("Title Screen");
        }
    }
}
