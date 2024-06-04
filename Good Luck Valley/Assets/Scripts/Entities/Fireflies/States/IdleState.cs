using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies
{
    public class IdleState : FireflyState
    {
        public IdleState(FireflyController fireflies, Animator animator) : base(fireflies, animator)
        {
        }

        public override void OnEnter()
        {
            fireflies.CheckPlaced();
        }

        public override void Update()
        {
            fireflies.CheckPlaced();
        }
    }
}