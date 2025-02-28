using GoodLuckValley.Events.Fireflies;
using GoodLuckValley.Events;
using UnityEngine;
using System.Collections.Generic;

namespace GoodLuckValley.Cameras.Triggers
{
    public class CameraLanternTriggerSetter : MonoBehaviour
    {
        [Header("References")]
        private List<TwoWayActivateableCameraTrigger> triggers;

        [Header("Fields")]
        [SerializeField] private int channel;

        private EventBinding<ActivateLantern> onActivateLantern;

        private void Awake()
        {
            // Get all of the triggers
            triggers = new List<TwoWayActivateableCameraTrigger>();
            GetComponents(triggers);
        }

        private void OnEnable()
        {
            onActivateLantern = new EventBinding<ActivateLantern>(EnableTrigger);
            EventBus<ActivateLantern>.Register(onActivateLantern);
        }

        private void OnDisable()
        {
            EventBus<ActivateLantern>.Deregister(onActivateLantern);
        }

        /// <summary>
        /// Enable the Trigger
        /// </summary>
        private void EnableTrigger(ActivateLantern eventData)
        {
            // Exit case - the channel doesn't match
            if (eventData.Channel != channel) return;

            // Iterate through each trigger
            foreach(TwoWayActivateableCameraTrigger trigger in triggers)
            {
                // Activate the trigger
                trigger.Activate();
            }
        }
    }
}
