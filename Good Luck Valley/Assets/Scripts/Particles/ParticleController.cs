using GoodLuckValley.Input;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Timers;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace GoodLuckValley.Particles
{
    public class ParticleController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private LayerDetection layerDetection;
        [SerializeField] private ParticleSystem runningParticles;
        [SerializeField] private ParticleSystem jumpingParticles;

        [SerializeField] private Sprite grassParticleSprite;
        [SerializeField] private Sprite dirtParticleSprite;
        [SerializeField] private Sprite stoneParticleSprite;

        [SerializeField] private Color grassMinColor;
        [SerializeField] private Color grassMaxColor;

        [SerializeField] private Color dirtMinColor;
        [SerializeField] private Color dirtMaxColor;

        private FrequencyTimer runParticleTimer;

        [Header("Fields")]
        [SerializeField] private bool runningParticlesActive;
        [SerializeField] private float runParticleInterval;
        [SerializeField] private float initialRunScaleX;
        [SerializeField] private float initialJumpScaleX;
        [SerializeField] private float initialJumpVelocityX;
        [SerializeField] private int direction;

        private void Awake()
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            runningParticles = particleSystems[0];
            jumpingParticles = particleSystems[1];

            // Get components
            layerDetection = GetComponentInParent<LayerDetection>();

            // Initialize the Run Particle Timer
            runParticleTimer = new FrequencyTimer(runParticleInterval);
            runParticleTimer.OnTick += PlayRunningParticles;

            // Set initial scales
            initialRunScaleX = runningParticles.transform.localScale.x;
            initialJumpScaleX = jumpingParticles.transform.localScale.x;

            // Set initial velocities
            initialJumpVelocityX = jumpingParticles.velocityOverLifetime.x.constant;

            UpdateDirection(Vector2.zero, true);
        }

        private void OnEnable()
        {
            inputReader.Move += UpdateDirection;
            layerDetection.OnGroundLayerChange += SetParticleGroundLayer;
            layerDetection.OnWallTypeChange += SetParticleWallLayer;
        }

        private void OnDisable()
        {
            inputReader.Move -= UpdateDirection;
            layerDetection.OnGroundLayerChange -= SetParticleGroundLayer;
            layerDetection.OnWallTypeChange -= SetParticleWallLayer;
        }

        private void SetParticleGroundLayer(GroundType groundType)
        {
            // Get the color gradient
            MinMaxGradient colorGradient = runningParticles.main.startColor;

            // Get the texture module
            TextureSheetAnimationModule textureModule = runningParticles.textureSheetAnimation;

            // Set the Sprite
            textureModule.SetSprite(0, groundType switch
            {
                GroundType.Grass => grassParticleSprite,
                GroundType.Dirt => dirtParticleSprite,
                GroundType.Stone => stoneParticleSprite,
                _ => null
            });

            switch(groundType)
            {
                case GroundType.Grass:
                    colorGradient = new MinMaxGradient(grassMinColor, grassMaxColor);
                    textureModule.SetSprite(0, grassParticleSprite);
                    break;

                case GroundType.Dirt:
                    colorGradient = new MinMaxGradient(dirtMinColor, dirtMaxColor);
                    textureModule.SetSprite(0, dirtParticleSprite);
                    break;

                case GroundType.Stone:
                    colorGradient.colorMin = Color.white;
                    colorGradient.colorMax = Color.white;
                    textureModule.SetSprite(0, stoneParticleSprite);
                    break;

                default:
                    break;
            }
        }

        private void SetParticleWallLayer(WallType wallType)
        {

        }

        private void OnDestroy()
        {
            // Dispose of the Run Particle Timer
            runParticleTimer.Dispose();
        }

        /// <summary>
        /// Update the particle directions
        /// </summary>
        private void UpdateDirection(Vector2 direction, bool started)
        {
            // Set the direction
            this.direction = (direction.x != 0) ? (int)Mathf.Sign(direction.x) : 0;

            // Update the directions of the particles
            UpdateDirections();
        }

        /// <summary>
        /// Play the running particles
        /// </summary>
        public void PlayRunningParticles()
        {
            // Play the running particle
            runningParticles.Play();

            // Start the frequency timer
            runParticleTimer.Start();
        }

        /// <summary>
        /// Stop the running particles
        /// </summary>
        public void StopRunningParticles()
        {
            // Stop the frequency timer
            runParticleTimer.Stop();
        }

        /// <summary>
        /// Play the jumping particles
        /// </summary>
        public void PlayJumpParticles() => jumpingParticles.Play();

        /// <summary>
        /// Update the directions of the particles
        /// </summary>
        private void UpdateDirections()
        {
            // Set the velocity of the jumping particles
            VelocityOverLifetimeModule velocityModule = jumpingParticles.velocityOverLifetime;
            float jumpVelocityX = (direction != 0) ? initialJumpVelocityX : 0;
            velocityModule.x = new MinMaxCurve(jumpVelocityX);

            // Exit case - if direction is 0
            if (direction == 0) return;

            // Get the scales
            Vector3 runLocalScale = runningParticles.transform.localScale;
            Vector3 jumpLocalScale = jumpingParticles.transform.localScale;

            // Modify the scales by the direction of movement
            runLocalScale.x = initialRunScaleX * direction;
            jumpLocalScale.x = initialJumpScaleX * direction;

            // Set the scales
            runningParticles.transform.localScale = runLocalScale;
            jumpingParticles.transform.localScale = jumpLocalScale;
        }
    }
}
