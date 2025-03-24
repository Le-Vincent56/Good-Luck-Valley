using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using GoodLuckValley.Utilities.PlayerLoop;

namespace GoodLuckValley.Timers
{
    internal static class TimerBootstrapper
    {
        static PlayerLoopSystem timerSystem;

        /// <summary>
        /// Initialize the Timer Bootstrapper
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            // Access the current player loop
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            if (!InsertTimerManager<Update>(ref currentPlayerLoop, 0))
            {
                Debug.LogWarning("Improved Timers are not initialized, unable to register TimerManager into the Update Loop");
                return;
            }

            // Set the player loop
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;

            // Handle changes to the play mode state
            static void OnPlayModeState(PlayModeStateChange state)
            {
                // Check if the play mode state is exiting
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    // Get the current player loop
                    PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

                    // Remove the timer manager
                    RemoveTimerManager<Update>(ref currentPlayerLoop);

                    // Reset the player loop
                    PlayerLoop.SetPlayerLoop(currentPlayerLoop);

                    // Clear the the TimerManager
                    TimerManager.Clear();
                }
            }
#endif
        }

        /// <summary>
        /// Remove a TimerManager from the player loop
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loop"></param>
        static void RemoveTimerManager<T>(ref PlayerLoopSystem loop) => PlayerLoopUtils.RemoveSystem<T>(ref loop, in timerSystem);

        /// <summary>
        /// Insert a TimerManager into a subsystem of the player loop
        /// </summary>
        static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index)
        {
            // Create the timer system
            timerSystem = new PlayerLoopSystem()
            {
                type = typeof(TimerManager),
                updateDelegate = TimerManager.UpdateTimers,
                subSystemList = null
            };

            return PlayerLoopUtils.InsertSystem<T>(ref loop, in timerSystem, index);
        }
    }
}