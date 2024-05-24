using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Interactables
{
    public interface IInteractable
    {
        /// <summary>
        /// Interact with the interactable
        /// </summary>
        public void Interact();
    }
}