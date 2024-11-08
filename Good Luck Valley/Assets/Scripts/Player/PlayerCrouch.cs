using UnityEngine;

namespace GoodLuckValley.Player
{
    public class PlayerCrouch
    {
        private PlayerController controller;
        private bool crouching;
        private float timeStartedCrouching;
        public bool Crouching { get => crouching; }
        private bool CrouchPressed => controller.FrameData.Input.Move.y < -controller.Stats.VerticalDeadZoneThreshold;
        public bool CanStand => IsStandingPosClear(controller.RB.position + controller.CharacterSize.StandingColliderCenter);
        public float TimeStartedCrouching { get => timeStartedCrouching; }

        public PlayerCrouch(PlayerController controller)
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
        public void CalculateCrouch()
        {
            // Exit case - if crouching is not allowed
            if (!controller.Stats.AllowCrouching) return;

            if (!crouching && CrouchPressed && controller.Collisions.Grounded)
                ToggleCrouching(true);
            else if (crouching && (!CrouchPressed && !controller.Collisions.Grounded))
                ToggleCrouching(false);
        }

        /// <summary>
        /// Toggle crouching for the player
        /// </summary>
        private void ToggleCrouching(bool shouldCrouch)
        {
            // Check if the Player should crouch
            if(shouldCrouch)
            {
                // Set the time the player started crouching
                timeStartedCrouching = controller.Time;

                // Start crouching
                crouching = true;
            } 
            else
            {
                // Exit case - if the Player cannot stand
                if (!CanStand) return;
                
                // Stop crouching
                crouching = false;
            }

            // Verify the collider mode
            controller.Collisions.SetColliderMode(
                crouching 
                ? CollisionHandler.ColliderMode.Crouching 
                : CollisionHandler.ColliderMode.Standard
            );
        }
    }
}
