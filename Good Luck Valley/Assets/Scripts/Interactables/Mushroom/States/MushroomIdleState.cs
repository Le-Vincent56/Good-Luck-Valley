using UnityEngine;

namespace GoodLuckValley.Interactables.Mushroom.States
{
    public class MushroomIdleState : MushroomPickupState
    {
        public MushroomIdleState(MushroomPickup pickup, Animator animator, ParticleSystem particleSystem) 
            : base(pickup, animator, particleSystem)
        { }

        public override void OnEnter() => animator.CrossFade(IDLE_HASH, FADE_DURATION);
    }
}
