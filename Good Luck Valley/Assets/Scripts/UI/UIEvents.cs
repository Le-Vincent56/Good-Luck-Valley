using GoodLuckValley.UI.Triggers;

namespace GoodLuckValley.Events.UI
{
    public struct FadeGraphic : IEvent
    {
        public int ID;
        public bool FadeIn;
        public GraphicTrigger Trigger;
    }
}
