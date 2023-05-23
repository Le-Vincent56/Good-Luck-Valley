using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Interactable : MonoBehaviour
{
    #region FIELDS
    protected bool interacting = false;
    protected bool inRange = false;
    protected bool controlTriggered = false;
    protected bool finishedInteracting = false;
    protected bool remove = false;
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
}
