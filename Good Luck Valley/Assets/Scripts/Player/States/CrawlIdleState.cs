using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class CrawlIdleState : BaseState
    {
        private BoxCollider2D boxCollider;

        public CrawlIdleState(PlayerController player, Animator animator, BoxCollider2D boxCollider) : base(player, animator)
        {
            this.boxCollider = boxCollider;
        }

        public override void OnEnter()
        {
            player.LearnControl("Crawl");

            // TODO: Replace with idle crawl animation
            animator.CrossFade(LocomotionHash, crossFadeDuration);

            // Allow the player to peek
            player.SetCanPeek(true);
        }

        public override void FixedUpdate()
        {
            // Calculate velocity
            player.CalculateVelocity();

            // Handle movement
            player.HandleMovement();
        }
    }
}