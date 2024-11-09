using System;
using UnityEngine;

namespace GoodLuckValley.Player.Movement
{
    [Serializable]
    public class PlayerSlide
    {
        private PlayerController controller;
        [SerializeField] private bool sliding;
        [SerializeField] private bool forcedSlide;
        [SerializeField] private bool manualSlide;
        [SerializeField] private bool slideJumping;

        public bool Sliding { get => sliding; set => sliding = value; }
        public bool SlidePressed { get => controller.FrameData.Input.Sliding; }
        public bool SlideJumping { get => slideJumping; set => slideJumping = value; }

        public PlayerSlide(PlayerController controller)
        {
            this.controller = controller;
        }

        public void CalculateSliding()
        {
            // Exit case - not grounded
            if(!controller.Collisions.Grounded)
            {
                sliding = false;
                forcedSlide = false;
                manualSlide = false;
                return;
            }

            // Get the angle of the ground
            float groundAngle = Vector2.Angle(controller.Collisions.GroundHit.normal, controller.Up);

            // Exit case - there's no slope to sli deon
            if (groundAngle == 0)
            {
                sliding = false;
                manualSlide = false;
                forcedSlide = false;
                return;
            } 
            // Check if the angle is greater than or equal to the max walkable slope angle
            else if (groundAngle >= controller.Stats.MaxWalkableSlope)
            {
                // Force a slide
                forcedSlide = true;
                sliding = true;
            } 
            // Otherwise, check if the angle is less than the max walkable slope angle
            else if (groundAngle < controller.Stats.MaxWalkableSlope)
            {
                // Don't force a slide
                forcedSlide = false;
                sliding = false;
            }

            // Check if not sliding, not forcing a slide, and the slide button is pressed
            if (!sliding && !forcedSlide && SlidePressed)
            {
                // Start a manual slide
                manualSlide = true;
                sliding = true;
            } 
            // Otherwise, check if already sliding, not forcing a slide, and the slide button is not pressed
            else if (sliding && !forcedSlide && !SlidePressed)
            {
                // Stop the manual slide
                sliding = false;
                manualSlide = false;
            }

            // Exit case - not sliding at all
            if (!sliding) return;

            // Get the direction to slide in
            Vector2 slideDirection = new Vector2(controller.Collisions.GroundNormal.x, -controller.Collisions.GroundNormal.y);

            // Exit case - enforcing the forced slide
            if(forcedSlide)
            {
                // Add the forced slide force
                controller.FrameData.AddForce(slideDirection.normalized * controller.Stats.ForcedSlideForce);
                return;
            }
            
            // Check if manual sliding
            if(manualSlide)
            {
                // Add the manual slide force
                controller.FrameData.AddForce(slideDirection.normalized * controller.Stats.ManualSlideForce);
            }
        }
    }
}
