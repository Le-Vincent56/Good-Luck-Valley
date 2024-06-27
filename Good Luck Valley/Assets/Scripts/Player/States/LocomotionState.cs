using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class LocomotionState : BaseState
    {
        private readonly PlayerSFXHandler sfx;

        public LocomotionState(PlayerController player, Animator animator, PlayerSFXHandler sfx) : base(player, animator) 
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            player.LearnControl("Move");

            animator.CrossFade(LocomotionHash, crossFadeDuration);

            // Allow the player to peek
            player.SetCanPeek(true);
        }

        public override void Update()
        {
            // Update footstep sounds
            sfx.Footsteps();
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
        }
    }
}