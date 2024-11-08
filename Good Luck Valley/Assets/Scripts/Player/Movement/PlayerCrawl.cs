using System;
using UnityEngine;

namespace GoodLuckValley.Player.Movement
{
    [Serializable]
    public class PlayerCrawl
    {
        private PlayerController controller;
        [SerializeField] private bool crawling;
        private float timeStartedCrawling;
        public bool Crawling { get => crawling; }
        private bool CrawlPressed { get => controller.FrameData.Input.Crawling; }
        public bool CanStand { get => IsStandingPosClear(controller.RB.position + controller.CharacterSize.StandingColliderCenter); }
        public float TimeStartedCrawling { get => timeStartedCrawling; }

        public PlayerCrawl(PlayerController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Check if the player position is clear to stand
        /// </summary>
        private bool IsStandingPosClear(Vector2 pos)
        {
            // Calcualte the size necessary to stand
            Vector2 size = controller.CharacterSize.StandingColliderSize - controller.Collisions.SkinWidth * Vector2.one;
            
            // Disable hit triggers
            Physics2D.queriesHitTriggers = false;

            // Check for an overlap with collision layers if the player were to stand
            Collider2D hit = Physics2D.OverlapBox(pos, size, 0, controller.Stats.CollisionLayers);

            // Reset whether or not to query hit triggers
            Physics2D.queriesHitTriggers = controller.CachedQueryMode;

            // Return false if there was a hit
            return !hit;
        }

        /// <summary>
        /// Calculate whether or not the player should be crouching
        /// </summary>
        public void CalculateCrawl()
        {
            // Exit case - if crouching is not allowed
            if (!controller.Stats.AllowCrouching) return;

            if (!crawling && CrawlPressed && controller.Collisions.Grounded)
                ToggleCrawling(true);
            else if (crawling && (!CrawlPressed || !controller.Collisions.Grounded))
                ToggleCrawling(false);
        }

        /// <summary>
        /// Toggle crouching for the player
        /// </summary>
        private void ToggleCrawling(bool shouldCrouch)
        {
            // Check if the Player should crouch
            if(shouldCrouch)
            {
                // Set the time the player started crouching
                timeStartedCrawling = controller.Time;

                // Start crouching
                crawling = true;
            } 
            else
            {
                // Exit case - if the Player cannot stand
                if (!CanStand) return;
                
                // Stop crouching
                crawling = false;
            }

            // Verify the collider mode
            controller.Collisions.SetColliderMode(
                crawling 
                ? CollisionHandler.ColliderMode.Crawling 
                : CollisionHandler.ColliderMode.Standard
            );
        }
    }
}
