using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class FloatDistanceBasedTrigger : DistanceBasedTrigger
    {
        [SerializeField] protected float minDistanceValue;
        [SerializeField] protected float maxDistanceValue;
        [SerializeField] protected float currentDistanceValue;

        protected override void CalculateDistanceValue(float currentDistance)
        {
            // Use float lerping to calculate the current distance value
            currentDistanceValue = Mathf.Lerp(minDistanceValue, maxDistanceValue, 1 - (currentDistance / totalDistance));
        }
    }
}
