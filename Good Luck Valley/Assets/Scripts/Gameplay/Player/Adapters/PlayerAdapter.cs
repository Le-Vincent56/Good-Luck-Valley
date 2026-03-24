using UnityEngine;
using GoodLuckValley.Core.DI.Attributes;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.Adapters
{
    /// <summary>
    /// Adapts and represents player-specific data and traits within the gameplay framework.
    /// </summary>
    public class PlayerAdapter : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private PlayerStats stats;
        [SerializeField] private CharacterSize characterSize;

        [Header("Components")] 
        [SerializeField] private Rigidbody2D rb;

        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private CapsuleCollider2D capsuleCollider;

        [Inject] private IPlayerService _playerService;

        private ColliderMode _lastColliderMode;

        public PlayerStats Stats => stats;
        public CharacterSize CharacterSize => characterSize;

        private void Start()
        {
            rb.gravityScale = 0f;
            ConfigureColliders();
            UpdateColliderMode(ColliderMode.Standing);
        }

        private void Update() => _playerService?.Update(Time.deltaTime);

        private void FixedUpdate()
        {
            if (_playerService == null) return;
            
            CollisionData collision = PerformDetection();

            Motor.MotorOutput output = _playerService.FixedUpdate(
                Time.fixedDeltaTime,
                Time.fixedTime,
                collision,
                rb.linearVelocity,
                Physics2D.gravity.y
            );

            rb.linearVelocity = output.Velocity;

            ColliderMode currentMode = _playerService.CurrentColliderMode;

            if (currentMode == _lastColliderMode) return;
            
            UpdateColliderMode(currentMode);
        }

        /// <summary>
        /// Performs collision detection for the player character, including ground, wall, and ceiling checks.
        /// Uses raycasts, box casts, and overlap checks to detect the environment and calculates
        /// properties such as ground distance, wall distances, and whether the ceiling is blocked.
        /// </summary>
        /// <returns>
        /// A <see cref="CollisionData"/> struct containing details of the detected collisions, including
        /// positional and contact information relevant to ground, walls, and ceiling obstacles.
        /// </returns>
        private CollisionData PerformDetection()
        {
            Vector2 position = rb.position;
            float stepHeight = characterSize.StepHeight;
            float width = characterSize.Width;
            float height = characterSize.Height;
            float rayInset = characterSize.RayInset;

            // Ground detection — multi-ray downward from step height
            Vector2 rayOrigin = position + Vector2.up * stepHeight;
            float rayLength = stepHeight + 0.1f;

            bool groundDetected = false;
            float groundDistance = float.MaxValue;
            Vector2 groundNormal = Vector2.up;
            float groundAngle = 0f;

            float halfWidth = width / 2f - rayInset;
            Vector2[] rayOrigins = new Vector2[]
            {
                rayOrigin,
                rayOrigin + Vector2.left * halfWidth,
                rayOrigin + Vector2.right * halfWidth
            };

            for (int i = 0; i < rayOrigins.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    rayOrigins[i], 
                    Vector2.down, 
                    rayLength, 
                    stats.CollisionLayers
                );

                if (!hit.collider) continue;
                if (groundDetected && !(hit.distance < groundDistance)) continue;
                    
                groundDetected = true;
                groundDistance = hit.distance;
                groundNormal = hit.normal;
                groundAngle = Vector2.Angle(Vector2.up, hit.normal);
            }

            // Wall detection — box casts left and right
            float wallRange = stats.WallDetectorRange;
            Vector2 boxSize = new Vector2(0.05f, height * 0.5f);
            Vector2 boxCenter = position + Vector2.up * (height * 0.5f);

            RaycastHit2D leftWallHit = Physics2D.BoxCast(
                boxCenter,
                boxSize, 
                0f, 
                Vector2.left, 
                wallRange + width / 2f,
                stats.ClimbableLayer
            );
            RaycastHit2D rightWallHit = Physics2D.BoxCast(
                boxCenter, 
                boxSize, 
                0f, 
                Vector2.right, 
                wallRange + width / 2f,
                stats.ClimbableLayer
            );

            bool leftWallDetected = leftWallHit.collider;
            bool rightWallDetected = rightWallHit.collider;
            float leftWallDistance = leftWallDetected ? leftWallHit.distance : 0f;
            float rightWallDistance = rightWallDetected ? rightWallHit.distance : 0f;

            // Ceiling detection — overlap check at standing size
            Vector2 ceilingCheckCenter = position + Vector2.up * height;
            Vector2 ceilingCheckSize = new Vector2(
                width - characterSize.SkinWidth * 2f,
                characterSize.SkinWidth * 2f
            );

            bool ceilingBlocked = Physics2D.OverlapBox(
                ceilingCheckCenter, 
                ceilingCheckSize, 
                0f, 
                stats.CeilingLayers
            );

            return new CollisionData(
                groundDetected, groundDistance, groundNormal, groundAngle,
                leftWallDetected, rightWallDetected,
                leftWallDistance, rightWallDistance,
                ceilingBlocked);
        }

        /// <summary>
        /// Configures the initial sizes, offsets, and edge radius for the player's colliders.
        /// Adjusts the box collider for standing configuration and the capsule collider for airborne configuration
        /// based on the <see cref="CharacterSize"/> data.
        /// </summary>
        private void ConfigureColliders()
        {
            // Standing box collider
            boxCollider.size = characterSize.StandingColliderSize;
            boxCollider.offset = characterSize.StandingColliderCenter;
            boxCollider.edgeRadius = characterSize.ColliderEdgeRadius;

            // Airborne capsule collider
            capsuleCollider.size = characterSize.AirborneColliderSize;
            capsuleCollider.offset = characterSize.AirborneColliderOffset;
        }

        /// <summary>
        /// Updates the player's collider setup based on the specified collider mode.
        /// </summary>
        /// <param name="mode">The desired <see cref="ColliderMode"/> to switch to.
        /// Determines whether the collider is set to standing, crawling, or airborne configuration.</param>
        private void UpdateColliderMode(ColliderMode mode)
        {
            _lastColliderMode = mode;

            switch (mode)
            {
                case ColliderMode.Standing:
                    boxCollider.enabled = true;
                    capsuleCollider.enabled = false;
                    boxCollider.size = characterSize.StandingColliderSize;
                    boxCollider.offset = characterSize.StandingColliderCenter;
                    break;

                case ColliderMode.Crawling:
                    boxCollider.enabled = true;
                    capsuleCollider.enabled = false;
                    boxCollider.size = characterSize.CrouchColliderSize;
                    boxCollider.offset = characterSize.CrouchColliderCenter;
                    break;

                case ColliderMode.Airborne:
                    boxCollider.enabled = false;
                    capsuleCollider.enabled = true;
                    break;
            }
        }
    }
}