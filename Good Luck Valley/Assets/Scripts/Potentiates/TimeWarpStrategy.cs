using GoodLuckValley.Events;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Timers;
using GoodLuckValley.Events.Potentiates;
using GoodLuckValley.Events.Player;

namespace GoodLuckValley.Potentiates
{
    public class TimeWarpStrategy : PotentiateStrategy
    {
        private Potentiate parent;
        private TimeWarpParticleController particles;
        private Optional<PotentiateHandler> storedHandler = Optional<PotentiateHandler>.None();
        private Optional<PlayerController> storedController = Optional<PlayerController>.None();
        private readonly CountdownTimer cooldownTimer;

        public TimeWarpStrategy(Potentiate parent, float duration)
        {
            // Set the parent object
            this.parent = parent;

            // Get particles
            particles = parent.GetComponentInChildren<TimeWarpParticleController>();

            // Set up the cooldown timer
            cooldownTimer = new CountdownTimer(1f);
            cooldownTimer.OnTimerStop += () => RespawnPotentiate();
        }

        ~TimeWarpStrategy()
        {
            // Dispose timers
            cooldownTimer.Dispose();
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

            // Nullify gravity
            EventBus<TimeWarpCollected>.Raise(new TimeWarpCollected()
            {
                Entering = true
            });

            return true;
        }

        public override void OnExit(PlayerController controller, PotentiateHandler handler)
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

            // Check if the cooldown timer is not running
            if(!cooldownTimer.IsRunning)
                // Allow potentiation
                parent.AllowPotentiation();
            
            // Nullify gravity
            EventBus<TimeWarpCollected>.Raise(new TimeWarpCollected()
            {
                Entering = false
            });
        }

        /// <summary>
        /// Deplete the Time Warp
        /// </summary>
        public override void Deplete()
        {
            // Start the cooldown timer
            cooldownTimer.Start();

            // Nullify gravity
            EventBus<TimeWarpCollected>.Raise(new TimeWarpCollected()
            {
                Entering = false
            });

            // Burst the particles
            particles.Burst();
        }

        /// <summary>
        /// Respawn the Time Warp
        /// </summary>
        private void RespawnPotentiate()
        {
            // Allow potentiation
            parent.AllowPotentiation();
        }
    }
}
