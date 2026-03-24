using UnityEngine;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Core.StateMachine.Services;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;
using GoodLuckValley.Gameplay.Player.Motor;
using GoodLuckValley.Gameplay.Player.States.Substates;

namespace GoodLuckValley.Gameplay.Player.States
{
    /// <summary>
    /// Represents the grounded state of the player within the state machine.
    /// This state handles the player's grounded behavior, such as determining
    /// the appropriate sub-state (e.g., standing idle, running, crawling) based on player input.
    /// </summary>
    public class GroundedState : IPlayerState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IPlayerState> _fsm;
        private readonly StateMachine<IGroundSubState> _subFsm;
        private bool _subFsmStarted;

        public GroundedState(
            IPlayerContext context, 
            IStateMachineContext<IPlayerState> fsm
        )
        {
            _context = context;
            _fsm = fsm;

            _subFsm = new StateMachine<IGroundSubState>();
            IStateMachineContext<IGroundSubState> subFsmContext = _subFsm;

            _subFsm.AddState(new GroundIdleState(context, subFsmContext));
            _subFsm.AddState(new GroundRunState(context, subFsmContext));
            _subFsm.AddState(new CrawlIdleState(context, subFsmContext));
            _subFsm.AddState(new CrawlRunState(context, subFsmContext));
        }

        public void Enter()
        {
            _context.Motor.SetGravityScale(0f);
            _context.Motor.SetMovementMode(MovementMode.Grounded);
            _context.Motor.ClearMaxFallSpeed();
            _context.Motor.ClearEndJumpEarlyMultiplier();
            _context.Motor.ClearWallVelocityOverride();

            // Choose initial sub-state based on input
            if (!_subFsmStarted)
            {
                if (_context.Input.CrouchHeld)
                {
                    if (HasHorizontalInput())
                        _subFsm.Start<CrawlRunState>();
                    else
                        _subFsm.Start<CrawlIdleState>();
                }
                else
                {
                    if (HasHorizontalInput())
                        _subFsm.Start<GroundRunState>();
                    else
                        _subFsm.Start<GroundIdleState>();
                }
                _subFsmStarted = true;
            }
            else
            {
                if (_context.Input.CrouchHeld)
                {
                    if (HasHorizontalInput())
                        _subFsm.ChangeState<CrawlRunState>();
                    else
                        _subFsm.ChangeState<CrawlIdleState>();
                }
                else
                {
                    if (HasHorizontalInput())
                        _subFsm.ChangeState<GroundRunState>();
                    else
                        _subFsm.ChangeState<GroundIdleState>();
                }
            }
        }

        /// <summary>
        /// Exits the current grounded state, delegating the exit logic to the active sub-state of the grounded state.
        /// This ensures that any specific cleanup or exit behavior necessary for the sub-state is completed before transitioning from the parent state.
        /// </summary>
        public void Exit() => _subFsm.CurrentState.Exit();

        /// <summary>
        /// Performs a per-frame update for the current grounded state by delegating the update logic to the active sub-state of the grounded state.
        /// Ensures that the appropriate sub-state behavior is executed during the update cycle.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame, used for time-dependent operations within the state.</param>
        public void Update(float deltaTime) => _subFsm.Update(deltaTime);

        /// <summary>
        /// Executes logic that needs to be evaluated at a fixed timestep while the player is in the grounded state.
        /// Handles state transitions such as jumping, bouncing, leaving the ground, or sliding based on the player's context.
        /// Delegates to a sub-state for further fixed update evaluation if no top-level transitions occur.
        /// </summary>
        /// <param name="fixedDeltaTime">The fixed time step duration to be used for time-dependent computations.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            // Top-level transitions (checked before sub-FSM)

            // Bounce takes priority
            if (_context.Bounce is { BouncePrepped: true })
            {
                _fsm.ChangeState<BounceState>();
                return;
            }

            // Jump
            if (_context.Input.JumpPressed)
            {
                _context.Jump.ExecuteNormalJump();
                _context.Jump.NotifyJumpExecuted(JumpType.Normal);
                _fsm.ChangeState<JumpState>();
                return;
            }

            // Ground lost
            if (!_context.IsGrounded)
            {
                if (_context.IsOnSteepSlope)
                {
                    _fsm.ChangeState<SlideState>();
                    return;
                }

                _context.Jump.NotifyLeftGround();
                _fsm.ChangeState<FallState>();
                return;
            }

            // Delegate to sub-FSM
            _subFsm.FixedUpdate(fixedDeltaTime);
        }

        /// <summary>
        /// Determines if there is horizontal input based on the player's movement input
        /// and the configured horizontal dead zone threshold.
        /// </summary>
        /// <returns>
        /// True if the absolute value of horizontal movement input exceeds the horizontal dead zone threshold; otherwise, false.
        /// </returns>
        private bool HasHorizontalInput()
        {
            return Mathf.Abs(_context.Input.Move.x) > _context.Stats.HorizontalDeadZoneThreshold;
        }
    }
}