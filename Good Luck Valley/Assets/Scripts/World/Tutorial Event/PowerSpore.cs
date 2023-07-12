using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerSpore : Interactable, IData
{
    #region REFERENCES
    [SerializeField] private MushroomScriptableObj mushroomEvent;
    private MushroomManager mushroomManager;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mushroomManager = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();

        remove = false;
    }

    /// <summary>
    /// Unlock the Mushroom Throw power and show tutorial text
    /// </summary>
    public override void Interact()
    {
        mushroomEvent.UnlockThrow();
        finishedInteracting = true;
        remove = true;
        active = false;
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
