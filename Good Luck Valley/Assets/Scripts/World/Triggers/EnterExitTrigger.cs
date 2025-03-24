using UnityEngine;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.World.Triggers
{
    public abstract class EnterExitTrigger : BaseTrigger
    {
        private bool inTrigger;

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the collision object is not a PlayerController
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Handle the enter function
            OnEnter(controller);

            // Set to inside the trigger
            inTrigger = true;
        }

        protected void OnTriggerStay2D(Collider2D collision)
        {
            // Exit case - if inside the trigger
            if (inTrigger) return;

            // Exit case - the collision object is not a PlayerController
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Handle the enter function
            OnEnter(controller);

            // Set to inside the trigger
            inTrigger = true;

        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - the collision object is not a PlayerController
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Handle the exit function
            OnExit(controller);

            // Set to outside the trigger
            inTrigger = false;
        }

        /// <summary>
        /// Called when the player enters the trigger
        /// </summary>
        public abstract void OnEnter(PlayerController controller);

        /// <summary>
        /// Called when the player exits the trigger
        /// </summary>
        public abstract void OnExit(PlayerController controller);
    }
}
