using GoodLuckValley.Audio.SFX;
using UnityEngine;

namespace GoodLuckValley.Mushroom.States
{
    public class DissipateState : MushroomState
    {
        private float dissipateTimer;
        public bool Finished { get => dissipateTimer <= 0; }
        private readonly MushroomSFXMaster sfx;

        public DissipateState(MushroomController mushroom, Animator animator, MushroomSFXMaster sfx) : base(mushroom, animator)
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            // Start the animation and set the dissipate timer
            animator.CrossFade(DissipateHash, crossFadeDuration);
            dissipateTimer = animator.GetCurrentAnimatorClipInfo(0).Length;

            // Play the bounce sound effect
            sfx.Dissipate();
        }

        public override void Update()
        {
            // Update timers
            if (dissipateTimer > 0)
                dissipateTimer -= Time.deltaTime;

            // If the animation is finished, destroy the game object
            if (Finished) Object.Destroy(mushroom.gameObject);
        }
    }
}