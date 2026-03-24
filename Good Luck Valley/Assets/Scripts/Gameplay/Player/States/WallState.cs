using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.States
{
    /// <summary>
    /// Represents the state of the player when interacting with a wall, such as during a wall slide or wall cling.
    /// This state manipulates player physics, such as gravity and fall speed, and configures collision behavior
    /// to adapt to wall-based interactions. The transition into this state can originate from various player actions
    /// like jumping or falling.
    /// </summary>
    public class WallState : IPlayerState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IPlayerState> _fsm;

        public WallState(
            IPlayerContext context, 
            IStateMachineContext<IPlayerState> fsm
        )
        {
            _context = context;
            _fsm = fsm;
        }

        /// <summary>
        /// Configures the player's state upon entering the wall interaction. This includes adjusting
        /// gravity to enable a controlled wall slide, limiting maximum fall speed, and setting the appropriate
        /// collider mode to reflect airborne behavior.
        /// </summary>
        public void Enter()
        {
            _context.Motor.SetGravityScale(_context.Stats.WallSlideGravityScale);
            _context.Motor.SetMaxFallSpeed(_context.Stats.WallSlideMaxSpeed);
            _context.SetColliderMode(ColliderMode.Airborne);
        }

        /// <summary>
        /// Performs necessary operations to cleanly exit the wall state, ensuring that any state-specific overrides
        /// or attachments related to wall interactions are cleared and reset.
        /// </summary>
        public void Exit()
        {
            _context.Motor.ClearWallVelocityOverride();
            _context.Wall.DetachFromWall();
        }

        /// <summary>
        /// Handles the update logic for the wall state, including processing frame-dependent behavior
        /// such as player inputs, state transitions, and movement dynamics.
        /// </summary>
        /// <param name="deltaTime">The time increment between the current and previous frame.</param>
        public void Update(float deltaTime) { }

        /// <summary>
        /// Handles fixed update logic for the wall state, including wall movement velocity,
        /// transitioning out of the wall state, or handling wall jumps.
        /// </summary>
        /// <param name="fixedDeltaTime">The fixed time increment used for physics calculations.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            // Wall velocity calculation
            float wallY = _context.Wall.CalculateWallVelocity(
                _context.Input.Move, 
                _context.CurrentVelocity.y, 
                fixedDeltaTime
            );
            _context.Motor.SetWallVelocityOverride(wallY);

            // Grounded
            if (_context.IsGrounded)
            {
                _context.Jump.NotifyGrounded();
                _context.Wall.NotifyGrounded();
                _context.Bounce.ResetBounce();
                _fsm.ChangeState<GroundedState>();
                return;
            }

            // Jump (wall jump)
            if (_context.Input.JumpPressed)
            {
                _context.Wall.ExecuteWallJump();
                _context.Jump.NotifyJumpExecuted(JumpType.Wall);
                _fsm.ChangeState<JumpState>();
                return;
            }

            // Wall lost
            if (!_context.Wall.IsOnWall && !_context.Wall.HasWallCoyoteTime)
            {
                _fsm.ChangeState<FallState>();
                return;
            }
        }
    }
}