using UnityEngine;

namespace GoodLuckValley.Mushroom.States
{
    public class GrowState : MushroomState
    {
        private float growTimer;
        public bool Finished { get => growTimer <= 0; }
        private readonly MushroomSFXHandler sfx;

        public GrowState(MushroomController mushroom, Animator animator, MushroomSFXHandler sfx) : base(mushroom, animator)
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            // Set the grow timer
            growTimer = 0.4f;

            // Play the bounce sound effect
            sfx.Grow();
        }

        public override void Update()
        {
            // Update timers
            if (growTimer > 0)
                growTimer -= Time.deltaTime;

            // If the animation is finished, reset the bounce
            if (Finished) mushroom.StopGrowing();
        }

        public override void FixedUpdate()
        {
            // Check collisions
            mushroom.CheckCollisions();

            // Handle collisions
            mushroom.HandleCollisions();
        }
    }
}