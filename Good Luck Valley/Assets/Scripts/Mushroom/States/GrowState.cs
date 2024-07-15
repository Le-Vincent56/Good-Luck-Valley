using GoodLuckValley.Audio.SFX;
using UnityEngine;

namespace GoodLuckValley.Mushroom.States
{
    public class GrowState : MushroomState
    {
        private float growTimer;
        public bool Finished { get => growTimer <= 0; }
        private readonly MushroomSFXMaster sfx;

        public GrowState(MushroomController mushroom, Animator animator, MushroomSFXMaster sfx) : base(mushroom, animator)
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            // Set the grow timer
            growTimer = 0.4f;

            // Update the sfx switch based on the tile type
            sfx.UpdateSwitch(mushroom.GetSpawnTileType());

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