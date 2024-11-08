using GoodLuckValley.Audio.SFX;
using GoodLuckValley.Entity;
using GoodLuckValley.Events;
using GoodLuckValley.Mushroom.States;
using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.World.Tiles;
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
        [SerializeField] private MushroomSFXMaster sfxHandler;
        [SerializeField] private DetectTileType detectTileType;

        [Header("Fields")]
        [SerializeField] private ShroomType shroomType;
        [SerializeField] private TileType spawnTile;
        [SerializeField] private Vector2 velocity;
        [SerializeField] private bool bounceEntity;
        [SerializeField] private bool growing;
        [SerializeField] private bool dissipating;

        private StateMachine stateMachine;

        private void Awake()
        {
            // Get components
            animator = GetComponentInChildren<Animator>();
            collisionHandler = GetComponent<StaticCollisionHandler>();
            data = GetComponent<MushroomInfo>();
            sfxHandler = GetComponent<MushroomSFXMaster>();

            // Set variables
            growing = true;

            // Find tile type
            detectTileType.RaycastStart = transform.position;
            spawnTile = detectTileType.CheckTileType();

            // Declare states
            stateMachine = new StateMachine();

            switch(shroomType)
            {
                case ShroomType.Regular:
                    GrowState growState = new GrowState(this, animator, sfxHandler);
                    IdleState idleState = new IdleState(this, animator);
                    BounceState bounceState = new BounceState(this, animator);
                    DissipateState dissipateState = new DissipateState(this, animator, sfxHandler);

                    // Define strict transitions
                    At(growState, idleState, new FuncPredicate(() => !growing));
                    At(growState, bounceState, new FuncPredicate(() => bounceEntity));
                    At(idleState, bounceState, new FuncPredicate(() => bounceEntity));
                    At(bounceState, idleState, new FuncPredicate(() => !bounceEntity));
                    Any(dissipateState, new FuncPredicate(() => dissipating));

                    // Set the initial state
                    stateMachine.SetState(growState);
                    break;

                case ShroomType.Quick:
                    GrowState quickGrowState = new GrowState(this, animator, sfxHandler);
                    IdleState quickIdleState = new IdleState(this, animator);
                    BounceState quickBounceState = new BounceState(this, animator);
                    DissipateState quickDissipateState = new DissipateState(this, animator, sfxHandler);

                    // Define strict transitions
                    At(quickGrowState, quickIdleState, new FuncPredicate(() => !growing));
                    At(quickGrowState, quickBounceState, new FuncPredicate(() => bounceEntity));
                    At(quickIdleState, quickBounceState, new FuncPredicate(() => bounceEntity));
                    At(quickBounceState, quickIdleState, new FuncPredicate(() => !bounceEntity));
                    Any(quickDissipateState, new FuncPredicate(() => dissipating));

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

        /// <summary>
        /// Add a transition from one State to another given a certain condition
        /// </summary>
        /// <param name="from">The State to define the transition from</param>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        /// <summary>
        /// Add a transition from any State to another one given a certain condition
        /// </summary>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the transition</param>
        private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

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
        /// Get the tile type of the spawn location
        /// </summary>
        /// <returns>The TileType of the spawn location</returns>
        public TileType GetSpawnTileType() => spawnTile;

        /// <summary>
        /// Stop the growing of the mushroom
        /// </summary>
        public void StopGrowing() => growing = false;

        /// <summary>
        /// Reset the bounce
        /// </summary>
        public void ResetBounce() => bounceEntity = false;

        public void Dissipate() => dissipating = true;
    }
}