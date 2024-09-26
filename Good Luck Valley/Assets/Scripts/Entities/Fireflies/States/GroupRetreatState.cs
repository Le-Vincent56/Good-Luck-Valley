using UnityEngine;


namespace GoodLuckValley.Entities.Fireflies
{
    public class GroupRetreatState : FireflyControlState
    {
        public GroupRetreatState(FireflyController fireflies, Animator animator) : base(fireflies, animator)
        {
        }

        public override void OnEnter()
        {
            fireflies.Retreat();
        }
    }
}