using GoodLuckValley.Architecture.EventBus;
using UnityEngine;

namespace GoodLuckValley
{
    public struct PlaceFadeCamera : IEvent
    {
        public Vector3 Position;
    }
}
