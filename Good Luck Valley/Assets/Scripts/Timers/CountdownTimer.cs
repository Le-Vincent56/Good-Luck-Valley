using UnityEngine;

namespace GoodLuckValley.Timers
{
    public class CountdownTimer : Timer
    {
        public override bool IsFinished => CurrentTime <= 0;

        public CountdownTimer(float value) : base(value) { }

        public override void Tick()
        {
            // If currently running and the time is greater than 0, reduce the time
            if (IsRunning && CurrentTime > 0) CurrentTime -= Time.deltaTime;

            // If currently running and the time is equal to or below 0, stop the Timer
            if (IsRunning && CurrentTime <= 0) Stop();
        }
    }
}