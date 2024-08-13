using GoodLuckValley.World.AreaTriggers;
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
        [SerializeField] private GameEvent onSetInteractable;

        [Header("References")]
        [SerializeField] protected Transform fireflies;
        [SerializeField] protected AreaCollider areaCollider;

        [Header("Fields")]
        [SerializeField] protected int channel;

        readonly Queue<ICommand<IInteractable>> commandQueue = new Queue<ICommand<IInteractable>>();

        public bool HasFireflies => fireflies != null;

        private void Awake()
        {
            areaCollider = GetComponent<AreaCollider>();
        }

        private void OnEnable()
        {
            areaCollider.OnTriggerEnter += EnterArea;
            areaCollider.OnTriggerExit += ExitArea;
        }

        private void OnDisable()
        {
            areaCollider.OnTriggerEnter -= EnterArea;
            areaCollider.OnTriggerExit -= ExitArea;
        }

        public void EnterArea(GameObject gameObject) => onSetInteractable.Raise(this, this);

        public void ExitArea(GameObject gameObject) => onSetInteractable.Raise(this, null);

        /// <summary>
        /// Queue a Command for the Collectible
        /// </summary>
        /// <param name="command"></param>
        public void QueueCommand(ICommand<IInteractable> command) => commandQueue.Enqueue(command);

        /// <summary>
        /// Execute the interact command
        /// </summary>
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

        /// <summary>
        /// Interact logic for the Lantern
        /// </summary>
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

        /// <summary>
        /// Withdraw the fireflies by seting the stored transform to null
        /// </summary>
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