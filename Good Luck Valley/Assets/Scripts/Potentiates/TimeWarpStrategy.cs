using GoodLuckValley.Architecture.EventBus;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Timers;

namespace GoodLuckValley.Potentiates
{
    public class TimeWarpStrategy : PotentiateStrategy
    {
        private Potentiate parent;
        private Optional<PotentiateHandler> storedHandler = Optional<PotentiateHandler>.None();
        private Optional<PlayerController> storedController = Optional<PlayerController>.None();
        private CountdownTimer durationTimer;
        private CountdownTimer cooldownTimer;

        public TimeWarpStrategy(Potentiate parent, float duration)
        {
            // Set the parent object
            this.parent = parent;

            // Initialize the duration timer
            durationTimer = new CountdownTimer(duration);

            durationTimer.OnTimerStart += () =>
            {
                // TODO: Start the ticking sound
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
        }

        ~TimeWarpStrategy()
        {
            // Dispose of the duration timer
            durationTimer.Dispose();
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

            // Set the color for feedback
            EventBus<PotentiateFeedback>.Raise(new PotentiateFeedback()
            {
                Color = new UnityEngine.Color(0.3608f, 0.7216f, 1f, 1f)
            });

            return true;
        }

        /// <summary>
        /// Deplete the Time Warp
        /// </summary>
        public override void Deplete()
        {
            // Pause (and deregister) the duration timer
            durationTimer.Pause(true);

            // Start the cooldown timer
            cooldownTimer.Start();

            // Set the color for feedback
            EventBus<PotentiateFeedback>.Raise(new PotentiateFeedback()
            {
                Color = new UnityEngine.Color(1f, 1f, 1f, 1f)
            });
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
        }
    }
}
