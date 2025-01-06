using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using GoodLuckValley.Utilities.PredefinedAssembly;
using GoodLuckValley.Architecture.EventBus;

namespace GoodLuckValley.Utilities.EventBus
{
    public static class EventBusUtils
    {
        public static IReadOnlyList<Type> EventTypes { get; set; }
        public static IReadOnlyList<Type> EventBusTypes { get; set; }

#if UNITY_EDITOR
        public static PlayModeStateChange PlayModeState { get; set; }

        /// <summary>
        /// Initialize Editor-specific functions
        /// </summary>
        [InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// Handle Play Mode state changes
        /// </summary>
        /// <param name="state"></param>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Set the Play Mode state
            PlayModeState = state;

            // Check if exiting Play Mode
            if (state == PlayModeStateChange.ExitingPlayMode)
                // Clear all the busses to prevent a memory leak
                ClearAllBusses();
        }
#endif

        /// <summary>
        /// Initialize the Event Busses
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            EventTypes = PredefinedAssemblyUtils.GetTypes(typeof(IEvent));
            EventBusTypes = InitializeAllBusses();
        }

        /// <summary>
        /// Create and initialize all Event Busses in the application
        /// </summary>
        /// <returns></returns>
        private static List<Type> InitializeAllBusses()
        {
            // Create a private List to store Event Bus Types
            List<Type> eventBusTypes = new List<Type>();

            // Create a type definition of a generic EventBus
            Type typedef = typeof(EventBus<>);

            // Iterate through each Event Type
            foreach (Type eventType in EventTypes)
            {
                // Get a bus type from the Event Type
                Type busType = typedef.MakeGenericType(eventType);

                // Add the bus type to the List
                eventBusTypes.Add(busType);
            }

            return eventBusTypes;
        }

        /// <summary>
        /// Debug the Event Bus
        /// </summary>
        public static void Debug()
        {
            string message = "Event Types: ";
            foreach(Type eventType in EventTypes)
            {
                message += $"\n{eventType}";
            }

            UnityEngine.Debug.LogError(message);

            string busMessage = "Event Bus Types: ";
            foreach (Type eventType in EventBusTypes)
            {
                busMessage += $"\n{eventType}";
            }

            UnityEngine.Debug.LogError(busMessage);
        }

        /// <summary>
        /// Clears all Event Busses in the application
        /// </summary>
        public static void ClearAllBusses()
        {
            for (int i = 0; i < EventBusTypes.Count; i++)
            {
                // Get the bus type
                Type busType = EventBusTypes[i];

                // Get the bus' "Clear" method
                MethodInfo clearMethod = busType.GetMethod(
                    "Clear",
                    BindingFlags.Static | BindingFlags.NonPublic
                );

                // Invoke the "Clear" method
                clearMethod?.Invoke(null, null);
            }
        }
    }
}