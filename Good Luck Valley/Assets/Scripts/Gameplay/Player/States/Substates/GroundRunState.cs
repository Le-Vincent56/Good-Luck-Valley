using UnityEngine;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.States.Substates
{
    /// <summary>
    /// Represents the running state of a player character while grounded.
    /// This state handles the player's behavior and interaction logic during ground-based movement.
    /// </summary>
    public class GroundRunState : IGroundSubState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IGroundSubState> _fsm;

        public GroundRunState(
            IPlayerContext context, 
            IStateMachineContext<IGroundSubState> fsm
        )
        {
            _context = context;
            _fsm = fsm;
        }

        /// <summary>
        /// Executes the necessary initialization and setup procedures when entering the ground running state.
        /// This method is invoked to prepare the player context, including setting collider configurations
        /// to ensure appropriate behavior and interactions while in the ground running state.
        /// </summary>
        public void Enter() => _context.SetColliderMode(ColliderMode.Standing);

        /// <summary>
        /// Performs necessary operations and cleanup when exiting the ground running state.
        /// This method is called as part of the state's lifecycle management to handle
        /// state-specific exit logic before transitioning to another state.
        /// </summary>
        public void Exit() { }

        /// <summary>
        /// Called every frame to perform updates required for the ground running state.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last Update call.</param>
        public void Update(float deltaTime) { }

        /// <summary>
        /// Called at a fixed interval to perform updates required for the ground running state.
        /// </summary>
        /// <param name="fixedDeltaTime">The time elapsed since the last FixedUpdate call.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (_context.Input.CrouchHeld)
            {
                _fsm.ChangeState<CrawlRunState>();
                return;
            }

            if (Mathf.Abs(_context.Input.Move.x) <= _context.Stats.HorizontalDeadZoneThreshold)
            {
                _fsm.ChangeState<GroundIdleState>();
                return;
            }
        }
    }
}