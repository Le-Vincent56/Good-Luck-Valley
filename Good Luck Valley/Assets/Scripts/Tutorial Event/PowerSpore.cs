using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSpore : Interactable
{
    MushroomManager mushroomManager;
    Tutorial tutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        remove = false;
        mushroomManager = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
    }

    public override void Interact()
    {
        tutorialManager.ShowingBounceText = true;
        mushroomManager.ThrowUnlocked = true;
        finishedInteracting = true;
        remove = true;
    }
}
