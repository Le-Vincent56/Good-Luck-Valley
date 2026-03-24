using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;
using GoodLuckValley.Gameplay.Player.Motor;

namespace GoodLuckValley.Gameplay.Player.States
{
    /// <summary>
    /// Represents the jump state for a player in the game state machine,
    /// handling the behavior specific to when a player is in the jumping state.
    /// </summary>
    public class JumpState : IPlayerState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IPlayerState> _fsm;

        public JumpState(
            IPlayerContext context, 
            IStateMachineContext<IPlayerState> fsm
        )
        {
            _context = context;
            _fsm = fsm;
        }

        /// <summary>
        /// Initializes the jump state by applying jump-specific adjustments such as setting gravity scale,
        /// enabling airborne movement mode, clearing restrictions on maximum fall speed,
        /// and adjusting the player's collider settings for airborne behavior.
        /// </summary>
        public void Enter()
        {
            _context.Motor.SetGravityScale(_context.Stats.JumpGravityScale);
            _context.Motor.SetMovementMode(MovementMode.Airborne);
            _context.Motor.ClearMaxFallSpeed();
            _context.SetColliderMode(ColliderMode.Airborne);
        }

        /// <summary>
        /// Exits the jump state, performing cleanup by resetting state-specific modifications such as end jump early multiplier and wall velocity overrides.
        /// </summary>
        public void Exit()
        {
            _context.Motor.ClearEndJumpEarlyMultiplier();
            _context.Motor.ClearWallVelocityOverride();
        }

        /// <summary>
        /// Updates the player's jump state, handling frame-based logic that occurs during the normal update step.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last Update frame, used for frame-rate independent calculations.</param>
        public void Update(float deltaTime)
        {
        }

        /// <summary>
        /// Handles logic to update the player's jump state during the physics simulation step.
        /// </summary>
        /// <param name="fixedDeltaTime">The time elapsed since the last FixedUpdate frame, used for frame-rate independent operations.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            // End jump early detection
            if (!_context.Input.JumpHeld)
                _context.Jump.NotifyJumpReleased();

            if (_context.Jump.EndedJumpEarly)
                _context.Motor.SetEndJumpEarlyMultiplier(_context.Stats.EndJumpEarlyExtraForceMultiplier);

            // Bounce takes priority
            if (_context.Bounce.BouncePrepped)
            {
                _fsm.ChangeState<BounceState>();
                return;
            }

            // Falling
            if (_context.CurrentVelocity.y <= 0f)
            {
                _fsm.ChangeState<FallState>();
                return;
            }

            // Wall
            if (_context.DetectedWallDirection != 0 
                && _context.Wall.ShouldAttachToWall(_context.Collision, _context.Input.Move, false)
            )
            {
                _context.Wall.AttachToWall(_context.DetectedWallDirection);
                _fsm.ChangeState<WallState>();
                return;
            }
        }
    }
}