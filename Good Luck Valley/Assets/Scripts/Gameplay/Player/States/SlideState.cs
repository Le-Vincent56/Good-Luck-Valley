using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.States
{
    /// <summary>
    /// Represents the player's sliding state within the state machine of the gameplay.
    /// This state is activated when the player transitions into a sliding condition,
    /// such as descending a steep slope or other predefined gameplay scenarios.
    /// </summary>
    public class SlideState : IPlayerState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IPlayerState> _fsm;

        public SlideState(
            IPlayerContext context, 
            IStateMachineContext<IPlayerState> fsm
        )
        {
            _context = context;
            _fsm = fsm;
        }

        /// <summary>
        /// Initializes the slide state by configuring the player's motor properties
        /// such as gravity scale and maximum fall speed, and sets the appropriate collider mode.
        /// </summary>
        public void Enter()
        {
            _context.Motor.SetGravityScale(_context.Stats.SlideGravityScale);
            _context.Motor.SetMaxFallSpeed(_context.Stats.SlidingMaxSpeed);
            _context.SetColliderMode(ColliderMode.Airborne);
        }

        /// <summary>
        /// Cleans up or finalizes the slide state before transitioning to another state.
        /// </summary>
        public void Exit() { }

        /// <summary>
        /// Updates the slide state during the update loop, called every frame.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        public void Update(float deltaTime) { }

        /// <summary>
        /// Updates the slide state during the fixed update loop.
        /// </summary>
        /// <param name="fixedDeltaTime">The time elapsed since the last fixed update.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            // Slope normalized — back to grounded
            if (!_context.IsOnSteepSlope && _context.Collision.GroundDetected)
            {
                _fsm.ChangeState<GroundedState>();
                return;
            }

            // Ground lost
            if (!_context.Collision.GroundDetected)
            {
                _fsm.ChangeState<FallState>();
                return;
            }
        }
    }
}