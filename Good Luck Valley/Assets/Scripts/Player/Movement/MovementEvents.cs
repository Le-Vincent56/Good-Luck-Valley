namespace GoodLuckValley.Architecture.EventBus
{
    public struct ForceMove : IEvent
    {
        public bool ManualMove;
        public int ForcedMoveDirection;
    }
}
