namespace GoodLuckValley.Events.Development
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
        public bool Invisible;
    }
}
