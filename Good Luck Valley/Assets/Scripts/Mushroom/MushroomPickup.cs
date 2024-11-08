using GoodLuckValley.Cameras;
using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.VFX;
using GoodLuckValley.World.Interactables;
using UnityEngine;

public class MushroomPickup : Collectible
{
    #region EVENTS
    [Header("Events")]
    [SerializeField] private GameEvent onDisablePlayerTimed;
    [SerializeField] private GameEvent onUnlockThrow;

    [Header("Fields")]
    [SerializeField] private AnimationCurve shakeCurve;
    [SerializeField] private float pickupDuration;
    [SerializeField] private float pickupShakeIntensity;

    [Header("Wwise")]
    [SerializeField] private AK.Wwise.Event playRumble;

    Blackboard playerBlackboard;
    BlackboardKey unlockedThrow;
    #endregion

    [Space]
    [SerializeField] private ShroomRoomLightController shroomRoomLightController;

    protected override void Start()
    {
        base.Start();

        playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
        unlockedThrow = playerBlackboard.GetOrRegisterKey("UnlockedThrow");
    }

    public override void Interact()
    {
        // Play the sound effect
        playRumble.Post(gameObject);
        
        // Disable the player
        onDisablePlayerTimed.Raise(this, pickupDuration);

        // Shake the camera
        CameraManager.Instance.ShakeCamera(shakeCurve, pickupDuration, pickupShakeIntensity);

        // Update the blackboard
        if (playerBlackboard.TryGetValue(unlockedThrow, out bool blackboardValue))
        {
            playerBlackboard.SetValue(unlockedThrow, true);
        }

        // Unlock the throw
        onUnlockThrow.Raise(this, null);

        shroomRoomLightController.StartLightTransition();

        // Collect the collectible
        base.Interact();
    }
}
