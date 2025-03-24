using GoodLuckValley.Persistence;
using UnityEngine;

namespace GoodLuckValley
{
    public class ActivateableTrigger : MonoBehaviour, IBind<ActivateableTriggerData>
    {
        protected ActivateableTriggerData data;
        [SerializeField] protected bool active;

        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        /// <summary>
        /// Activate the trigger
        /// </summary>
        public void Activate()
        {
            // Set active
            active = true;

            // Exit case - the data does not exist
            if (data == null) return;

            // Save the activate state
            data.Active = true;
        }

        /// <summary>
        /// Bind the Activateable Trigger's data
        /// </summary>
        public void Bind(ActivateableTriggerData data)
        {
            // Bind the data
            this.data = data;
            this.data.ID = ID;

            // Set active
            active = data.Active;

            // Exit case - the trigger is not active
            if (!active) return;

            // Activate the trigger
            Activate();
        }
    }
}
