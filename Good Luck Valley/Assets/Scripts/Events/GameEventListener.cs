using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GoodLuckValley.Events
{
    [System.Serializable]
    public class CustomGameEvent : UnityEvent<Component, object> { }

    public class GameEventListener : MonoBehaviour
    {
        #region FIELDS
        public GameEvent gameEvent;
        public CustomGameEvent response;
        #endregion

        /// <summary>
        /// Register to the GameEvent during OnEnable()
        /// </summary>
        private void OnEnable()
        {
            gameEvent.RegisterListener(this);
        }

        /// <summary>
        /// Unregister to the GameEvent during OnDisable()
        /// </summary>
        private void OnDisable()
        {
            gameEvent.UnregisterListener(this);
        }

        /// <summary>
        /// Invoke the response when the GameEvent is raised
        /// </summary>
        public void OnEventRaised(Component sender, object data)
        {
            response.Invoke(sender, data);
        }
    }
}
