using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public abstract class Interactable : MonoBehaviour
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
    [SerializeField] protected bool inRange = false;
    protected bool controlTriggered = false;
    protected bool finishedInteracting = false;
    protected bool playedSound = false;
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
        playedSound = false;
        remove = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Show the outline if in range
        if(inRange)
        {
            gameObject.GetComponent<SpriteRenderer>().material.SetInt("_Active", 1);
        } else
        {
            gameObject.GetComponent<SpriteRenderer>().material.SetInt("_Active", 0);
        }

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
}
