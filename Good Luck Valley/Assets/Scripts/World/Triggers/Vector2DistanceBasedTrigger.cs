using DG.Tweening;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class Vector2DistanceBasedTrigger : DistanceBasedTrigger
    {
        [SerializeField] protected Vector2 minDistanceValue;
        [SerializeField] protected Vector2 maxDistanceValue;
        [SerializeField] protected Vector2 currentDistanceValue;

        [Header("Tweening Variables")]
        [SerializeField] protected Ease correctionEase;
        [SerializeField] protected float correctionDuration;
        private Tween correctionTween;

        protected override void CalculateDistanceValue(float currentDistance)
        {
            // Calculate easing t-value
            float t = 1 - (currentDistance / totalDistance);
            t = ApplyEasing(t);

            // Use Vector2 lerping with easing to calculate the current distance value
            currentDistanceValue = Vector2.Lerp(minDistanceValue, maxDistanceValue, t);
        }

        /// <summary>
        /// Calculate the exit value
        /// </summary>
        protected Vector2 CalculateExitValue(int exitDirection, int minDirection, int maxDirection)
        {
            return (exitDirection == maxDirection) ? maxDistanceValue : minDistanceValue;
        }
    }
}
