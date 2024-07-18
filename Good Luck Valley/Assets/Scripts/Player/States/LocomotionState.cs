using GoodLuckValley.Audio.SFX;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class LocomotionState : BaseState
    {
        private readonly PlayerSFXMaster sfx;

        public LocomotionState(PlayerController player, Animator animator, PlayerSFXMaster sfx) : base(player, animator) 
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            Debug.Log("Entering Locomotion State!");

            player.LearnControl("Move");

            animator.CrossFade(LocomotionHash, crossFadeDuration);

            // Allow the player to peek
            player.SetCanPeek(true);

            // Start playing ground impacts sounds with the run value for the speed RTPC
            sfx.SetSpeedRTPC(sfx.RUN);
            sfx.StartGroundImpacts();
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
            Debug.Log("Exiting Locomotion State!");

            // Reset footstep sounds
            sfx.StopGroundImpacts();
        }
    }
}