using UnityEngine;

namespace GoodLuckValley.Architecture.EventBus
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
}
