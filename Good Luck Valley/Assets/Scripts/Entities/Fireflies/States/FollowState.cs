using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies
{
    public class FollowState : FireflyState
    {
        public FollowState(FireflyController fireflies, Animator animator) : base(fireflies, animator)
        {
        }

        public override void FixedUpdate()
        {
            fireflies.HandleMovement();
        }

        public override void OnExit()
        {
            // If pending placed, then that means the fireflies are following a lantern,
            // so set the lantern as the new retreat target and set the following
            // target to none
            if (fireflies.IsPendingPlaced)
                fireflies.SetRetreatTarget();
        }
    }
}