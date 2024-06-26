using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.Patterns.ServiceLocator;
using GoodLuckValley.World.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPickup : Collectible
{
    #region EVENTS
    [Header("Events")]
    [SerializeField] private GameEvent onTeachInteractable;
    [SerializeField] private GameEvent onUnlockThrow;

    Blackboard unlockBlackboard;
    BlackboardKey unlockedSpiritPower;
    #endregion

    protected override void Start()
    {
        base.Start();

        unlockBlackboard = ServiceLocator.For(this).Get<BlackboardController>().GetBlackboard("Unlocks");
        unlockedSpiritPower = unlockBlackboard.GetOrRegisterKey("UnlockedSpiritPower");
    }

    public override void Interact()
    {
        Debug.Log("Interact");

        // Teach interactable
        onTeachInteractable.Raise(this, null);

        // Unlock the throw
        onUnlockThrow.Raise(this, null);

        // Update the blackboard
        if (unlockBlackboard.TryGetValue(unlockedSpiritPower, out bool blackboardValue))
            unlockBlackboard.SetValue(unlockedSpiritPower, true);

        // Collect the collectible
        base.Interact();
    }
}
