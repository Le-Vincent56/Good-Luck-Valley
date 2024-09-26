using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies.States
{
    public class IdleState : FireflyState
    {
        private float waitTimer;
        private float waitTimeMin;
        private float waitTimeMax;

        public IdleState(Firefly firefly, float waitTimeMin, float waitTimeMax) : base(firefly)
        {
            this.waitTimeMin = waitTimeMin;
            this.waitTimeMax = waitTimeMax;
        }

        public override void OnEnter()
        {
            // Set a random wait time
            waitTimer = Random.Range(waitTimeMin, waitTimeMax);
        }

        public override void Update()
        {
            // Subtract from the wait timer
            waitTimer -= Time.deltaTime;

            // Check if the wait timer has expired
            if (waitTimer <= 0)
                // Set the firefly to move
                firefly.SetToMove(true);
        }
    }
}
