using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSpore : Interactable
{
    #region REFERENCES
    private MushroomManager mushroomManager;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mushroomManager = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();

        remove = false;
    }

    /// <summary>
    /// Unlock the Mushroom Throw power and show tutorial text
    /// </summary>
    public override void Interact()
    {
        mushroomManager.ThrowUnlocked = true;
        finishedInteracting = true;
        remove = true;
        mushroomManager.ShroomCounter.ResetQueue();
    }
}
