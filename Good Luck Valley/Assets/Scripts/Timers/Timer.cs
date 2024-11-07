using System;
using UnityEngine;

namespace GoodLuckValley.Timers
{
    public abstract class Timer : IDisposable
    {
        private bool disposed;
        protected float initialTime;

        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }
        public float Progress => Mathf.Clamp(CurrentTime / initialTime, 0f, 1f);
        public abstract bool IsFinished { get; }

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value)
        {
            initialTime = value;
        }

        ~Timer()
        {
            // Dispose with false if disposed by the finalizer
            Dispose(false);
        }

        /// <summary>
        /// Start the Timer
        /// </summary>
        public void Start()
        {
            // Set the current time to the initial time
            CurrentTime = initialTime;

            // Exit case - the Timer is already running
            if (IsRunning) return;

            // Start running the Timer
            IsRunning = true;

            // Register the Timer into the TimerManager
            TimerManager.RegisterTimer(this);

            // Invoke the Timer start event
            OnTimerStart.Invoke();
        }

        /// <summary>
        /// Stop the Timer
        /// </summary>
        public void Stop()
        {
            // Exit case - the Timer is not running
            if (!IsRunning) return;

            // Stop running the Timer
            IsRunning = false;

            // Deregister the Timer from the TimerManager
            TimerManager.DeregisterTimer(this);

            // Invoke the Timer stop event
            OnTimerStop.Invoke();
        }

        /// <summary>
        /// Tick the Timer
        /// </summary>
        public abstract void Tick();

        /// <summary>
        /// Pause the Timer
        /// </summary>
        public void Pause() => IsRunning = false;

        /// <summary>
        /// Resume the Timer
        /// </summary>
        public void Resume() => IsRunning = true;

        /// <summary>
        /// Reset the Timer to its initial time
        /// </summary>
        public virtual void Reset() => CurrentTime = initialTime;

        /// <summary>
        /// Reset the Timer to a new time
        /// </summary>
        public virtual void Reset(float newTime)
        {
            // Set the initial time to the new time
            initialTime = newTime;

            // Reset the timer
            Reset();
        }

        /// <summary>
        /// Dispose of the Timer
        /// </summary>
        public void Dispose()
        {
            // Dispose with true
            Dispose(true);

            // Supress the finalizer as the resources have already been freed
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Exit case - if the Timer has already been disposed
            if (disposed) return;

            // Check if coming from Dispose() and not the finalizer
            if (disposing)
            {
                // Deregister the Timer from the TimerManager
                TimerManager.DeregisterTimer(this);
            }

            // Set disposed
            disposed = true;
        }
    }
}