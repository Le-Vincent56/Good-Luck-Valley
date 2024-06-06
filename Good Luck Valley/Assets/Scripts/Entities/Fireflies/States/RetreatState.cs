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
            fireflies.Retreat();
        }
    }
}