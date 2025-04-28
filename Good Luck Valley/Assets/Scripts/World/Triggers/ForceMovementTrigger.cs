using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Player;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Scenes;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class ForceMovementTrigger : EnterExitTrigger
    {
        protected SceneLoader sceneLoader;

        [Header("Fields")]
        [SerializeField] private bool forceMovement;
        [SerializeField] private int direction;
        [SerializeField] private bool fromGate = false;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
        }

        public override void OnEnter(PlayerController controller)
        {
            // Exit case - if the player needs to come from a gate and is not
            if (fromGate && !sceneLoader.LoadingFromGate) return;

            EventBus<ForcePlayerMove>.Raise(new ForcePlayerMove()
            {
                ForcedMove = forceMovement,
                ForcedMoveDirection = direction
            });
        }

        public override void OnExit(PlayerController controller) { }
    }
}
