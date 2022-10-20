using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
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
        if (controlTriggered)
        {
            interacting = true;
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

    public virtual void Interact()
    {

    }
}
