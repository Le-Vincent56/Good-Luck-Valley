using UnityEngine;

namespace GoodLuckValley.Mushroom.States
{
    public class BounceState : MushroomState
    {
        private float animationTimer;
        public bool Finished { get => animationTimer <= 0; }

        public BounceState(MushroomController mushroom, Animator animator) : base(mushroom, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(BounceHash, crossFadeDuration);
            animationTimer = animator.GetCurrentAnimatorStateInfo(0).length;
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