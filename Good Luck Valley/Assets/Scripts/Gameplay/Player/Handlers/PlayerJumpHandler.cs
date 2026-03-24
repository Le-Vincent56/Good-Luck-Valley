using UnityEngine;
using GoodLuckValley.Core.Input.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.Handlers
{
    /// <summary>
    /// Handles the player's jumping mechanics, including normal jumps,
    /// air jumps, and jump-related state management. This class interacts
    /// with the player's motor, input system, and stats to provide responsive
    /// and dynamic jump capabilities.
    /// </summary>
    public class PlayerJumpHandler : IJumpHandler
    {
        private readonly IPlayerMotor _motor;
        private readonly IPlayerInput _input;
        private readonly PlayerStats _stats;

        private float _coyoteTimeRemaining;
        private float _jumpClearanceRemaining;
        private int _airJumpsRemaining;
        private bool _endedJumpEarly;
        private JumpType _lastJumpType;
        private bool _hasJumped;

        public bool IsInJumpClearance => _jumpClearanceRemaining > 0f;
        public bool EndedJumpEarly => _endedJumpEarly;
        public int AirJumpsRemaining => _airJumpsRemaining;
        public JumpType LastJumpType => _lastJumpType;
        public bool HasCoyoteTime => _coyoteTimeRemaining > 0f;
        public bool HasAirJump => _airJumpsRemaining > 0;

        public PlayerJumpHandler(IPlayerMotor motor, IPlayerInput input, PlayerStats stats)
        {
            _motor = motor;
            _input = input;
            _stats = stats;
        }

        /// <summary>
        /// Executes a normal jump for the player by applying a vertical impulse
        /// using the player's defined jump power. The Y-axis velocity is reset
        /// upon execution to ensure consistent jump behavior.
        /// </summary>
        public void ExecuteNormalJump()
        {
            _motor.ApplyImpulse(
                new Vector2(0f, _stats.JumpPower), 
                false, 
                true
            );
        }

        /// <summary>
        /// Executes an air jump for the player by applying a vertical impulse using the player's jump power.
        /// Reduces the number of remaining air jumps by one upon execution.
        /// </summary>
        public void ExecuteAirJump()
        {
            _motor.ApplyImpulse(new Vector2(0f, _stats.JumpPower), false, true);
            _airJumpsRemaining--;
        }

        /// <summary>
        /// Notifies the handler that the player has made contact with the ground.
        /// This restores the maximum number of air jumps and resets coyote time, jump states, and related flags.
        /// </summary>
        public void NotifyGrounded()
        {
            _airJumpsRemaining = _stats.MaxAirJumps;
            _coyoteTimeRemaining = 0f;
            _endedJumpEarly = false;
            _hasJumped = false;
            _lastJumpType = JumpType.None;
        }

        /// <summary>
        /// Notifies the handler that the player has left the ground.
        /// This initializes the coyote time, allowing a brief window to perform a jump after becoming airborne.
        /// </summary>
        public void NotifyLeftGround()
        {
            if (_hasJumped) return;
            
            _coyoteTimeRemaining = _stats.CoyoteTime;
        }

        /// <summary>
        /// Notifies the handler that a jump has been executed, initializing related state
        /// such as jump clearance time, coyote time, and tracking the type of jump performed.
        /// </summary>
        /// <param name="type">The type of jump that was executed, represented as a value of the JumpType enum.</param>
        public void NotifyJumpExecuted(JumpType type)
        {
            _lastJumpType = type;
            _jumpClearanceRemaining = _stats.JumpClearanceTime;
            _coyoteTimeRemaining = 0f;
            _hasJumped = true;
            _endedJumpEarly = false;
        }

        /// <summary>
        /// Handles the player's jump release action by marking the jump as ended early
        /// if the jump button is no longer held, the player has jumped, and the jump has
        /// not already been ended early.
        /// </summary>
        public void NotifyJumpReleased()
        {
            if (_input.JumpHeld || !_hasJumped || _endedJumpEarly) return;
            
            _endedJumpEarly = true;
        }

        /// <summary>
        /// Restores one air jump to the player if the current air jumps are less than the maximum allowed.
        /// </summary>
        public void RestoreAirJump()
        {
            if (_airJumpsRemaining >= _stats.MaxAirJumps) return;
            
            _airJumpsRemaining++;
        }

        /// <summary>
        /// Grants the player a specified number of additional air jumps.
        /// </summary>
        /// <param name="count">The number of air jumps to add to the player's current remaining air jumps.</param>
        public void GrantAirJumps(int count) => _airJumpsRemaining += count;

        /// <summary>
        /// Updates the internal timers for coyote time and jump clearance based on the elapsed time.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, used to decrement internal timers.</param>
        /// <param name="currentTime">The current time in the game, used for time-dependent logic.</param>
        public void Tick(float deltaTime, float currentTime)
        {
            if (_coyoteTimeRemaining > 0f)
                _coyoteTimeRemaining -= deltaTime;

            if (_jumpClearanceRemaining > 0f)
                _jumpClearanceRemaining -= deltaTime;
        }
    }
}