using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        pauseMenu.Paused = true;
        Color color = vineWall.GetComponent<SpriteRenderer>().color;
        vineWall.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, color.a - 0.01f);
     
        if (vineWall.GetComponent<SpriteRenderer>().color.a <= 0)
        {
            disableEvent.Unlock();
            finishedInteracting = true;
            vineWall.SetActive(false);
            pauseMenu.Paused = false;

            GameObject endScreen = GameObject.Find("Demo Ending Text");
            if (endScreen != null)
            {
                finishedInteracting = false;
                pauseMenu.Paused = true;
                disableEvent.Lock();
                StartCoroutine(FadeEndScreen(endScreen));
            }
        }
        yield return null;
    }

    private IEnumerator FadeEndScreen(GameObject endScreen)
    {
        endScreen.GetComponent<Text>().color = new Color(1, 1, 1, endScreen.GetComponent<Text>().color.a + 0.01f);
        endScreen.GetComponentInChildren<Image>().color = new Color(1, 1, 1, endScreen.GetComponentInChildren<Image>().color.a + 0.01f);
        endScreen.GetComponentInChildren<Image>().GetComponentInChildren<Text>().color = new Color(1, 1, 1, endScreen.GetComponentInChildren<Image>().GetComponentInChildren<Text>().color.a + 0.01f);
        GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color.a + 0.01f);

        if (GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color.a >= 1)
        {
            GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .85f);
            finishedInteracting = true;
        }

        yield return null;
    }
}
