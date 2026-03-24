using UnityEngine;
using GoodLuckValley.Core.Input.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.Handlers
{
    /// <summary>
    /// Handles player interactions with walls, including attaching to walls, detaching, wall jumping,
    /// modifying movement input, and processing wall-related state updates.
    /// This class integrates player motor, input handling, and player stats to manage wall-specific behaviors.
    /// </summary>
    public class PlayerWallHandler : IWallHandler
    {
        private readonly IPlayerMotor _motor;
        private readonly IPlayerInput _input;
        private readonly PlayerStats _stats;

        private bool _isOnWall;
        private int _wallDirection;
        private bool _isWallJumping;
        private float _wallCoyoteTimeRemaining;
        private float _reattachCooldownRemaining;
        private float _wallJumpElapsed;
        private int _lastWallDirection;

        public bool IsOnWall => _isOnWall;
        public int WallDirection => _wallDirection;
        public bool IsWallJumping => _isWallJumping;
        public bool HasWallCoyoteTime => _wallCoyoteTimeRemaining > 0f;

        public PlayerWallHandler(IPlayerMotor motor, IPlayerInput input, PlayerStats stats)
        {
            _motor = motor;
            _input = input;
            _stats = stats;
        }

        /// <summary>
        /// Determines whether the player should attach to a wall based on collision data,
        /// movement input, grounded status, wall detection, input direction, and cooldown.
        /// </summary>
        /// <param name="collision">Data related to player collision, including wall detection information.</param>
        /// <param name="moveInput">The player's current movement input as a vector.</param>
        /// <param name="isGrounded">A boolean indicating whether the player is currently grounded.</param>
        /// <returns>True if the player should attach to the wall; otherwise, false.</returns>
        public bool ShouldAttachToWall(
            CollisionData collision,
            Vector2 moveInput,
            bool isGrounded
        )
        {
            if (isGrounded) return false;
            if (_reattachCooldownRemaining > 0f) return false;

            int wallDir = 0;
            if (collision.RightWallDetected) wallDir = 1;
            else if (collision.LeftWallDetected) wallDir = -1;

            if (wallDir == 0) return false;

            if (_stats.RequireInputPush)
            {
                float inputDir = Mathf.Sign(moveInput.x);
                
                if (Mathf.Abs(moveInput.x) < _stats.HorizontalDeadZoneThreshold)
                    return false;
                
                if ((int)inputDir != wallDir) return false;
            }

            return true;
        }

        /// <summary>
        /// Attaches the player to the wall, setting the wall attachment state, wall direction,
        /// and resetting the wall coyote time and reattach cooldown.
        /// </summary>
        /// <param name="direction">The direction of the wall the player is attaching to, represented as an integer.</param>
        public void AttachToWall(int direction)
        {
            _isOnWall = true;
            _wallDirection = direction;
            _lastWallDirection = direction;
            _wallCoyoteTimeRemaining = 0f;
            _reattachCooldownRemaining = 0f;
        }

        /// <summary>
        /// Detaches the player from the wall, resetting the wall attachment state, wall direction,
        /// and initializing the wall coyote time and reattach cooldown based on player stats.
        /// </summary>
        public void DetachFromWall()
        {
            _isOnWall = false;
            _wallDirection = 0;
            _wallCoyoteTimeRemaining = _stats.WallCoyoteTime;
            _reattachCooldownRemaining = _stats.WallReattachCooldown;
        }

        /// <summary>
        /// Executes a wall jump, applying an impulse to the player away from the wall
        /// and transitioning the player out of the wall attachment state.
        /// </summary>
        public void ExecuteWallJump()
        {
            Vector2 impulse = new Vector2(
                -_lastWallDirection * _stats.WallJumpPowerX,
                _stats.WallJumpPowerY
            );

            _motor.ApplyImpulse(impulse, true, true);

            _isOnWall = false;
            _wallDirection = 0;
            _isWallJumping = true;
            _wallJumpElapsed = 0f;
            _reattachCooldownRemaining = _stats.WallReattachCooldown;
        }

        /// <summary>
        /// Adjusts the player's movement input based on the current wall jump state, allowing
        /// for input reversal during an initial loss window and gradual restoration during a recovery phase.
        /// </summary>
        /// <param name="rawInput">The original movement input vector provided by the player.</param>
        /// <returns>A modified movement input vector adjusted according to the wall jump state and elapsed time.</returns>
        public Vector2 AdjustMoveInput(Vector2 rawInput)
        {
            if (!_isWallJumping) return rawInput;

            float totalLossTime = _stats.WallJumpTotalInputLossTime;
            float returnTime = _stats.WallJumpInputLossReturnTime;

            if (_wallJumpElapsed < totalLossTime)
            {
                // Full input reversal during loss window
                return new Vector2(-rawInput.x, rawInput.y);
            }

            float recoveryElapsed = _wallJumpElapsed - totalLossTime;

            if (recoveryElapsed >= returnTime)
                return rawInput;

            // Lerp from reversed (-1) to normal (1)
            float nerfPoint = recoveryElapsed / returnTime;
            float multiplier = Mathf.Lerp(-1f, 1f, nerfPoint);
            return new Vector2(rawInput.x * multiplier, rawInput.y);
        }

        /// <summary>
        /// Calculates the vertical velocity of the player when interacting with a wall,
        /// accounting for input direction, current vertical velocity, and frame time.
        /// </summary>
        /// <param name="moveInput">The player's movement input vector, where the y-component determines climbing or sliding behavior.</param>
        /// <param name="currentYVelocity">The current vertical velocity of the player during the wall interaction.</param>
        /// <param name="deltaTime">The time elapsed in the current frame, used for velocity calculation.</param>
        /// <returns>The calculated vertical velocity based on the player's input and whether they are climbing or sliding down the wall.</returns>
        public float CalculateWallVelocity(
            Vector2 moveInput,
            float currentYVelocity,
            float deltaTime
        )
        {
            if (moveInput.y < -_stats.VerticalDeadZoneThreshold)
            {
                // Climbing down — direct input control
                return moveInput.y * _stats.WallClimbSpeed;
            }

            // Sliding down with acceleration
            float minVelocity = Mathf.Min(currentYVelocity, 0f);
            return Mathf.MoveTowards(
                minVelocity, 
                -_stats.WallClimbSpeed,
                _stats.WallFallAcceleration * deltaTime
            );
        }

        /// <summary>
        /// Resets the wall-related state, including wall attachment, wall jumping,
        /// coyote time, reattach cooldown, and wall jump tracking, when the player is grounded.
        /// </summary>
        public void NotifyGrounded()
        {
            _isOnWall = false;
            _wallDirection = 0;
            _isWallJumping = false;
            _wallCoyoteTimeRemaining = 0f;
            _reattachCooldownRemaining = 0f;
            _wallJumpElapsed = 0f;
        }

        /// <summary>
        /// Updates the state of the object based on the time elapsed since the last update.
        /// </summary>
        /// <param name="deltaTime">The time in seconds since the last call to this method.</param>
        /// <param name="currentTime">The current time in seconds.</param>
        public void Tick(float deltaTime, float currentTime)
        {
            if (_wallCoyoteTimeRemaining > 0f)
                _wallCoyoteTimeRemaining -= deltaTime;

            if (_reattachCooldownRemaining > 0f)
                _reattachCooldownRemaining -= deltaTime;

            if (!_isWallJumping) return;
            _wallJumpElapsed += deltaTime;

            float totalTime = _stats.WallJumpTotalInputLossTime + _stats.WallJumpInputLossReturnTime;

            if (_wallJumpElapsed < totalTime) return;
            
            _isWallJumping = false;
        }
    }
}