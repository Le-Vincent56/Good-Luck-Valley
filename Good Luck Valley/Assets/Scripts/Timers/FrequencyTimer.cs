using System;
using UnityEngine;

namespace GoodLuckValley.Timers
{
    public class FrequencyTimer : Timer
    {
        protected float timeThreshold;

        public override bool IsFinished => !IsRunning;
        public float TicksPerSecond { get; protected set; }

        public Action OnTick = delegate { };

        public FrequencyTimer(float ticksPerSecond) : base(0)
        {
            // Calculate the time threshold from the ticks per second
            CalculateTimeThreshold(ticksPerSecond);
        }

        public override void Tick()
        {
            // Check if the Timer is running and the current time is greater or equal
            // to the time threshold
            if (IsRunning && CurrentTime >= timeThreshold)
            {
                // If so, reduce the current time by the time threshold
                CurrentTime -= timeThreshold;

                // Invoke the tick event
                OnTick.Invoke();
            }

            // Check if the Timer is running and the current time
            // is less than the time threshold
            if (IsRunning && CurrentTime < timeThreshold)
            {
                // If so, add deltaTime to the current time
                CurrentTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// Reset the Frequency Timer by setting the current time to 0
        /// </summary>
        public override void Reset() => CurrentTime = 0;

        /// <summary>
        /// Reset the Frequency Timer using a new time threshold
        /// </summary>
        /// <param name="newTicksPerSecond"></param>
        public override void Reset(float newTicksPerSecond)
        {
            // Calculate the new time threshold
            CalculateTimeThreshold(newTicksPerSecond);

            // Reset the Frequency Timer
            Reset();
        }

        /// <summary>
        /// Calculate the time threshold from the ticks per second
        /// </summary>
        protected virtual void CalculateTimeThreshold(float ticksPerSecond)
        {
            // Set the ticks per second
            TicksPerSecond = ticksPerSecond;

            // Calculate the time threshold
            timeThreshold = 1f / TicksPerSecond;
        }
    }
}