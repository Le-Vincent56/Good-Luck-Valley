using UnityEngine;

namespace GoodLuckValley.Mushroom.States
{
    public class BounceState : MushroomState
    {
        private float animationTimer;
        public bool Finished { get => animationTimer > 0; }

        public BounceState(MushroomController mushroom, Animator animator) : base(mushroom, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(BounceHash, crossFadeDuration);
            animationTimer = animator.GetCurrentAnimatorClipInfo(0).Length;
        }

        public override void Update()
        {
            // Update timers
            if (animationTimer > 0)
                animationTimer -= Time.deltaTime;
        }

        public override void FixedUpdate()
        {
            mushroom.CheckCollisions();

            // Handle collisions
            mushroom.HandleCollisions();
        }

        public override void OnExit()
        {
            // Reset the bounce
            mushroom.ResetBounce();
        }
    }

}