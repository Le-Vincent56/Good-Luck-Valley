using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Events
{
    [CreateAssetMenu(menuName = "Game Event")]
    public class GameEvent : ScriptableObject
    {
        #region FIELDS
        public List<GameEventListener> listeners = new List<GameEventListener>();
        #endregion

        /// <summary>
        /// Raise the event
        /// </summary>
        public void Raise(Component sender, object data)
        {
            // Loop through all of the listeners and invoke their responses
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnEventRaised(sender, data);
            }
        }

        /// <summary>
        /// Register a listener to the Game Event
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterListener(GameEventListener listener)
        {
            // Check if the list already contains the listener
            if (!listeners.Contains(listener))
            {
                // If it doesn't, add the listener to the list
                listeners.Add(listener);
            }
        }

        /// <summary>
        /// Unregister a listener from the Game Event
        /// </summary>
        /// <param name="listener"></param>
        public void UnregisterListener(GameEventListener listener)
        {
            // Check if the list contains the listener
            if (listeners.Contains(listener))
            {
                // If it does, remove the listener from the list
                listeners.Remove(listener);
            }
        }
    }
}