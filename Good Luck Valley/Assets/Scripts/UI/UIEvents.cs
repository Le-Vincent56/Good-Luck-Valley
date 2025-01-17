using GoodLuckValley.Architecture.EventBus;
using UnityEngine;

namespace GoodLuckValley.UI.Events
{
    public struct FadeGraphic : IEvent
    {
        public int ID;
        public bool FadeIn;
    }
}
