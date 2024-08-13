using GoodLuckValley.Audio.SFX;
using GoodLuckValley.Entity;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class CrawlLocomotionState : BaseState
    {
        private readonly PlayerSFXMaster sfx;
        private DynamicCollisionHandler collisionHandler;
        private BoxCollider2D boxCollider;

        public CrawlLocomotionState(PlayerController player, Animator animator,
            PlayerSFXMaster sfx, BoxCollider2D boxCollider, 
            DynamicCollisionHandler collisionHandler) : base(player, animator)
        {
            this.sfx = sfx;
            this.boxCollider = boxCollider;
            this.collisionHandler = collisionHandler;
        }

        public override void OnEnter()
        {
            player.LearnControl("Crawl");

            // TODO: Replace with locomotion crawl animation
            animator.CrossFade(CrawlLocomotionHash, crossFadeDuration);

            // Allow the player to peek
            player.SetCanPeek(true);

            // Set player box collider
            boxCollider.offset = new Vector2(-0.05183601f, -0.4508255f);
            boxCollider.size = new Vector2(0.8475914f, 0.4697776f);
            collisionHandler.UpdateCollider();

            // Start playing footsteps with crawl RTPC values
            sfx.SetSpeedRTPC(sfx.CRAWL);
            sfx.StartGroundImpacts();

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
            // Reset box collider
            boxCollider.offset = new Vector2(-0.009529829f, -0.1905082f);
            boxCollider.size = new Vector2(0.5014615f, 0.9904121f);
            collisionHandler.UpdateCollider();

            // Stop ground impacts
            sfx.StopGroundImpacts();

            // Adjust sprite position
            animator.transform.position = new Vector2(animator.transform.position.x, animator.transform.position.y + 0.039f);
        }
    }
}