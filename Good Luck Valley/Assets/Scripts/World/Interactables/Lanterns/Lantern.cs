using GoodLuckValley.Entities.Fireflies;
using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Commands;
using GoodLuckValley.Patterns.Visitor;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Interactables
{
    public class Lantern : MonoBehaviour, IInteractable, IVisitable
    {
        [Header("Events")]
        [SerializeField] private GameEvent onStoreFireflies;
        [SerializeField] private GameEvent onWithdrawFireflies;

        [Header("References")]
        [SerializeField] protected Transform fireflies;

        [Header("Fields")]
        [SerializeField] protected int channel;

        readonly Queue<ICommand<IInteractable>> commandQueue = new Queue<ICommand<IInteractable>>();

        public bool HasFireflies => fireflies != null;

        /// <summary>
        /// Queue a Command for the Collectible
        /// </summary>
        /// <param name="command"></param>
        public void QueueCommand(ICommand<IInteractable> command) => commandQueue.Enqueue(command);

        public void ExecuteCommand()
        {
            // Create and enqueue the interactable command
            InteractableCommand newCommand = new InteractableCommand.Builder(new List<IInteractable> { this })
                .WithAction(_ => Interact())
                .Build();

            // Queue the command
            QueueCommand(newCommand);

            // Execute the command
            if(commandQueue.Count > 0)
            {
                commandQueue.Dequeue()?.Execute();
            }
        }

        public void Interact()
        {
            if(HasFireflies)
            {
                // Withdraw the fireflies
                // Calls to:
                //  - FireflyController.OnWithdrawFireflies();
                onWithdrawFireflies.Raise(this, channel);
            } else
            {
                // Store the fireflies
                // Calls to:
                //  - FireflyController.OnStoreFireflies();
                onStoreFireflies.Raise(this, channel);
            }
        }

        public void Withdraw()
        {
            fireflies = null;
        }

        /// <summary>
        /// Accept the fireflies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        public virtual void Accept<T>(T visitor) where T : Component, IVisitor
        {
            // Verify the correct visitor
            if(visitor is FireflyController controller)
            {
                // Set the fireflies
                fireflies = controller.gameObject.transform;
            }
        }
    }
}