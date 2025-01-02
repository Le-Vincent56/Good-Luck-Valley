using GoodLuckValley.Architecture.EventBus;

namespace GoodLuckValley.Player.Animation
{
    public struct ForceDirectionChange : IEvent
    {
        public int DirectionToFace;
        public bool BufferUpdate;
    }
}
