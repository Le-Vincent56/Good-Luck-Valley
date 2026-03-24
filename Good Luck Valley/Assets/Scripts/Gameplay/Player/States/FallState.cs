using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;
using GoodLuckValley.Gameplay.Player.Motor;

namespace GoodLuckValley.Gameplay.Player.States
{
    /// <summary>
    /// Represents the state of the player when they are falling during gameplay.
    /// This state is part of the player's state machine and handles the behavior
    /// and transitions associated with the falling state.
    /// </summary>
    public class FallState : IPlayerState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IPlayerState> _fsm;

        public FallState(
            IPlayerContext context, 
            IStateMachineContext<IPlayerState> fsm
        )
        {
            _context = context;
            _fsm = fsm;
        }

        /// <summary>
        /// Called when entering the FallState. Configures the player's motor and other components
        /// to behave appropriately for a falling state, including setting gravity scale, maximum fall speed,
        /// movement mode, and collision mode.
        /// </summary>
        public void Enter()
        {
            _context.Motor.SetGravityScale(_context.Stats.FallGravityScale);
            _context.Motor.SetMaxFallSpeed(_context.Stats.FallingMaxSpeed);
            _context.Motor.SetMovementMode(MovementMode.Airborne);
            _context.Motor.ClearEndJumpEarlyMultiplier();
            _context.SetColliderMode(ColliderMode.Airborne);
        }

        /// <summary>
        /// Called when exiting the FallState. Used to perform any necessary cleanup or state transition logic.
        /// </summary>
        public void Exit() { }

        /// <summary>
        /// Executes logic in the update loop for the FallState.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        public void Update(float deltaTime) { }

        /// <summary>
        /// Executes logic in the physics update loop for the FallState.
        /// </summary>
        /// <param name="fixedDeltaTime">The time elapsed since the last physics update.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            // Fast fall toggle
            if (_context.Input.Move.y < -_context.Stats.VerticalDeadZoneThreshold)
                _context.Motor.SetGravityScale(_context.Stats.FallGravityScale * _context.Stats.FastFallMultiplier);
            else
                _context.Motor.SetGravityScale(_context.Stats.FallGravityScale);

            // Landing
            if (_context.IsGrounded)
            {
                _context.Jump.NotifyGrounded();
                _context.Wall.NotifyGrounded();
                _context.Bounce.ResetBounce();
                _fsm.ChangeState<GroundedState>();
                return;
            }

            // Steep slope
            if (_context.IsOnSteepSlope)
            {
                _fsm.ChangeState<SlideState>();
                return;
            }

            // Jump (coyote or air)
            if (_context.Input.JumpPressed)
            {
                if (_context.Jump.HasCoyoteTime)
                {
                    _context.Jump.ExecuteNormalJump();
                    _context.Jump.NotifyJumpExecuted(JumpType.Normal);
                    _fsm.ChangeState<JumpState>();
                    return;
                }

                if (_context.Jump.HasAirJump)
                {
                    _context.Jump.ExecuteAirJump();
                    _context.Jump.NotifyJumpExecuted(JumpType.Air);
                    _fsm.ChangeState<JumpState>();
                    return;
                }
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

            // Bounce
            if (_context.Bounce.BouncePrepped)
            {
                _fsm.ChangeState<BounceState>();
                return;
            }
        }
    }
}