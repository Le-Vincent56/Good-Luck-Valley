using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies.States
{
    public class FireflyIdleState : FireflyState
    {
        private readonly CountdownTimer waitTimer;
        private readonly float waitTimeMin;
        private readonly float waitTimeMax;

        public FireflyIdleState(Firefly firefly, float waitTimeMin, float waitTimeMax) : base(firefly)
        {
            this.waitTimeMin = waitTimeMin;
            this.waitTimeMax = waitTimeMax;

            // Create the wait Countdown Timer
            waitTimer = new CountdownTimer(waitTimeMin);
            waitTimer.OnTimerStop += () =>
            {
                // Set the Firefly to wander
                firefly.Wandering = true;
                firefly.Idle = false;
            };
        }

        ~FireflyIdleState()
        {
            // Dispose of the wait Countdown Timer
            waitTimer.Dispose();
        }

        public override void OnEnter()
        {
            // Set a random wait time and start the timer
            waitTimer.Reset(Random.Range(waitTimeMin, waitTimeMax));
            waitTimer.Start();
        }
    }
}
