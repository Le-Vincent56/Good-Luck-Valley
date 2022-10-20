using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnguishLotus : Interactable
{
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
            Interact();
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

    public override void Interact()
    {
        Debug.Log("Lotus Picked");
        finishedInteracting = true;
    }
}
