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

    Blackboard playerBlackboard;
    BlackboardKey unlockedThrow;
    #endregion

    protected override void Start()
    {
        base.Start();

        playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
        unlockedThrow = playerBlackboard.GetOrRegisterKey("UnlockedThrow");
    }

    public override void Interact()
    {
        Debug.Log("Interact");

        // Teach interactable
        onTeachInteractable.Raise(this, null);

        // Update the blackboard
        if (playerBlackboard.TryGetValue(unlockedThrow, out bool blackboardValue))
            playerBlackboard.SetValue(unlockedThrow, true);

        // Collect the collectible
        base.Interact();
    }
}
