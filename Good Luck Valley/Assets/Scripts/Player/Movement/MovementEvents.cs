namespace GoodLuckValley.Architecture.EventBus
{
    public struct ForcePlayerMove : IEvent
    {
        public bool ForcedMove;
        public int ForcedMoveDirection;
    }
}
