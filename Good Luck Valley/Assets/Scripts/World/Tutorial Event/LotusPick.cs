using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LotusPick : Interactable
{
    #region REFERENCES
    [SerializeField] private PauseScriptableObj pauseEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    #endregion

    #region FIELDS
    [SerializeField] GameObject vineWall;
    [SerializeField] private float fadeAmount;
    #endregion

    void Start()
    {
        vineWall = GameObject.Find("lotus wall");

        remove = false;
        if (fadeAmount == 0)
        {
            fadeAmount = 0.01f;
        }
    }

    void Update()
    {
        Debug.Log("Control trigger: " + controlTriggered);
        // Check if interactable is triggered
        if (controlTriggered)
        {
            // Interact and set variables
            Interact();
            
            interacting = true;
            StartCoroutine(FadeVines());
            StartCoroutine(FadeLotus());

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
        pauseEvent.SetPaused(true);
        while(vineWall.GetComponent<Tilemap>().color.a > 0)
        {
            vineWall.GetComponent<Tilemap>().color = new Color(vineWall.GetComponent<Tilemap>().color.r, 
                vineWall.GetComponent<Tilemap>().color.g, vineWall.GetComponent<Tilemap>().color.b, 
                vineWall.GetComponent<Tilemap>().color.a - vineWall.GetComponent<DecomposableVine>().DecomposeTime);
        }

        disableEvent.Unlock();
        finishedInteracting = true;
        vineWall.GetComponent<DecomposableVine>().Active = false;
        vineWall.SetActive(false);
        pauseEvent.SetPaused(false);

        GameObject endScreen = GameObject.Find("Demo Ending Text");
        if (endScreen != null)
        {
            finishedInteracting = false;
            pauseEvent.SetPaused(true);
            disableEvent.Lock();
            StartCoroutine(FadeEndScreen(endScreen));
            endScreen.GetComponentInChildren<Button>().interactable = true;
            yield return null;
        }

        yield return null;
    }

    private IEnumerator FadeLotus()
    {
        // While alpha values are under the desired numbers, increase them by an unscaled delta time (because we are paused)
        while (GetComponent<SpriteRenderer>().color.a > 0)
        {
            Debug.Log("?");
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, GetComponent<SpriteRenderer>().color.a - 0.01f);
            yield return null;
        }

        // Set active to false
        active = false;
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
            yield return null;
        }

        yield return null;
    }
}
