using GoodLuckValley.Architecture.ServiceLocator;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Physics
{
    public class PhysicsOrchestrator : SerializedMonoBehaviour
    {
        [Header("Physics Objects")]
        [SerializeField] private IPhysicsObject player;
        [SerializeField] private HashSet<IPhysicsObject> physicsObjects;
        private float time;

        private void Awake()
        {
            // Register the PhysicsOrchestrator as a service
            ServiceLocator.ForSceneOf(this).Register(this);

            // Initialize the list
            physicsObjects = new HashSet<IPhysicsObject>();
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            time += delta;

            // Update the physics objects
            foreach (IPhysicsObject physicsObject in physicsObjects)
            {
                physicsObject.TickUpdate(delta, time);
            }

            // Update the player
            player.TickUpdate(delta, time);
        }

        private void FixedUpdate()
        {
            float delta = Time.deltaTime;

            // Update the physics objects
            foreach (IPhysicsObject physicsObject in physicsObjects)
            {
                physicsObject.TickFixedUpdate(delta);
            }

            // Update the player
            player.TickFixedUpdate(delta);
        }

        /// <summary>
        /// Set the Player
        /// </summary>
        public void SetPlayer(IPhysicsObject player) => this.player = player;

        /// <summary>
        /// Remove the Player
        /// </summary>
        public void RemovePlayer() => player = null;

        /// <summary>
        /// Regeister a Physics Object 
        /// </summary>
        public void Register(IPhysicsObject physicsObject) => physicsObjects.Add(physicsObject);

        /// <summary>
        /// Deregister a Physics Object
        /// </summary>
        public void Deregister(IPhysicsObject physicsObject)
        {
            // Exit case - the Physics Object is not in the list
            if (!physicsObjects.Contains(physicsObject)) return;

            // Remove the Physics Object
            physicsObjects.Remove(physicsObject);
        }
    }
}
