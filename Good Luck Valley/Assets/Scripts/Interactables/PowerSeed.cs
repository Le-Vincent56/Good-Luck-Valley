using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSeed : Interactable
{
    MushroomManager mushroomManager;

    // Start is called before the first frame update
    void Start()
    {
        remove = false;
        mushroomManager = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
    }

    public override void Interact()
    {
        mushroomManager.ThrowUnlocked = true;
        finishedInteracting = true;
        remove = true;
    }
}
