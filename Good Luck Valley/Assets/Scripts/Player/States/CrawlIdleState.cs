using GoodLuckValley.Entity;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class CrawlIdleState : BaseState
    {
        private BoxCollider2D boxCollider;
        private DynamicCollisionHandler collisionHandler;

        public CrawlIdleState(PlayerController player, Animator animator, BoxCollider2D boxCollider,
            DynamicCollisionHandler collisionHandler) : base(player, animator)
        {
            this.boxCollider = boxCollider;
            this.collisionHandler = collisionHandler;
        }

        public override void OnEnter()
        {
            player.LearnControl("Crawl");

            // TODO: Replace with idle crawl animation
            animator.CrossFade(CrawlIdleHash, crossFadeDuration);

            // Allow the player to peek
            player.SetCanPeek(true);

            // Set player collider
            boxCollider.offset = new Vector2(-0.05183601f, -0.4508255f);
            boxCollider.size = new Vector2(0.8475914f, 0.4697776f);
            collisionHandler.UpdateCollider();

            // Adjust sprite position
            animator.transform.position = new Vector2(animator.transform.position.x, animator.transform.position.y - 0.039f);
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
            // Reset collider
            boxCollider.offset = new Vector2(-0.009529829f, -0.1905082f);
            boxCollider.size = new Vector2(0.5014615f, 0.9904121f);
            collisionHandler.UpdateCollider();

            // Adjust sprite position
            animator.transform.position = new Vector2(animator.transform.position.x, animator.transform.position.y + 0.039f);
        }
    }
}