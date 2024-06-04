using GoodLuckValley.Entity;
using GoodLuckValley.Mushroom;
using GoodLuckValley.Patterns.Commands;
using GoodLuckValley.Patterns.Observer;
using GoodLuckValley.Patterns.Visitor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoodLuckValley.World.Decomposables
{
    public class DecomposableController : MonoBehaviour, IVisitor
    {
        [Header("References")]
        [SerializeField] private StaticCollisionHandler collisionHandler;
        [SerializeField] public List<IDecomposable> decomposables = new List<IDecomposable>();

        [Header("Fields")]
        [SerializeField] private bool regrow = true;
        [SerializeField] private bool attachedMushroom;
        [SerializeField] private float shroomSapTime;
        readonly Queue<ICommand<IDecomposable>> commandQueue = new Queue<ICommand<IDecomposable>>();
        private Vector2 velocity;

        private void Awake()
        {
            collisionHandler = GetComponent<StaticCollisionHandler>();
            decomposables = GetComponentsInChildren<IDecomposable>().ToList();
        }

        private void FixedUpdate()
        {
            // Update collisions
            HandleCollisions();

            // Check if there's mushroom collision
            if((collisionHandler.collisions.Left ||  collisionHandler.collisions.Right ||
                collisionHandler.collisions.Above || collisionHandler.collisions.Below) &&
                !attachedMushroom && regrow)
            {
                // If so, set attached mushroom to true
                attachedMushroom = true;

                DecomposeAll();

                // Have the shroom timer accept this as a visitor
                collisionHandler.collisions.Entity.GetComponent<ShroomTimer>().Accept(this);
            }

            // Check if a collision has been removed
            if(attachedMushroom && !(collisionHandler.collisions.Left || collisionHandler.collisions.Right ||
                collisionHandler.collisions.Above || collisionHandler.collisions.Below) && regrow)
            {
                // Set attached mushroom to false
                attachedMushroom = false;

                RecomposeAll();
            }
        }

        /// <summary>
        /// Queue a Command for the Decomposable Controller
        /// </summary>
        /// <param name="command"></param>
        public void QueueCommand(ICommand<IDecomposable> command) => commandQueue.Enqueue(command);

        /// <summary>
        /// Execute the command for the Decomposable Controller
        /// </summary>
        public void ExecuteCommand()
        {
            if (commandQueue.Count > 0)
            {
                commandQueue.Dequeue()?.Execute();
            }
        }

        private void DecomposeAll()
        {
            // Create a new decompose command
            DecomposeCommand newCommand = new DecomposeCommand.Builder(decomposables)
                .WithAction(Decompose)
                .Build();

            // Queue the command
            QueueCommand(newCommand);

            // Execute the command
            ExecuteCommand();
        }

        private void RecomposeAll()
        {
            // If the vines are not to regrow, return
            if (!regrow) return;

            // Create a new recompose command
            DecomposeCommand newCommand = new DecomposeCommand.Builder(decomposables)
                .WithAction(Recompose)
                .Build();

            // Queue the command
            QueueCommand(newCommand);

            // Execute the command
            ExecuteCommand();
        }

        /// <summary>
        /// Decompose a decomposable
        /// </summary>
        /// <param name="decomposable"></param>
        private void Decompose(IDecomposable decomposable) => decomposable.Decompose();

        /// <summary>
        /// Recompose a decomposable
        /// </summary>
        /// <param name="decomposable"></param>
        private void Recompose(IDecomposable decomposable) => decomposable.Recompose();

        public void UpdateRegrow(bool plucked)
        {
            // If the brain is plucked, do not regrow
            regrow = !plucked;

            if(!regrow && !attachedMushroom)
            {
                DecomposeAll();
            }
        }

        /// <summary>
        /// Handle static collisions for the Decomposable Controller
        /// </summary>
        /// <param name="standingOnPlatform"></param>
        public void HandleCollisions(bool standingOnPlatform = false)
        {
            // Update raycasts
            collisionHandler.UpdateRaycastOrigins();

            // Reset collisions
            collisionHandler.collisions.ResetInfo();

            // Set the old velocity
            collisionHandler.collisions.PrevVelocity = velocity;

            // Handle collisions
            collisionHandler.HandleCollisions(ref velocity);
        }

        /// <summary>
        /// Visit the ShroomTimer and set the sap time
        /// </summary>
        /// <param name="shroomTimer">The ShroomTimer to set the sap time to</param>
        public void Visit(ShroomTimer shroomTimer)
        {
            // Set teh duration of the shroom timer
            shroomTimer.SetDuration(shroomSapTime);
        }
    }
}