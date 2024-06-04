using UnityEngine;


namespace GoodLuckValley.Entities.Fireflies
{
    public class RetreatState : FireflyState
    {
        public RetreatState(FireflyController fireflies, Animator animator) : base(fireflies, animator)
        {
        }

        public override void OnEnter()
        {
            // Clear the current following target
        }

        public override void OnExit()
        {
            // Set the transform to the retreat transform
        }
    }
}