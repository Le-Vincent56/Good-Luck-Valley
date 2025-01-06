using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class Vector2DistanceBasedTrigger : DistanceBasedTrigger
    {
        [SerializeField] protected Vector2 minDistanceValue;
        [SerializeField] protected Vector2 maxDistanceValue;
        [SerializeField] protected Vector2 currentDistanceValue;

        protected override void CalculateDistanceValue(float currentDistance)
        {
            // Use Vector2 lerping to calculate the current distance value
            currentDistanceValue = Vector2.Lerp(minDistanceValue, maxDistanceValue, 1 - (currentDistance / totalDistance));
        }
    }
}
