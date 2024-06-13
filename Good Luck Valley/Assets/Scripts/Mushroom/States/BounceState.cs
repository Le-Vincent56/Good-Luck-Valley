using UnityEngine;

namespace GoodLuckValley.Mushroom.States
{
    public class BounceState : MushroomState
    {
        private float animationTimer;
        public bool Finished { get => animationTimer <= 0; }
        private readonly MushroomSFXHandler sfx;

        public BounceState(MushroomController mushroom, Animator animator, MushroomSFXHandler sfx) : base(mushroom, animator)
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            animator.CrossFade(BounceHash, crossFadeDuration);
            animationTimer = animator.GetCurrentAnimatorStateInfo(0).length;

            // Play the bounce sound effect
            sfx.Bounce();
        }

        public override void Update()
        {
            // Update timers
            if (animationTimer > 0)
                animationTimer -= Time.deltaTime;

            // If the animation is finished, reset the bounce
            if (Finished) mushroom.ResetBounce();
        }

        public override void FixedUpdate()
        {
            // Handle collisions
            mushroom.HandleCollisions();
        }
    }

}