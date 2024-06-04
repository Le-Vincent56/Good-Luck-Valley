using System;
using UnityEngine;
using UnityEngine.Events;

namespace GoodLuckValley.Patterns.Observer
{
    [Serializable]
    public class Observer<T>
    {
        [SerializeField] T value;
        [SerializeField] UnityEvent<T> onValueChanged;

        public T Value
        {
            get => value;
            set => Set(value);
        }

        public Observer(T value, UnityAction<T> callback = null)
        {
            // Set the value
            this.value = value;
            
            // Create and assign the event
            onValueChanged = new UnityEvent<T>();
            if (callback != null) onValueChanged.AddListener(callback);
        }

        public void Set(T value)
        {
            // If the value is the same, return
            if (Equals(this.value, value)) return;
            this.value = value;
            Invoke();
        }

        public void Invoke()
        {
            Debug.Log($"Invoking {onValueChanged.GetPersistentEventCount()} listeners");
            onValueChanged.Invoke(value);
        }

        public void AddListener(UnityAction<T> callback)
        {
            // Return if no callback is given
            if (callback == null) return;

            // Set the observer evnt if not set already
            if (onValueChanged == null) onValueChanged = new UnityEvent<T>();

            // Add the listener
            onValueChanged.AddListener(callback);
        }

        public void RemoveListener(UnityAction<T> callback)
        {
            // Return if no callback is given
            if (callback == null) return;

            // Return if the observer event has not been set
            if (onValueChanged == null) return;

            // Remove the listener
            onValueChanged.RemoveListener(callback);
        }

        public void RemoveAllListeners()
        {
            // Return if the observer event has not been set
            if (onValueChanged == null) return;

            // Remove all listeners from the observer event
            onValueChanged.RemoveAllListeners();
        }

        public void Dispose()
        {
            // Remove all listeners
            RemoveAllListeners();

            // Set the observer event to null
            onValueChanged = null;

            // Default the value
            value = default;
        }
    }
}