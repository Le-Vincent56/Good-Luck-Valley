using UnityEngine;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.States.Substates
{
    /// <summary>
    /// Represents the player's state when they are idle and in a crouched or crawling position.
    /// This state is a part of the ground substates and is managed by the parent state machine.
    /// </summary>
    public class CrawlIdleState : IGroundSubState
    {
        private readonly IPlayerContext _context;
        private readonly IStateMachineContext<IGroundSubState> _fsm;

        public CrawlIdleState(
            IPlayerContext context, 
            IStateMachineContext<IGroundSubState> fsm
        )
        {
            _context = context;
            _fsm = fsm;
        }

        /// <summary>
        /// Initializes the CrawlIdleState by configuring the player's collider to "Crawling" mode
        /// and initializing the crawling movement with zero initial speed.
        /// </summary>
        public void Enter()
        {
            _context.SetColliderMode(ColliderMode.Crawling);
            _context.Crawl.StartCrawl(0f);
        }

        /// <summary>
        /// Executes the logic required to exit the CrawlIdleState.
        /// </summary>
        public void Exit() => _context.Crawl.EndCrawl();

        /// <summary>
        /// Updates the state of the CrawlIdleState during the Update phase of the game loop.
        /// </summary>
        /// <param name="deltaTime">The time elapsed between the current and the previous frame, used to update game logic.</param>
        public void Update(float deltaTime) { }

        /// <summary>
        /// Updates the behavior of the CrawlIdleState during the FixedUpdate phase of the game loop.
        /// </summary>
        /// <param name="fixedDeltaTime">The fixed time step at which physics and FixedUpdate methods are called during the game loop.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (!_context.Input.CrouchHeld && _context.Crawl.CanStand)
            {
                _fsm.ChangeState<GroundIdleState>();
                return;
            }

            if (Mathf.Abs(_context.Input.Move.x) > _context.Stats.HorizontalDeadZoneThreshold)
            {
                _fsm.ChangeState<CrawlRunState>();
                return;
            }
        }
    }
}