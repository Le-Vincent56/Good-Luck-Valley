using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Commands;
using GoodLuckValley.World.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal.Collectibles
{
    public class Note : Collectible
    {
        #region EVENTS
        [SerializeField] private GameEvent onAddNote;
        #endregion

        public override void Interact()
        {
            if(!Journal.Instance.Unlocked)
            {
                Debug.Log("Cannot collected because the Journal has not yet been unlocked");

                // Create and enqueue the interactable command
                InteractableCommand newCommand = new InteractableCommand.Builder(new List<IInteractable> { this })
                    .WithAction(_ => Interact())
                    .Build();
                QueueCommand(newCommand);
                return;
            }

            base.Interact();

            // Unlock the next journal note
            Journal.Instance.GetNextNote();
        }
    }
}