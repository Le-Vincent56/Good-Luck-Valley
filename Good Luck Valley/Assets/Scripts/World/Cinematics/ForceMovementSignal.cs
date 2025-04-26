using GoodLuckValley.Events.Player;
using GoodLuckValley.Events;
using UnityEngine;

namespace GoodLuckValley.World.Cinematics
{
    public class ForceMovementSignal : MonoBehaviour
    {
        [Header("Fields")]
        [SerializeField] private bool forceMovement;
        [SerializeField] private int direction;

        public void ForcePlayerMovement()
        {
            EventBus<ForcePlayerMove>.Raise(new ForcePlayerMove()
            {
                ForcedMove = forceMovement,
                ForcedMoveDirection = direction
            });
        }
    }
}
