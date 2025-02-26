using GoodLuckValley.Events;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Timers;
using GoodLuckValley.Events.Potentiates;

namespace GoodLuckValley.Potentiates
{
    public class TimeWarpStrategy : PotentiateStrategy
    {
        private Potentiate parent;
        private TimeWarpParticleController particles;
        private Optional<PotentiateHandler> storedHandler = Optional<PotentiateHandler>.None();
        private Optional<PlayerController> storedController = Optional<PlayerController>.None();
        private readonly CountdownTimer durationTimer;
        private readonly CountdownTimer cooldownTimer;
        private readonly CountdownTimer timeScaleTimer;
        private readonly CountdownTimer rtpcInTimer;
        private readonly CountdownTimer rtpcOutTimer;

        public TimeWarpStrategy(Potentiate parent, float duration)
        {
            // Set the parent object
            this.parent = parent;

            // Get particles
            particles = parent.GetComponentInChildren<TimeWarpParticleController>();

            // Initialize the duration timer
            durationTimer = new CountdownTimer(duration);

            rtpcInTimer = new CountdownTimer(0.5f);

            rtpcInTimer.OnTimerStart += () =>
            {
                parent.SetPrimaryRTPC(0f);
            };

            rtpcInTimer.OnTimerTick += () =>
            {
                parent.SetPrimaryRTPC(1f - rtpcInTimer.Progress);

            };

            rtpcInTimer.OnTimerStop += () =>
            {
                parent.SetPrimaryRTPC(1f);
            };

            rtpcOutTimer = new CountdownTimer(0.5f);

            rtpcOutTimer.OnTimerStart += () =>
            {
                parent.SetPrimaryRTPC(1f);
            };

            rtpcOutTimer.OnTimerTick += () =>
            {
                parent.SetPrimaryRTPC(rtpcOutTimer.Progress);
            };

            rtpcOutTimer.OnTimerStop += () =>
            {
                parent.SetPrimaryRTPC(0f);
            };

            durationTimer.OnTimerStart += () =>
            {
                // Start the ticking sound
                parent.SetSecondaryRTPC(0f);
                parent.PlaySFX();
            };

            durationTimer.OnTimerTick += () =>
            {
                // Set the ticking RTPC to escalate as the timer progresses
                parent.SetSecondaryRTPC(1f - durationTimer.Progress);
            };

            durationTimer.OnTimerStop += () =>
            {
                // Process the stored controller, if it exists
                storedController.Match(
                    onValue: playerController => {
                        // Remove the time jump
                        playerController.Jump.RemoveTimeJump();

                        // Nullify the player controller
                        playerController = null;
                        return 0;
                    },
                    onNoValue: () => { return 0; }
                );

                storedHandler.Match(
                    onValue: potentiateHandler =>
                    {
                        // Deplete the last potentiate
                        potentiateHandler.RemovePotentiate();

                        // Nullify the potentiate handler
                        potentiateHandler = null;
                        return 0;
                    },
                    onNoValue: () => { return 0; }
                );

                // Reset the secondary RTPC and stop SFX
                parent.StopSFX();
                parent.SetSecondaryRTPC(0f);

                // Start the RTPC Out Timer
                rtpcOutTimer.Start();

                RespawnPotentiate();

                // Set the color for feedback
                EventBus<PotentiateFeedback>.Raise(new PotentiateFeedback()
                {
                    Color = new UnityEngine.Color(1f, 1f, 1f, 1f)
                });
            };

            // Set up the cooldown timer
            cooldownTimer = new CountdownTimer(1f);

            cooldownTimer.OnTimerStop += () => RespawnPotentiate();

            timeScaleTimer = new CountdownTimer(0.25f);

            timeScaleTimer.OnTimerStart += () =>
            {
                // Set the time scale
                UnityEngine.Time.timeScale = 0.75f;
            };

            timeScaleTimer.OnTimerStop += () =>
            {
                UnityEngine.Time.timeScale = 1f;
            };
        }

        ~TimeWarpStrategy()
        {
            // Dispose timers
            durationTimer.Dispose();
            cooldownTimer.Dispose();
            timeScaleTimer.Dispose();
            rtpcInTimer.Dispose();
            rtpcOutTimer.Dispose();
        }

        /// <summary>
        /// Collect the Time Warp and potentiate the Player
        /// </summary>
        public override bool Potentiate(PlayerController controller, PotentiateHandler handler)
        {
            // Exit case - the player already has a time jump
            if (controller.Jump.HasTimeJump)
            {
                // Nullify the stored Player Controller if it exists
                storedController.Match(
                    onValue: playerController => { playerController = null; return 0; },
                    onNoValue: () => { return 0; }
                );

                // Nullify the stored Potentiate Handler if it exists
                storedHandler.Match(
                    onValue: potentiateHandler => { potentiateHandler = null; return 0; },
                    onNoValue: () => { return 0; }
                );

                return false;
            }

            // Set the Player Controller
            storedController = controller;

            // Allow a jump buffer
            storedController.Value.Jump.BufferedJumpUsable = true;

            // Add a time jump
            storedController.Value.Jump.AddTimeJump();

            // Set the Potentiate Handler
            storedHandler = handler;

            // Start the duration timer
            durationTimer.Start();

            // Start the time scale timer
            timeScaleTimer.Start();

            // Set the color for feedback
            EventBus<PotentiateFeedback>.Raise(new PotentiateFeedback()
            {
                Color = new UnityEngine.Color(0.3608f, 0.7216f, 1f, 1f)
            });

            // Start fading in the Time Warp audio effect
            rtpcInTimer.Start();

            // Stop particles
            particles.Stop();

            return true;
        }

        /// <summary>
        /// Deplete the Time Warp
        /// </summary>
        public override void Deplete()
        {
            // Pause (and deregister) the duration timer
            durationTimer.Pause(true);

            // Pause (and deregister) the Time Warp audio timer
            rtpcInTimer.Pause(true);

            // Stop the time scale timer
            timeScaleTimer.Stop();

            // Start the cooldown timer
            cooldownTimer.Start();

            // Set the color for feedback
            EventBus<PotentiateFeedback>.Raise(new PotentiateFeedback()
            {
                Color = new UnityEngine.Color(1f, 1f, 1f, 1f)
            });

            // Start the RTPC Out Timer
            rtpcOutTimer.Start();

            // Reset the secondary RTPC and stop SFX
            parent.StopSFX();
            parent.SetSecondaryRTPC(0f);
        }

        /// <summary>
        /// Respawn the Time Warp
        /// </summary>
        private void RespawnPotentiate()
        {
            // Allow potentiation
            parent.AllowPotentiation();

            // Fade the parent sprite back in
            parent.Fade(1f);

            // Play the particles
            particles.Play();
        }
    }
}
