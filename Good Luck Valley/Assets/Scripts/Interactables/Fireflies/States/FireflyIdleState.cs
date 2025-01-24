using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies.States
{
    public class FireflyIdleState : FireflyState
    {
        private readonly CountdownTimer waitTimer;
        private readonly float waitTimeMin;
        private readonly float waitTimeMax;

        private float bounceOffset;
        private float bounceSpeed;
        private float bounceAmplitude;

        public FireflyIdleState(Firefly firefly, float waitTimeMin, float waitTimeMax, float bounceSpeed, float bounceAmplitude) : base(firefly)
        {
            this.waitTimeMin = waitTimeMin;
            this.waitTimeMax = waitTimeMax;
            this.bounceSpeed = bounceSpeed;
            this.bounceAmplitude = bounceAmplitude;

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

            // Reset the bounce offset
            bounceOffset = 0f;
        }

        public override void Update()
        {
            // Calculate the bounce effect
            bounceOffset = Mathf.Sin(Time.time * bounceSpeed) * bounceAmplitude;

            // Apply the bounce effect
            firefly.transform.position += Vector3.up * bounceOffset;
        }
    }
}
