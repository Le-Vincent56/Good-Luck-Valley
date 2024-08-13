using UnityEngine;

namespace GoodLuckValley.Mushroom.States
{
    public class IdleState : MushroomState
    {
        public IdleState(MushroomController mushroom, Animator animator) : base(mushroom, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            // Check collisions
            mushroom.CheckCollisions();

            // Handle collisions
            mushroom.HandleCollisions();
        }
    }
}