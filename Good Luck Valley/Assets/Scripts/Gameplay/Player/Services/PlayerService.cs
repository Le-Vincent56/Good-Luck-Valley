using System;
using UnityEngine;
using GoodLuckValley.Core.Input.Interfaces;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Core.StateMachine.Services;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;
using GoodLuckValley.Gameplay.Player.Motor;
using GoodLuckValley.Gameplay.Player.States;

namespace GoodLuckValley.Gameplay.Player.Services
{
    /// <summary>
    /// PlayerService manages player-specific functionalities, including movement handling,
    /// input processing, and player state transitions. It acts as a central service that
    /// coordinates various aspects of a player's behavior within the game.
    /// </summary>
    public class PlayerService : IPlayerService, IPlayerContext, IPlayerAbilityControl
    {
        private readonly IPlayerMotor _motor;
        private readonly IJumpHandler _jumpHandler;
        private readonly IWallHandler _wallHandler;
        private readonly IBounceHandler _bounceHandler;
        private readonly ICrawlHandler _crawlHandler;
        private readonly IPlayerInput _input;
        private readonly PlayerStats _stats;
        private readonly CharacterSize _characterSize;
        private readonly StateMachine<IPlayerState> _fsm;
        private readonly IPlayerForceReceiver _forceReceiver;

        // Computed per-tick
        private CollisionData _collision;
        private Vector2 _currentVelocity;
        private bool _isGrounded;
        private bool _isOnSteepSlope;
        private int _detectedWallDirection;
        private ColliderMode _colliderMode;
        private Vector2 _position;

        // IPlayerService
        public Vector2 Position => _position;
        public Vector2 Velocity => _currentVelocity;
        public bool IsGrounded => _isGrounded;
        public ColliderMode CurrentColliderMode => _colliderMode;
        public IPlayerForceReceiver ForceReceiver => _forceReceiver;
        public IPlayerAbilityControl AbilityControl => this;
        public event Action<IPlayerState, IPlayerState> OnStateChanged;

        // IPlayerContext
        IPlayerMotor IPlayerContext.Motor => _motor;
        IPlayerInput IPlayerContext.Input => _input;
        IJumpHandler IPlayerContext.Jump => _jumpHandler;
        IWallHandler IPlayerContext.Wall => _wallHandler;
        IBounceHandler IPlayerContext.Bounce => _bounceHandler;
        ICrawlHandler IPlayerContext.Crawl => _crawlHandler;
        PlayerStats IPlayerContext.Stats => _stats;
        CharacterSize IPlayerContext.CharacterSize => _characterSize;
        CollisionData IPlayerContext.Collision => _collision;
        bool IPlayerContext.IsGrounded => _isGrounded;
        bool IPlayerContext.IsOnSteepSlope => _isOnSteepSlope;
        int IPlayerContext.DetectedWallDirection => _detectedWallDirection;
        Vector2 IPlayerContext.CurrentVelocity => _currentVelocity;
        ColliderMode IPlayerContext.CurrentColliderMode => _colliderMode;
        IStateMachineContext<IPlayerState> IPlayerContext.StateMachine => _fsm;

        public PlayerService(
            IPlayerMotor motor,
            IJumpHandler jumpHandler,
            IWallHandler wallHandler,
            IBounceHandler bounceHandler,
            ICrawlHandler crawlHandler,
            IPlayerInput input,
            PlayerStats stats,
            CharacterSize characterSize)
        {
            _motor = motor;
            _jumpHandler = jumpHandler;
            _wallHandler = wallHandler;
            _bounceHandler = bounceHandler;
            _crawlHandler = crawlHandler;
            _input = input;
            _stats = stats;
            _characterSize = characterSize;

            // Cast motor to force receiver (PlayerMotor implements both)
            _forceReceiver = (IPlayerForceReceiver)motor;

            // Create FSM and states
            _fsm = new StateMachine<IPlayerState>();
            IStateMachineContext<IPlayerState> fsmContext = _fsm;

            _fsm.AddState(new GroundedState(this, fsmContext));
            _fsm.AddState(new JumpState(this, fsmContext));
            _fsm.AddState(new FallState(this, fsmContext));
            _fsm.AddState(new WallState(this, fsmContext));
            _fsm.AddState(new BounceState(this, fsmContext));
            _fsm.AddState(new SlideState(this, fsmContext));

            _fsm.OnStateChanged += (prev, curr) => OnStateChanged?.Invoke(prev, curr);
            _fsm.Start<GroundedState>();
        }

