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

    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
        remove = false;
        fadeTimer = 3.0f;
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
            // Start the fade tiemr
            fadeTimer -= Time.deltaTime;

            // Once the fade timer hits 0, finish interacting
            // and go to title screen
            if(fadeTimer <= 0)
            {
                finishedInteracting = true;
                SceneManager.LoadScene("Title Screen");
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
}
