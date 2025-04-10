using GoodLuckValley.Events;
using GoodLuckValley.Events.Player;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class ForceMovementTrigger : EnterExitTrigger
    {
        [Header("Feilds")]
        [SerializeField] private bool forceMovement;
        [SerializeField] private int direction;

        public override void OnEnter(PlayerController controller)
        {
            EventBus<ForcePlayerMove>.Raise(new ForcePlayerMove()
            {
                ForcedMove = forceMovement,
                ForcedMoveDirection = direction
            });
        }

        public override void OnExit(PlayerController controller) { }
    }
}
