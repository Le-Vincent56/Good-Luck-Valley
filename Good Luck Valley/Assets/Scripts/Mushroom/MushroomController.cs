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
        [SerializeField] private MushroomSFXHandler sfxHandler;

        [Header("Fields")]
        [SerializeField] private ShroomType shroomType;
        [SerializeField] private Vector2 velocity;
        [SerializeField] private bool bounceEntity;
        [SerializeField] private bool growing;

        private StateMachine stateMachine;

        private void Awake()
        {
            // Get components
            animator = GetComponent<Animator>();
            collisionHandler = GetComponent<StaticCollisionHandler>();
            data = GetComponent<MushroomInfo>();
            sfxHandler = GetComponent<MushroomSFXHandler>();

            // Set variables
            growing = true;

            // Declare states
            stateMachine = new StateMachine();

            switch(shroomType)
            {
                case ShroomType.Regular:
                    GrowState growState = new GrowState(this, animator, sfxHandler);
                    IdleState idleState = new IdleState(this, animator);
                    BounceState bounceState = new BounceState(this, animator, sfxHandler);

                    // Define strict transitions
                    At(growState, idleState, new FuncPredicate(() => !growing));
                    At(growState, bounceState, new FuncPredicate(() => bounceEntity));
                    At(idleState, bounceState, new FuncPredicate(() => bounceEntity));
                    At(bounceState, idleState, new FuncPredicate(() => !bounceEntity));

                    // Set the initial state
                    stateMachine.SetState(growState);
                    break;

                case ShroomType.Quick:
                    GrowState quickGrowState = new GrowState(this, animator, sfxHandler);
                    IdleState quickIdleState = new IdleState(this, animator);
                    BounceState quickBounceState = new BounceState(this, animator, sfxHandler);

                    // Define strict transitions
                    At(quickGrowState, quickIdleState, new FuncPredicate(() => !growing));
                    At(quickGrowState, quickBounceState, new FuncPredicate(() => bounceEntity));
                    At(quickIdleState, quickBounceState, new FuncPredicate(() => bounceEntity));
                    At(quickBounceState, quickIdleState, new FuncPredicate(() => !bounceEntity));

                    // Set the initial state
                    stateMachine.SetState(quickGrowState);
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

        /// <summary>
        /// Stop the growing of the mushroom
        /// </summary>
        public void StopGrowing() => growing = false;

        /// <summary>
        /// Reset the bounce
        /// </summary>
        public void ResetBounce() => bounceEntity = false;
    }
}