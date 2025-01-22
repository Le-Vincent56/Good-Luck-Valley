using System.Collections.Generic;
using GoodLuckValley.Extensions.List;

namespace GoodLuckValley.Timers
{
    public static class TimerManager
    {
        private static readonly List<Timer> timers = new();
        private static readonly List<Timer> sweep = new();

        /// <summary>
        /// Register a Timer
        /// </summary>
        public static void RegisterTimer(Timer timer) => timers.Add(timer);

        /// <summary>
        /// Deregister a Timer
        /// </summary>
        public static void DeregisterTimer(Timer timer) => timers.Remove(timer);

        /// <summary>
        /// Check if a Timer is registered
        /// </summary>
        public static bool CheckRegistry(Timer timer) => timers.Contains(timer);

        /// <summary>
        /// Update the Timers
        /// </summary>
        public static void UpdateTimers()
        {
            // Exit case - if there are no Timers to manage
            if (timers.Count == 0) return;

            // Add the timers to the sweep list
            sweep.RefreshWith(timers);

            // Iterate through each Timer in the sweep list
            foreach (Timer timer in sweep)
            {
                // Tick the Timer
                timer.Tick();
            }
        }

        /// <summary>
        /// Clear the Timers list
        /// </summary>
        public static void Clear()
        {
            // Refresh the sweep list with the Timers
            sweep.RefreshWith(timers);

            // Iterate through each timer in the sweep list
            foreach (Timer timer in sweep)
            {
                // Dispose of the timer
                timer.Dispose();
            }

            // Clear the lists
            timers.Clear();
            sweep.Clear();
        }
    }
}