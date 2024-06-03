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
            // Follow the current target
        }
    }
}