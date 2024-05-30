using GoodLuckValley.Entity;
using GoodLuckValley.Mushroom.States;
using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private CollisionHandler collisionHandler;

        [Header("Fields")]
        [SerializeField] private Vector2 velocity;
        [SerializeField] private bool bounceEntity;

        private StateMachine stateMachine;

        private void Awake()
        {
            // Get components
            animator = GetComponent<Animator>();
            collisionHandler = GetComponent<CollisionHandler>();

            // Declare states
            stateMachine = new StateMachine();
            IdleState idleState = new IdleState(this, animator);
            BounceState bounceState = new BounceState(this, animator);

            // Define strict transitions
            At(idleState, bounceState, new FuncPredicate(() => bounceEntity));
            At(bounceState, idleState, new FuncPredicate(() => !bounceEntity && bounceState.Finished));

            // Set the initial state
            stateMachine.SetState(idleState);
        }

        private void Update()
        {
            // Update the state machine
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            // Update the state machine
            stateMachine.FixedUpdate();
        }

        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        public void CheckCollisions()
        {
            // Check if there's a collision
            if(collisionHandler.collisions.Above || collisionHandler.collisions.Below ||
                collisionHandler.collisions.Left || collisionHandler.collisions.Right)
            {
                // Bounce the entity
                bounceEntity = true;
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

            // Handle horizontal collisions
            collisionHandler.HandleAllCollisionsForStatic();

            // Handle platforms
            if (standingOnPlatform)
            {
                collisionHandler.collisions.Below = true;
            }
        }

        public void ResetBounce()
        {
            bounceEntity = false;
        }
    }
}