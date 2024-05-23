using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Patterns
{
    public abstract class Timer
    {
        #region FIELDS
        protected float initialTime;
        #endregion

        #region PROPERTIES
        protected float Time { get; set; }
        public bool IsRunning { get; protected set; }
        public float Progress => Time / initialTime;
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };
        #endregion

        protected Timer(float value)
        {
            initialTime = value;
            IsRunning = false;
        }

        /// <summary>
        /// Pause the Timer
        /// </summary>
        public void Pause() => IsRunning = false;

        /// <summary>
        /// Resume the Timer
        /// </summary>
        public void Resume() => IsRunning = true;

        /// <summary>
        /// Start the Timer
        /// </summary>
        public void Start()
        {
            // Set the time
            Time = initialTime;

            // Check if running already
            if (!IsRunning)
            {
                // If not, set it to running and invoke events
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        /// <summary>
        /// Stop the Timer
        /// </summary>
        public void Stop()
        {
            // Check if running already
            if (IsRunning)
            {
                // If so, stop it from running and invoke events
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }

        /// <summary>
        /// Tick the Timer
        /// </summary>
        /// <param name="deltaTime"></param>
        public abstract void Tick(float deltaTime);
    }

    public class CountdownTimer : Timer
    {
        #region PROPERTIES
        public bool IsFinished => Time <= 0;
        #endregion

        public CountdownTimer(float value) : base(value) { }

        public override void Tick(float deltaTime)
        {
            // Check if running and there's time left
            if(IsRunning && Time > 0)
            {
                // Subtract time
                Time -= deltaTime;
            }

            // Check if running and time is less than
            // or equal to zero
            if(IsRunning && Time <= 0)
            {
                // Stop the timer
                Stop();
            }
        }

        /// <summary>
        /// Reset the Timer to the initial time
        /// </summary>
        public void Reset() => Time = initialTime;

        /// <summary>
        /// Reset the Timer to a new time
        /// </summary>
        /// <param name="newTime"></param>
        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
    }
}