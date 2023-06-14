using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSpore : Interactable
{
    #region REFERENCES
    private MushroomManager mushroomManager;
    private Tutorial tutorialManager;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mushroomManager = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();

        remove = false;
    }

    /// <summary>
    /// Unlock the Mushroom Throw power and show tutorial text
    /// </summary>
    public override void Interact()
    {
        tutorialManager.ShowingBounceText = true;
        tutorialManager.ShowingMushroomInteractText = false;
        tutorialManager.MushroomInteracted = true;
        mushroomManager.ThrowUnlocked = true;
        finishedInteracting = true;
        remove = true;
    }
}
