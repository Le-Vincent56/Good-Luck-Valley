using GoodLuckValley.World.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.Handlers
{
    public class InteractableHandler : MonoBehaviour
    {
        #region FIELDS
        private IInteractable interactable;
        #endregion

        /// <summary>
        /// Set the current Interactable for the Player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void SetInteractable(Component sender, object data)
        {
            // Check if the data type is correct
            if (data is not IInteractable || data is null) return;

            // Cast the data
            IInteractable sentInteractable = (IInteractable)data;

            // Set the interactable
            interactable = sentInteractable;
        }

        /// <summary>
        /// Interact with the current Interactable, if not null
        /// </summary>
        public void Interact(Component sender, object data) => interactable?.Interact();
    }
}