using GoodLuckValley.Patterns.ServiceLocator;
using UnityEngine;

namespace GoodLuckValley.World.Physics
{
    public class PhysicsOrchestrator : MonoBehaviour
    {
        private IPhysicsObject player;
        private float time;

        private void Awake()
        {
            // Register the PhysicsOrchestrator as a service
            ServiceLocator.ForSceneOf(this).Register(this);
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            time += delta;

            // Update the player
            player.TickUpdate(delta, time);
        }

        private void FixedUpdate()
        {
            float delta = Time.deltaTime;

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
    }
}
