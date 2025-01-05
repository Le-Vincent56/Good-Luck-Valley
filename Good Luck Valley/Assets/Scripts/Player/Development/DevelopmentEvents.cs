using GoodLuckValley.Architecture.EventBus;

namespace GoodLuckValley.Player.Development.Events
{
    public struct ToggleDevelopmentTools : IEvent
    {
        public bool Active;
        public bool Debug;
        public bool NoClip;
    }

    public struct ChangeDevelopmentTools : IEvent
    {
        public bool Debug;
        public bool NoClip;
    }
}
