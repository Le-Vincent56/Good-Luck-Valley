using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley
{
    public class RTPCTimer : MonoBehaviour
    {
        [Header("WWise RTPCs")]
        [SerializeField] private AK.Wwise.RTPC rtpc;

        private CountdownTimer rtpcTimer;
        [SerializeField] private float startingValue;
        [SerializeField] private float endingValue;
        [SerializeField] private float timerDuration;

        private void Awake()
        {
            // Create the RTPC Timer
            rtpcTimer = new CountdownTimer(timerDuration);

            rtpcTimer.OnTimerStart += () =>
            {
                // Set the global value for the RTPC to the starting value
                rtpc.SetGlobalValue(startingValue);
            };

            rtpcTimer.OnTimerTick += () =>
            {
                // Set the global value for the RTPC to reflect the progress of the timer in between the starting and ending value
                float interpolatedValue = Mathf.Lerp(startingValue, endingValue, 1 - rtpcTimer.Progress);
                rtpc.SetGlobalValue(interpolatedValue);
            };

            rtpcTimer.OnTimerStop += () =>
            {
                // Set the global value for the RTPC to the ending value
                rtpc.SetGlobalValue(endingValue);
            };
        }

        private void OnDestroy()
        {
            // Stop the timer if it exists
            rtpcTimer?.Dispose();
        }
        
        /// <summary>
        /// Start the RTPC Timer
        /// </summary>
        public void StartTimer() => rtpcTimer.Start();
    }
}
