namespace GoodLuckValley.Events.Animation
{
    public struct ForceDirectionChange : IEvent
    {
        public int DirectionToFace;
        public bool BufferUpdate;
    }
}
