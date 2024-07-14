using GoodLuckValley.Audio.SFX;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class SlideState : BaseState
    {
        private readonly PlayerSFXMaster sfx;
        private readonly float offset;

        public SlideState(PlayerController player, Animator animator, PlayerSFXMaster sfx) : base(player, animator)
        {
            this.sfx = sfx;
            offset = 0.25f;
        }

        public override void OnEnter()
        {
            player.LearnControl("Slide");

            // TODO: Replace with slide animation
            animator.CrossFade(SlideHash, crossFadeDuration);

            // Set slide transform
            animator.transform.localPosition = new Vector2(animator.transform.localPosition.x - offset, animator.transform.localPosition.y);

            // Allow the player to peek
            player.SetCanPeek(true);

            // Start the slide effect sound
            sfx.StartSlide();
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
            // Reset slide transform
            animator.transform.localPosition = new Vector2(0, 0);

            // Stop the slide effect sound
            sfx.StopSlide();
        }
    }
}