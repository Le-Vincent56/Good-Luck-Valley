using DG.Tweening;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class FloatDistanceBasedTrigger : DistanceBasedTrigger
    {
        [SerializeField] protected float minDistanceValue;
        [SerializeField] protected float maxDistanceValue;
        [SerializeField] protected float currentDistanceValue;

        [Header("Tweening Variables")]
        [SerializeField] protected Ease correctionEase;
        [SerializeField] protected float correctionDuration;
        private Tween correctionTween;

        protected override void CalculateDistanceValue(float currentDistance)
        {
            // Calculate easing t-value
            float t = 1 - (currentDistance / totalDistance);
            t = ApplyEasing(t);

            // Use float lerping to calculate the current distance value
            currentDistanceValue = Mathf.Lerp(minDistanceValue, maxDistanceValue, t);
        }

        /// <summary>
        /// Calculate the exit value
        /// </summary>
        protected float CalculateExitValue(int exitDirection, int minDirection, int maxDirection)
        {
            return (exitDirection == maxDirection) ? maxDistanceValue : minDistanceValue;
        }
    }
}
