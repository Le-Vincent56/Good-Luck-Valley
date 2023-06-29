using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotusPick : Interactable
{
    #region REFERENCES
    [SerializeField] private DisableScriptableObj disableEvent;
    private Tutorial tutorialManager;
    private PlayerMovement playerMovement;
    private PauseMenu pauseMenu;
    #endregion

    #region FIELDS
    private bool endLevel = false;
    [SerializeField] GameObject vineWall;
    [SerializeField] private float fadeTimer = 3.0f;
    #endregion

    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        vineWall = GameObject.Find("AnguishVine");

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
            StartCoroutine(FadeVines());

            // If the inteaction has finished, reset the variables
            if (finishedInteracting)
            {
                controlTriggered = false;
            }
        }
        else
        {
            // If the control is not triggered, set interacting to false
            interacting = false;
        }

        // If endlevel
        if (endLevel)
        {
            // Start the fade timer
            if (!finishedInteracting)
            {
                disableEvent.Lock();
                pauseMenu.Paused = true;
            }
            else if (pauseMenu.Paused)
            {
                disableEvent.Unlock();
                pauseMenu.Paused = false;
            }
        }
    }

    /// <summary>
    /// Disable Lotus tutorial text and end the level
    /// </summary>
    public override void Interact()
    {
        // End the level
        endLevel = true;
    }

    private IEnumerator FadeVines()
    {
        Color color = vineWall.GetComponent<SpriteRenderer>().color;
        vineWall.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a - 0.001f);
     
        if (vineWall.GetComponent<SpriteRenderer>().color.a <= 0)
        {
            finishedInteracting = true;
        }
        yield return null;
    }
}
