using GoodLuckValley.Persistence;
using GoodLuckValley.Patterns.Commands;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Interactables
{
    public class Collectible : MonoBehaviour, IInteractable, IBind<CollectibleSaveData>
    {
        #region REFERENCES
        [SerializeField] protected CollectibleSaveData data;
        #endregion

        #region FIELDS
        readonly Queue<ICommand<IInteractable>> commandQueue = new Queue<ICommand<IInteractable>>();
        [SerializeField] protected bool isCollected;
        #endregion

        #region PROPERTIES
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        #endregion

        private void Start()
        {
            // Check if the Collectible should be active
            CheckActive();

            // Create and enqueue the interactable command
            InteractableCommand newCommand = new InteractableCommand.Builder(new List<IInteractable> { this })
                .WithAction(_ => Interact())
                .Build();
            QueueCommand(newCommand);
        }

        /// <summary>
        /// Queue a Command for the Collectible
        /// </summary>
        /// <param name="command"></param>
        public void QueueCommand(ICommand<IInteractable> command) => commandQueue.Enqueue(command);
        
        /// <summary>
        /// Execute the command for hte interactable
        /// </summary>
        public void ExecuteCommand()
        {
            if(commandQueue.Count > 0)
            {
                commandQueue.Dequeue()?.Execute();
            }
        }

        /// <summary>
        /// Interactable with the collectible
        /// </summary>
        public virtual void Interact()
        {
            isCollected = true;
            data.collected = true;

            CheckActive();
        }

        /// <summary>
        /// Check if the Collectible should be active
        /// </summary>
        public void CheckActive()
        {
            if (isCollected) gameObject.SetActive(false);
            else gameObject.SetActive(true);
        }

        /// <summary>
        /// Bind the Collectible for persistence
        /// </summary>
        /// <param name="data"></param>
        public void Bind(CollectibleSaveData data)
        {
            this.data = data;
            this.data.ID = ID;

            // Set collected data
            isCollected = data.collected;
        }
    }
}