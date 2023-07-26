using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.VFX;

public class LotusPick : Interactable, IData
{
    #region REFERENCES
    [SerializeField] private PauseScriptableObj pauseEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    private VisualEffect lotusParticles;
    #endregion

    #region FIELDS
    [SerializeField] GameObject vineWall;
    [SerializeField] private float fadeAmount;
    #endregion

    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Thickness", 0.02f);
        lotusParticles = GetComponentInChildren<VisualEffect>();

        remove = false;
        if (fadeAmount == 0)
        {
            fadeAmount = 0.01f;
        }
    }

    void Update()
    {
        // Show the outline if in range
        if (inRange)
        {
            gameObject.GetComponent<SpriteRenderer>().material.SetInt("_Active", 1);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().material.SetInt("_Active", 0);
        }

        // Debug.Log(vineWall.activeSelf);
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

                StopAllCoroutines();

                disableEvent.Unlock();
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
        // Save the game
        DataManager.Instance.SaveGame();

        // Lock the player
        disableEvent.Lock();

        // Start the lotus pick by playing the sound
        if(!playedSound)
        {
            StartCoroutine(PlayLotusSounds());
            playedSound = true;
        }
    }

    private IEnumerator FadeVines()
    {
        pauseEvent.SetPaused(true);
        while (vineWall.GetComponent<Tilemap>().color.a > 0)
        {
            vineWall.GetComponent<Tilemap>().color = new Color(vineWall.GetComponent<Tilemap>().color.r, 
                vineWall.GetComponent<Tilemap>().color.g, vineWall.GetComponent<Tilemap>().color.b, 
                vineWall.GetComponent<Tilemap>().color.a - fadeAmount);

            yield return null;
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
    }

    private IEnumerator FadeLotus()
    {
        // While alpha values are under the desired numbers, increase them by an unscaled delta time (because we are paused)
        while (GetComponent<SpriteRenderer>().color.a > 0)
        {
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, GetComponent<SpriteRenderer>().color.a - fadeAmount);
            yield return null;
        }

        // Set active to false
        active = false;
        lotusParticles.enabled = false;
        yield return null;
    }

    private IEnumerator FadeEndScreen(GameObject endScreen)
    {
        while (endScreen.GetComponent<Text>().color.a <= 1)
        {
            endScreen.GetComponent<Text>().color = new Color(1, 1, 1, endScreen.GetComponent<Text>().color.a + fadeAmount);
            endScreen.GetComponentInChildren<Image>().color = new Color(1, 1, 1, endScreen.GetComponentInChildren<Image>().color.a + fadeAmount);
            endScreen.GetComponentInChildren<Image>().GetComponentInChildren<Text>().color = new Color(1, 1, 1, endScreen.GetComponentInChildren<Image>().GetComponentInChildren<Text>().color.a + fadeAmount);
            GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color.a + fadeAmount);

            if (GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color.a >= 0.85)
            {
                GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .85f);
            }
            yield return null;
        }
        finishedInteracting = true;
    } 

    private IEnumerator PlayPickSound()
    {
        // Play lotus pick sound
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.LotusPick, transform.position);

        // Wait until the "pick" noise
        yield return new WaitForSeconds(2.5f);

        // Start the fading coroutines
        StartCoroutine(FadeVines());
        StartCoroutine(FadeLotus());

        // Return
        yield break;
    }

    private IEnumerator PlayLotusSounds()
    {
        // Play the pick sound
        yield return StartCoroutine(PlayPickSound());

        // Play vine sound
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.VineDecompose, transform.position);

        // Return
        yield break;
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
