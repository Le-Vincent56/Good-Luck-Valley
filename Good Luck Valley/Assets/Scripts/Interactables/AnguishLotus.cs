using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnguishLotus : Interactable
{

    public override void Interact()
    {
        Debug.Log("Lotus Picked");
        finishedInteracting = true;
    }
}
