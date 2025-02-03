using System.Collections.Generic;

namespace GoodLuckValley.Events
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> bindings = new();

        /// <summary>
        /// Register an Event Binding with the Event Bus
        /// </summary>
        public static void Register(EventBinding<T> binding) => bindings.Add(binding);

        /// <summary>
        /// Deregister an Event Binding with the Event Bus
        /// </summary>
        /// <param name="binding"></param>
        public static void Deregister(EventBinding<T> binding) => bindings.Remove(binding);

        /// <summary>
        /// Raise an Event
        /// </summary>
        public static void Raise(T @event)
        {
            // Get all of the bindings
            HashSet<IEventBinding<T>> snapshot = new HashSet<IEventBinding<T>>(bindings);

            // Iterate through each binding
            foreach (IEventBinding<T> binding in snapshot)
            {
                //Debug.LogError($"Checking Event Binding: {binding}");

                // Check if the HashSet contains the bindings
                if (bindings.Contains(binding))
                {
                    //Debug.LogError($"Contains Event Binding: {binding}");

                    // Invoke the binding
                    binding.OnEvent.Invoke(@event);
                    binding.OnEventNoArgs.Invoke();
                }
            }
        }

        /// <summary>
        /// Clear the Event Bus from its Event Bindings
        /// </summary>
        private static void Clear() => bindings.Clear();
    }
}