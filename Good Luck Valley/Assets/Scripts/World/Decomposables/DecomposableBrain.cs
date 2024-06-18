using GoodLuckValley.Patterns.Commands;
using GoodLuckValley.Patterns.Observer;
using GoodLuckValley.Persistence;
using GoodLuckValley.World.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Decomposables
{
    public class DecomposableBrain : MonoBehaviour, IInteractable
    {
        [Header("Fields")]
        public Observer<bool> Plucked = new Observer<bool>(false);
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        readonly Queue<ICommand<IInteractable>> commandQueue = new Queue<ICommand<IInteractable>>();

        private void Start()
        {
            // Check if the brain should be active
            CheckActive();

            // Enqueue the command
            if(gameObject.activeSelf)
            {
                InteractableCommand pluckCommand = new InteractableCommand.Builder(new List<IInteractable> { this })
                    .WithAction(_ => Interact())
                    .Build();
                QueueCommand(pluckCommand);
            }
        }

        /// <summary>
        /// Queue a Command for the Collectible
        /// </summary>
        /// <param name="command"></param>
        public void QueueCommand(ICommand<IInteractable> command) => commandQueue.Enqueue(command);

        /// <summary>
        /// Execute the command for the interactable
        /// </summary>
        public void ExecuteCommand()
        {
            if (commandQueue.Count > 0)
            {
                commandQueue.Dequeue()?.Execute();
            }
        }

        /// <summary>
        /// Interact with the brain
        /// </summary>
        public void Interact()
        {
            // Update the observer
            Plucked.Value = true;

            // Check if the brain should be active
            CheckActive();
        }

        /// <summary>
        /// Check if the brain should be active
        /// </summary>
        public void CheckActive()
        {
            // Check if the brain has been plucked
            if (Plucked.Value) 
            {
                // Dispose of the plucked observer
                Plucked.Dispose();

                // De-activate the game object
                gameObject.SetActive(false);
            } else
            {
                // Activate the game object
                gameObject.SetActive(true);
            }
            
        }
    }
}