using UnityEngine;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.States.Substates
{
    /// <summary>
    /// Represents the idle state of a player when grounded.
    /// Implements the functionality specific to the GroundIdleState in the context of a state machine.
    /// </summary>
    public class GroundIdleState : IGroundSubState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IGroundSubState> _fsm;

        public GroundIdleState(
            IPlayerContext context, 
            IStateMachineContext<IGroundSubState> fsm
        )
        {
            _context = context;
            _fsm = fsm;
        }

        /// <summary>
        /// Executes the logic necessary when the GroundIdleState is entered.
        /// Sets the player's collider mode to standing to align with the state context.
        /// </summary>
        public void Enter() => _context.SetColliderMode(ColliderMode.Standing);

        /// <summary>
        /// Handles the logic required to transition out of the ground idle state.
        /// Invoked when the state is exited to perform necessary cleanup or preparation for the next state.
        /// </summary>
        public void Exit() { }

        /// <summary>
        /// Update logic for the ground idle state.
        /// Invoked during the main update loop to handle time-dependent behaviors or transitions.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame update, used for time-sensitive computations.</param>
        public void Update(float deltaTime) { }

        /// <summary>
        /// Fixed update logic for the ground idle state.
        /// This state checks for input and transitions to the appropriate sub-state based on the input.
        /// </summary>
        /// <param name="fixedDeltaTime">The fixed time step duration to be used for time-dependent computations.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (_context.Input.CrouchHeld)
            {
                _fsm.ChangeState<CrawlIdleState>();
                return;
            }

            if (Mathf.Abs(_context.Input.Move.x) > _context.Stats.HorizontalDeadZoneThreshold)
            {
                _fsm.ChangeState<GroundRunState>();
                return;
            }
        }
    }
}