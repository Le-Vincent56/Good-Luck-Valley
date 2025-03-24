using UnityEngine;

namespace GoodLuckValley.Events.Player
{
    public struct PlacePlayer : IEvent
    {
        public Vector3 Position;
    }

    public struct ForcePlayerMove : IEvent
    {
        public bool ForcedMove;
        public int ForcedMoveDirection;
    }

    public struct  TimeWarpCollected : IEvent
    {
        public bool Entering;
    }

    public struct ActivatePlayer : IEvent { }
    public struct DeactivatePlayer : IEvent { }
}
