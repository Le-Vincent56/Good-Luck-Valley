using GoodLuckValley.Entity;
using GoodLuckValley.Patterns.Observer;
using UnityEngine;

namespace GoodLuckValley.World.Decomposables
{
    public class DecomposableController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StaticCollisionHandler collisionHandler;

        [Header("Fields")]
        [SerializeField] private Observer<bool> attachedMushroom = new Observer<bool>(false);
        [SerializeField] private Vector2 velocity;

        private void Awake()
        {
            collisionHandler = GetComponent<StaticCollisionHandler>();
        }

        private void FixedUpdate()
        {
            HandleCollisions();

            if(collisionHandler.collisions.Left ||  collisionHandler.collisions.Right ||
                collisionHandler.collisions.Above || collisionHandler.collisions.Below)
            {
                Debug.Log("Yuurd");
            }
        }

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

            // Handle platforms
            if (standingOnPlatform)
            {
                collisionHandler.collisions.Below = true;
            }
        }
    }
}