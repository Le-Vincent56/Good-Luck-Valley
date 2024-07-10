using GoodLuckValley.Audio.SFX;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class ThrowLocomotionState : BaseState
    {
        private readonly PlayerSFXMaster sfx;
        private float animationTimer;
        private bool Finished { get => animationTimer <= 0; }

        public ThrowLocomotionState(PlayerController player, Animator animator, PlayerSFXMaster sfx) : base(player, animator)
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            animator.CrossFade(ThrowLocomotionHash, crossFadeDuration);
            animationTimer = 0.2f;

            // Allow the player to peek
            player.SetCanPeek(true);

            // Play the throwing sound
            sfx.SporeThrow();

            // Set throwing again to false
            player.SetThrowingAgain(false);
        }

        public override void Update()
        {
            // Update footstep sounds
            sfx.Footsteps();

            // Update timers
            if (animationTimer > 0)
                animationTimer -= Time.deltaTime;

            // If the animation is finished, stop throwing
            if (Finished) player.SetThrow(false);
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
            // Reset footstep sounds
            sfx.ResetFootsteps();

            // Set throwing to false
            player.SetThrow(false);
        }
    }
}