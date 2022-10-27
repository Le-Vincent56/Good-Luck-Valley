using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnguishLotus : Interactable
{
    public override void Interact()
    {
        // Interact with the lotus
        Debug.Log("Lotus Picked");
        finishedInteracting = true;
    }
}
