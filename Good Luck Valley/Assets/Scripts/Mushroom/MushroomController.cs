using GoodLuckValley.Entity;
using GoodLuckValley.Events;
using GoodLuckValley.Mushroom.States;
using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomController : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onShroomEnter;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private StaticCollisionHandler collisionHandler;
        [SerializeField] private MushroomInfo data;

        [Header("Fields")]
        [SerializeField] private ShroomType shroomType;
        [SerializeField] private Vector2 velocity;
        [SerializeField] private bool bounceEntity;

        private StateMachine stateMachine;

        private void Awake()
        {
            // Get components
            animator = GetComponent<Animator>();
            collisionHandler = GetComponent<StaticCollisionHandler>();
            data = GetComponent<MushroomInfo>();

            // Declare states
            stateMachine = new StateMachine();

            switch(shroomType)
            {
                case ShroomType.Regular:
                    IdleState idleState = new IdleState(this, animator);
                    BounceState bounceState = new BounceState(this, animator);

                    // Define strict transitions
                    At(idleState, bounceState, new FuncPredicate(() => bounceEntity));
                    At(bounceState, idleState, new FuncPredicate(() => !bounceEntity));

                    // Set the initial state
                    stateMachine.SetState(idleState);
                    break;

                case ShroomType.Wall:
                    WallIdleState wallIdleState = new WallIdleState(this, animator);

                    At(wallIdleState, wallIdleState, new FuncPredicate(() => bounceEntity));

                    stateMachine.SetState(wallIdleState);
                    break;
            }
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

        public void CheckCollisions()
        {
            // Check if there's a collision
            if(collisionHandler.collisions.Above || collisionHandler.collisions.Below ||
                collisionHandler.collisions.Left || collisionHandler.collisions.Right)
            {
                // Bounce the entity
                bounceEntity = true;
                onShroomEnter?.Raise(this, data);
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

        public void ResetBounce()
        {
            bounceEntity = false;
        }
    }
}