        public void SetColliderMode(ColliderMode mode) => _colliderMode = mode;

        /// <summary>
        /// Processes a fixed update for the player's state, including handling physics, movement, and collision detection.
        /// This method is called on a fixed interval to ensure consistent game physics behavior.
        /// </summary>
        /// <param name="deltaTime">The time step in seconds since the last fixed update.</param>
        /// <param name="currentTime">The current in-game time in seconds.</param>
        /// <param name="collision">Collision data containing information about the player's interaction with the environment.</param>
        /// <param name="currentVelocity">The current velocity of the player as a 2D vector.</param>
        /// <param name="globalGravityY">The global gravity value affecting the player on the Y-axis.</param>
        /// <returns>A <c>MotorOutput</c> struct containing the computed velocity and other physics state.</returns>
        public MotorOutput FixedUpdate(
            float deltaTime,
            float currentTime,
            CollisionData collision,
            Vector2 currentVelocity,
            float globalGravityY
        )
        {
            // Store tick data
            _collision = collision;
            _currentVelocity = currentVelocity;

            // Compute game state
            _isGrounded = collision.GroundDetected
                          && collision.GroundAngle <= _stats.MaxWalkableSlope
                          && !_jumpHandler.IsInJumpClearance
                          && _bounceHandler.CanDetectGround;

            _isOnSteepSlope = collision.GroundDetected
                              && collision.GroundAngle > _stats.MaxWalkableSlope;

            _detectedWallDirection = collision.RightWallDetected ? 1
                : collision.LeftWallDetected ? -1
                : 0;

            // Update handlers
            _crawlHandler.UpdateCeilingState(collision.CeilingBlocked);
            _jumpHandler.Tick(deltaTime, currentTime);
            _wallHandler.Tick(deltaTime, currentTime);
            _bounceHandler.Tick(deltaTime);
            _crawlHandler.Tick(deltaTime, currentTime);

            // Motor tick
            _motor.BeginTick();

            // FSM tick
            Vector2 moveInput = _wallHandler.AdjustMoveInput(_input.Move);
            _fsm.FixedUpdate(deltaTime);

            // Calculate velocity
            MotorOutput output = _motor.CalculateVelocity(
                currentVelocity, 
                moveInput, 
                collision, 
                globalGravityY, 
                deltaTime
            );

            _currentVelocity = output.Velocity;

            return output;
        }

        /// <summary>
        /// Updates the player's state, processing any state changes or logic associated with the current state.
        /// The update is based on the elapsed time since the previous frame.
        /// </summary>
        /// <param name="deltaTime">The time, in seconds, that has elapsed since the last update.</param>
        public void Update(float deltaTime) => _fsm.Update(deltaTime);

        /// <summary>
        /// Restores the player's ability to perform another air jump.
        /// This method re-enables air jump functionality for the player,
        /// typically used after consuming a previously granted air jump.
        /// </summary>
        public void RestoreAirJump() => _jumpHandler.RestoreAirJump();

        /// <summary>
        /// Grants the player additional air jumps by specifying the number to be added.
        /// This method adjusts the player's jump capability during gameplay to allow
        /// multiple consecutive jumps before landing.
        /// </summary>
        /// <param name="count">
        /// The number of additional air jumps to grant to the player.
        /// </param>
        public void GrantAirJumps(int count) => _jumpHandler.GrantAirJumps(count);

        /// <summary>
        /// Prepares the player character for a bounce operation, typically in response
        /// to a collision or interaction with a surface. This method utilizes the bounce
        /// handler to set up the necessary parameters for executing the bounce mechanics.
        /// </summary>
        /// <param name="yContactValue">
        /// The y-coordinate of the contact point used to calculate or configure the bounce
        /// properties.
        /// </param>
        public void PrepareBounce(float yContactValue) => _bounceHandler.PrepareBounce(yContactValue);
    }
}