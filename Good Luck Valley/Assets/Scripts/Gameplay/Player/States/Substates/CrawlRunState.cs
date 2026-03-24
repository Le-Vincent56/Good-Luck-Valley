using UnityEngine;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.States.Substates
{
    /// <summary>
    /// Represents a state where the player is crawling and running within the grounded state.
    /// Implements the <see cref="IGroundSubState"/> interface for integration with the player's ground substate management system.
    /// </summary>
    public class CrawlRunState : IGroundSubState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IGroundSubState> _fsm;

        public CrawlRunState(
            IPlayerContext context, 
            IStateMachineContext<IGroundSubState> fsm
        )
        {
            _context = context;
            _fsm = fsm;
        }

        /// <summary>
        /// Initiates the CrawlRun state by configuring the player's collider to crawling mode
        /// and starting the crawling behavior with an initial speed of zero.
        /// </summary>
        public void Enter()
        {
            _context.SetColliderMode(ColliderMode.Crawling);
            _context.Crawl.StartCrawl(0f);
        }

        /// <summary>
        /// Exits the current CrawlRun state. Invokes the end crawling behavior using the associated player context.
        /// </summary>
        public void Exit() => _context.Crawl.EndCrawl();

        /// <summary>
        /// Updates the gameplay logic for the CrawlRun state on the player. This method is called every frame
        /// and allows for handling time-sensitive operations based on the elapsed time since the last frame.
        /// </summary>
        /// <param name="deltaTime">The time in seconds that has passed since the previous frame update.</param>
        public void Update(float deltaTime) { }

        /// <summary>
        /// Executes logic that is updated at fixed time intervals during the CrawlRun state.
        /// Checks player input and physical conditions to determine state transitions.
        /// </summary>
        /// <param name="fixedDeltaTime">The time interval since the last fixed update.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (!_context.Input.CrouchHeld && _context.Crawl.CanStand)
            {
                _fsm.ChangeState<GroundRunState>();
                return;
            }

            if (Mathf.Abs(_context.Input.Move.x) <= _context.Stats.HorizontalDeadZoneThreshold)
            {
                _fsm.ChangeState<CrawlIdleState>();
                return;
            }
        }
    }
}