using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class CrawlLocomotionState : BaseState
    {
        private readonly PlayerSFXHandler sfxHandler;
        private BoxCollider2D boxCollider;

        public CrawlLocomotionState(PlayerController player, Animator animator, 
            PlayerSFXHandler sfxHandler, BoxCollider2D boxCollider) : base(player, animator)
        {
            this.sfxHandler = sfxHandler;
            this.boxCollider = boxCollider;
        }

        public override void OnEnter()
        {
            player.LearnControl("Crawl");

            // TODO: Replace with locomotion crawl animation
            animator.CrossFade(CrawlLocomotionHash, crossFadeDuration);

            // Allow the player to peek
            player.SetCanPeek(true);
        }

        public override void Update()
        {
            // TODO: Update slide sound
        }

        public override void FixedUpdate()
        {
            // Calculate velocity
            player.CalculateVelocity();

            // Handle movement
            player.HandleMovement();
        }

        public override void OnExit()
        {
            // TODO: (Maybe) reset slide sound
        }
    }
}