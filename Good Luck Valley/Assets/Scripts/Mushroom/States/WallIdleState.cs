using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.States
{
    public class WallIdleState : WallMushroomState
    {
        public WallIdleState(MushroomController mushroom, Animator animator) : base(mushroom, animator)
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