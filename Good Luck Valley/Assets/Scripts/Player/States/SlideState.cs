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
            animator.CrossFade(LocomotionHash, crossFadeDuration);

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