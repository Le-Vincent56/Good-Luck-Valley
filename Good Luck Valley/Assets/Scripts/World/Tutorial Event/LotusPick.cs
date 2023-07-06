using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LotusPick : Interactable
{
    #region REFERENCES
    [SerializeField] private DisableScriptableObj disableEvent;
    private PauseMenu pauseMenu;
    #endregion

    #region FIELDS
    [SerializeField] GameObject vineWall;
    [SerializeField] private float fadeAmount;
    #endregion

    void Start()
    {
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        vineWall = GameObject.Find("lotus wall");

        remove = false;
        if (fadeAmount == 0)
        {
            fadeAmount = 0.01f;
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
    }

    private IEnumerator FadeVines()
    {
        pauseMenu.Paused = true;
        Color color = vineWall.GetComponent<Tilemap>().color;
        vineWall.GetComponent<Tilemap>().color = new Color(color.r, color.g, color.b, color.a - fadeAmount);
     
        if (vineWall.GetComponent<Tilemap>().color.a <= 0)
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
        endScreen.GetComponent<Text>().color = new Color(1, 1, 1, endScreen.GetComponent<Text>().color.a + fadeAmount);
        endScreen.GetComponentInChildren<Image>().color = new Color(1, 1, 1, endScreen.GetComponentInChildren<Image>().color.a + fadeAmount);
        endScreen.GetComponentInChildren<Image>().GetComponentInChildren<Text>().color = new Color(1, 1, 1, endScreen.GetComponentInChildren<Image>().GetComponentInChildren<Text>().color.a + fadeAmount);
        GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color.a + fadeAmount);

        if (GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color.a >= 0.85)
        {
            GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .85f);
        }

        if (endScreen.GetComponent<Text>().color.a >= 1)
        {
            finishedInteracting = true;
        }

        yield return null;
    }
}
