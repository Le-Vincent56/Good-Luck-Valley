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
    #endregion

    #region FIELDS
    private GameObject shroomVines;
    [SerializeField] private float fadeAmount = 0.001f;
    #endregion

    void Start()
    {
        remove = false;

        shroomVines = GameObject.Find("ShroomVineWall");
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

        // Play the vine flee sound
        if (!playedSound)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.VineFlee, transform.position);
            playedSound = true;
        }
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
        //Shit too fast changing to fadeAmount
        //shroomVines.GetComponent<DecomposableVine>().DecomposeTime

        while (shroomVines.GetComponent<SpriteRenderer>().color.a > 0)
        {
            shroomVines.GetComponent<SpriteRenderer>().color = new Color(shroomVines.GetComponent<SpriteRenderer>().color.r,
            shroomVines.GetComponent<SpriteRenderer>().color.g,
            shroomVines.GetComponent<SpriteRenderer>().color.b, 
            shroomVines.GetComponent<SpriteRenderer>().color.a - fadeAmount);

            yield return null;
        }
        if (shroomVines.GetComponent<SpriteRenderer>().color.a <= 0)
        {
            shroomVines.GetComponent<DecomposableVine>().Active = false;
            shroomVines.SetActive(false);
            finishedInteracting = true;
            disableEvent.Unlock();
        }
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
