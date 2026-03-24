using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;
using GoodLuckValley.Gameplay.Player.Motor;

namespace GoodLuckValley.Gameplay.Player.States
{
    /// <summary>
    /// Represents a player state that handles the behavior during a bounce action.
    /// This state is typically triggered after a player performs an action, such as a jump
    /// or collision, that results in a bouncing motion.
    /// </summary>
    public class BounceState : IPlayerState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IPlayerState> _fsm;

        public BounceState(
            IPlayerContext context, 
            IStateMachineContext<IPlayerState> fsm
        )
        {
            _context = context;
            _fsm = fsm;
        }

        /// <summary>
        /// Executes the initialization logic for the state when it becomes active.
        /// This method is responsible for setting up the context and applying necessary
        /// configurations or actions to prepare the state for its associated behaviors.
        /// </summary>
        public void Enter()
        {
            _context.Motor.SetGravityScale(_context.Stats.BounceGravityScale);
            _context.Motor.SetMovementMode(MovementMode.Airborne);
            _context.Motor.ClearMaxFallSpeed();
            _context.SetColliderMode(ColliderMode.Airborne);
            _context.Bounce.ExecuteBounce();
        }

        /// <summary>
        /// Handles the exit logic for the state when transitioning to another state or when the state ends.
        /// This method is invoked during the lifecycle management of the state to perform any necessary
        /// cleanup or actions required before leaving the state.
        /// </summary>
        public void Exit() { }

        /// <summary>
        /// Updates the state with logic that should occur during a standard update cycle.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last Update, used for frame-dependent calculations.</param>
        public void Update(float deltaTime) { }

        /// <summary>
        /// Handles logic that should occur during the fixed update step of the physics engine.
        /// </summary>
        /// <param name="fixedDeltaTime">The time elapsed since the last FixedUpdate, used for consistent physics calculations.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            // Wall
            if (_context.DetectedWallDirection != 0 
                && _context.Wall.ShouldAttachToWall(_context.Collision, _context.Input.Move, false)
               )
            {
                _context.Wall.AttachToWall(_context.DetectedWallDirection);
                _fsm.ChangeState<WallState>();
                return;
            }

            // Falling
            if (_context.CurrentVelocity.y <= 0f)
            {
                _fsm.ChangeState<FallState>();
                return;
            }
        }
    }
}