using GoodLuckValley.Entity;
using GoodLuckValley.Player.Input;
using GoodLuckValley.World.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.Handlers
{
    public class InteractableHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader input;

        [Header("Fields")]
        [SerializeField] public bool canInteract;
        private IInteractable interactable;

        private void OnEnable()
        {
            input.Interact += OnInteract;
        }

        private void OnDisable()
        {
            input.Interact -= OnInteract;
        }

        /// <summary>
        /// Interact with the current interactable
        /// </summary>
        /// <param name="started"></param>
        public void OnInteract(bool started)
        {
            if(started)
            {
                interactable?.ExecuteCommand();
            }
        }

        /// <summary>
        ///  Set an interactable for the player to handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OnSetInteractable(Component sender, object data)
        {
            // Check if the data is null
            if (data is null)
            {
                // Set the interactable to null
                interactable = null;
            } else if(data is IInteractable)
            {
                // If not, cast the data
                IInteractable interactable = (IInteractable)data;

                // Set the data
                this.interactable = interactable;
            }

            // Set whether or not the player can interact
            canInteract = interactable != null;
        }
    }
}