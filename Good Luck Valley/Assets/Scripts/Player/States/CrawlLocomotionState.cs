using GoodLuckValley.Entity;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class CrawlLocomotionState : BaseState
    {
        private readonly PlayerSFXHandler sfxHandler;
        private DynamicCollisionHandler collisionHandler;
        private BoxCollider2D boxCollider;

        public CrawlLocomotionState(PlayerController player, Animator animator, 
            PlayerSFXHandler sfxHandler, BoxCollider2D boxCollider, 
            DynamicCollisionHandler collisionHandler) : base(player, animator)
        {
            this.sfxHandler = sfxHandler;
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

            // Reset box collider
            boxCollider.offset = new Vector2(-0.009529829f, -0.1905082f);
            boxCollider.size = new Vector2(0.5014615f, 0.9904121f);
            collisionHandler.UpdateCollider();
        }
    }
}