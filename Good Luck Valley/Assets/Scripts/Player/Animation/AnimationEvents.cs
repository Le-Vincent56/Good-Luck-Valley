namespace GoodLuckValley.Architecture.EventBus
{
    public struct ForceDirectionChange : IEvent
    {
        public int DirectionToFace;
        public bool BufferUpdate;
    }
}
