using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public abstract class Interactable : MonoBehaviour, IData
{
    #region FIELDS
    // Create unique ids for each note
    [SerializeField] protected string id;
    [ContextMenu("Generate GUID for ID")]
    protected void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    protected bool interacting = false;
    protected bool inRange = false;
    protected bool controlTriggered = false;
    protected bool finishedInteracting = false;
    [SerializeField] protected bool remove = false;
    [SerializeField] protected bool active = true;
    #endregion

    #region PROPERTIES
    public bool InRange { get { return inRange; } set { inRange = value; } }
    public bool ControlTriggered { get { return controlTriggered; } set { controlTriggered = value; } }
    public bool Remove { get { return remove; } set { remove = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        remove = false;
    }

    // Update is called once per frame
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
        }
        else
        {
            interacting = false;
        }
    }

    /// <summary>
    /// Interaction with the object
    /// </summary>
    public abstract void Interact();

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        // Get the data for all the notes that have been collected
        string currentLevel = SceneManager.GetActiveScene().name;
        Debug.Log(data);

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
