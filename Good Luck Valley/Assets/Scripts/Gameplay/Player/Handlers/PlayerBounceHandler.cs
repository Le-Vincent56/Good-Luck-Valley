using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.Handlers
{
    /// <summary>
    /// The PlayerBounceHandler class is responsible for managing the logic related to the player's bounce mechanics.
    /// This includes preparing the bounce, executing it with calculated force, and resetting the bounce state.
    /// The class also determines whether it can detect ground based on internal states, such as ignore detection timing.
    /// </summary>
    public class PlayerBounceHandler : IBounceHandler
    {
        private readonly IPlayerMotor _motor;
        private readonly PlayerStats _stats;

        private bool _bouncePrepped;
        private bool _isBouncing;
        private float _yContactValue;
        private float _ignoreDetectionRemaining;

        public bool IsBouncing => _isBouncing;
        public bool BouncePrepped => _bouncePrepped;
        public bool CanDetectGround => _ignoreDetectionRemaining <= 0f;

        public PlayerBounceHandler(IPlayerMotor motor, PlayerStats stats)
        {
            _motor = motor;
            _stats = stats;
        }

        /// <summary>
        /// Prepares the player for a bounce action by setting the bounce-prepped status, storing the provided
        /// vertical contact value, and initializing the remaining time during which ground detection is ignored.
        /// </summary>
        /// <param name="yContactValue">The vertical contact value used for calculating bounce power.</param>
        public void PrepareBounce(float yContactValue)
        {
            _bouncePrepped = true;
            _yContactValue = yContactValue;
            _ignoreDetectionRemaining = _stats.BounceIgnoreDetectionTime;
        }

        /// <summary>
        /// Executes the player's bounce action by applying an upward impulse to the motor,
        /// calculated as a linear interpolation between the maximum and minimum bounce power
        /// based on the vertical contact value. Clears the prepared bounce status and sets
        /// the active bouncing status.
        /// </summary>
        public void ExecuteBounce()
        {
            float power = Mathf.Lerp(
                _stats.MaxBouncePower, 
                _stats.MinBouncePower,
                _yContactValue
            );
            _motor.ApplyImpulse(new Vector2(0f, power), false, true);

            _bouncePrepped = false;
            _isBouncing = true;
        }

        /// <summary>
        /// Resets the bounce handler's internal state, including prepared bounce status,
        /// active bounce status, vertical contact value, and ground detection ignore timer.
        /// </summary>
        public void ResetBounce()
        {
            _bouncePrepped = false;
            _isBouncing = false;
            _yContactValue = 0f;
            _ignoreDetectionRemaining = 0f;
        }

        /// <summary>
        /// Updates the bounce handler's internal state, primarily for managing the
        /// ground detection ignore timer.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, used to decrement relevant timers.</param>
        public void Tick(float deltaTime)
        {
            if (_ignoreDetectionRemaining <= 0f)
                return;
            
            _ignoreDetectionRemaining -= deltaTime;
        }
    }
}