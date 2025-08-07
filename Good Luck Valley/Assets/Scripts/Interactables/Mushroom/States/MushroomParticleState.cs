using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley.Interactables.Mushroom.States
{
    public class MushroomParticleState : MushroomPickupState
    {
        private CountdownTimer bounceTimer;
        
        public MushroomParticleState(MushroomPickup pickup, Animator animator, ParticleSystem particleSystem) 
            : base(pickup, animator, particleSystem)
        { }

        public override void OnEnter()
        {
            animator.CrossFade(EFFECT_HASH, FADE_DURATION);
            particleSystem.Play();
            
            // Create a timer that sets the mushroom to idle after the animation
            bounceTimer = new CountdownTimer(animator.GetCurrentAnimatorStateInfo(0).length);
            bounceTimer.OnTimerStop += () => pickup.ResetTrigger();
            bounceTimer.Start();
        }
        
        public override void OnExit()
        {
            // Dispose the Timer
            bounceTimer.Dispose();
        }
    }
}
