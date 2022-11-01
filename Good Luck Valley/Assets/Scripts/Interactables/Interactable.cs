using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Interactable : MonoBehaviour
{
    #region FIELDS
    [Header("Interact Variables")]
    public bool interacting = false;
    public bool inRange = false;
    public bool controlTriggered = false;
    public bool finishedInteracting = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
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
