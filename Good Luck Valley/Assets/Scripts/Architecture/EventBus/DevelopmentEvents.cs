namespace GoodLuckValley.Architecture.EventBus
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
