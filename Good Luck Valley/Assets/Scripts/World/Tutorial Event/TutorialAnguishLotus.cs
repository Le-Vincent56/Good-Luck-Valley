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
    #endregion

    #region FIELDS
    private bool endLevel = false;
    private GameObject[] shroomWalls;
    [SerializeField] private float fadeAmount = 0.02f;
    #endregion

    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        remove = false;
        endLevel = false;

        shroomWalls = new GameObject[3];

        for (int i = 0; i < 3; i++)
        {
            shroomWalls[i] = GameObject.Find("Wall" + i);
        }
    }

    void Update()
    {
        // Check if interactable is triggered
        if (controlTriggered)
        {
            // Interact and set variables
            Interact();
            interacting = true;
            StartCoroutine(FadeVines());

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
            if (!finishedInteracting)
            {   
                pauseMenu.Paused = true;
                //tutorialManager.ShowingDemoEndText = true;
            }
            else if (pauseMenu.Paused)
            {
                foreach (GameObject g in shroomWalls)
                {
                    g.SetActive(false);
                }

                pauseMenu.Paused = false;
            }
        }
    }

    /// <summary>
    /// Disable Lotus tutorial text and end the level
    /// </summary>
    public override void Interact()
    {
        // Lock the player
        disableEvent.Lock();

        // End the level
        endLevel = true;
    }

    private IEnumerator FadeVines()
    {
        foreach (GameObject g in shroomWalls)
        {
            Color color = g.GetComponent<SpriteRenderer>().color;
            g.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a - fadeAmount);
        }
        if (shroomWalls[0].GetComponent<SpriteRenderer>().color.a <= 0)
        {
            finishedInteracting = true;
            disableEvent.Unlock();
        }
        yield return null;
    }
}
