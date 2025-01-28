using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class LocomotionState : SubState
    {
        public LocomotionState(PlayerController controller, AnimationController animator, ParticleController particles)
            : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            // Enter the locomotion animation
            animator.EnterLocomotion();

            // Play running particles
            particles.PlayRunningParticles();
        }

        public override void OnExit()
        {
            // Stop playing running particles
            particles.StopRunningParticles();
        }
    }
}
