using GoodLuckValley.Events;
using GoodLuckValley.World.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPickup : Collectible
{
    #region EVENTS
    [Header("Events")]
    [SerializeField] private GameEvent onUnlockThrow;
    #endregion

    public override void Interact()
    {
        // Unlock the throw
        onUnlockThrow.Raise(this, null);

        // Collect the collectible
        base.Interact();
    }
}
