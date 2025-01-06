using DG.Tweening;

namespace GoodLuckValley.Architecture.EventBus
{
    public struct FadeScene : IEvent
    {
        public bool FadeIn;
        public Ease EaseType;
        public TweenCallback OnComplete;
    }
}
