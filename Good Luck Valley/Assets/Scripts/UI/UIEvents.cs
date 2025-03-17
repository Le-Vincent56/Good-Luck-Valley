using GoodLuckValley.UI.Triggers;

namespace GoodLuckValley.Events.UI
{
    public struct SetPaused : IEvent
    {
        public bool Paused;
    }

    public struct FadeTutorialCanvasGroup : IEvent
    {
        public int ID;
        public bool FadeIn;
        public GraphicTrigger Trigger;
    }

    public struct FadeInteractableCanvasGroup : IEvent
    {
        public int ID;
        public float Value;
        public float Duration;
    }
}
