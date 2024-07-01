using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class SlideState : BaseState
    {
        private readonly PlayerSFXHandler sfx;

        public SlideState(PlayerController player, Animator animator, PlayerSFXHandler sfx) : base(player, animator)
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            player.LearnControl("Slide");

            // TODO: Replace with slide animation
            animator.CrossFade(SlideHash, crossFadeDuration);

            // Set slide transform
            animator.transform.localPosition = new Vector2(animator.transform.localPosition.x - 0.25f, animator.transform.localPosition.y);

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
            // Reset slide transform
            animator.transform.localPosition = new Vector2(animator.transform.localPosition.x + 0.25f, animator.transform.localPosition.y);

            // TODO: (Maybe) reset slide sound
        }
    }
}