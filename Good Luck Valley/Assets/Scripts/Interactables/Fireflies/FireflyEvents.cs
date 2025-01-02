using GoodLuckValley.Architecture.EventBus;

namespace GoodLuckValley.Interactables.Fireflies.Events
{
    public struct ActivateLantern : IEvent
    {
        public int Channel;
    }
}
