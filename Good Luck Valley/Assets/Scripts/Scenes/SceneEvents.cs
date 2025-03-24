using DG.Tweening;

namespace GoodLuckValley.Events.Scenes
{
    public struct FadeScene : IEvent
    {
        public bool FadeIn;
        public Ease EaseType;
        public TweenCallback OnComplete;
    }
}
