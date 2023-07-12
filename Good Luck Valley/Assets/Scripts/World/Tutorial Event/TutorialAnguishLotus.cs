using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TutorialAnguishLotus : Interactable, IData
{
    #region REFERENCES
    [SerializeField] private PauseScriptableObj pauseEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    private Tutorial tutorialManager;
    private PlayerMovement playerMovement;
    #endregion

    #region FIELDS
    private bool endLevel = false;
    private DecomposableVine[] shroomWalls;
    [SerializeField] private float fadeAmount = 0.02f;
    #endregion

    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        remove = false;
        endLevel = false;

        shroomWalls = new DecomposableVine[3];

        for (int i = 0; i < 3; i++)
        {
            shroomWalls[i] = GameObject.Find("Wall" + i).GetComponent<DecomposableVine>();
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
            StartCoroutine(FadeLotus());

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
    }

    /// <summary>
    /// Disable Lotus tutorial text and end the level
    /// </summary>
    public override void Interact()
    {
        // Lock the player
        disableEvent.Lock();
    }

    private IEnumerator FadeLotus()
    {
        // While alpha values are under the desired numbers, increase them by an unscaled delta time (because we are paused)
        while (GetComponent<SpriteRenderer>().color.a > 0)
        {
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, GetComponent<SpriteRenderer>().color.a - fadeAmount);
            active = false;
            yield return null;
        }
    }

    private IEnumerator FadeVines()
    {
        foreach (DecomposableVine g in shroomWalls)
        {
            while(g.GetComponent<SpriteRenderer>().color.a > 0)
            {
                g.GetComponent<SpriteRenderer>().color = new Color(g.GetComponent<SpriteRenderer>().color.r, 
                    g.GetComponent<SpriteRenderer>().color.g, 
                    g.GetComponent<SpriteRenderer>().color.b, g.GetComponent<SpriteRenderer>().color.a - g.DecomposeTime);
            }
        }
        if (shroomWalls[shroomWalls.Length - 1].GetComponent<SpriteRenderer>().color.a <= 0)
        {
            foreach (DecomposableVine g in shroomWalls)
            {
                // Set the vine to inactive
                g.Active = false;
                g.gameObject.SetActive(false);
            }
            finishedInteracting = true;
            disableEvent.Unlock();
        }

        yield return null;
    }

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        // Get the data for all the notes that have been collected
        string currentLevel = SceneManager.GetActiveScene().name;

        // Try to get the value of the interactable
        data.levelData[currentLevel].assetsActive.TryGetValue(id, out active);

        // Check if the note has been added
        if (!active)
        {
            // Remove the note
            remove = true;
        }
        Debug.Log("Lotus active: " + active);
        // Set if the gameobject is active
        gameObject.SetActive(active);
    }

    public void SaveData(GameData data)
    {
        string currentLevel = SceneManager.GetActiveScene().name;

        // Check to see if data has the id of the note
        if (data.levelData[currentLevel].assetsActive.ContainsKey(id))
        {
            // If so, remove it
            data.levelData[currentLevel].assetsActive.Remove(id);
        }

        // Add the id and the current bool to make sure everything is up to date
        data.levelData[currentLevel].assetsActive.Add(id, active);
    }
    #endregion
}
