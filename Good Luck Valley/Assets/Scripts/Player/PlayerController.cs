using GoodLuckValley.Input;
using GoodLuckValley.Extensions.GameObjects;
using GoodLuckValley.Player.Data;
using UnityEngine;

namespace GoodLuckValley.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameInputReader input;
        [SerializeField] private PlayerStats stats;
        private Rigidbody2D rb;
        private ConstantForce2D constForce;

        private BoxCollider2D boxCollider;
        private CapsuleCollider2D airborneCollider;
        private CollisionHandler collisionHandler;

        public Rigidbody2D RB { get => rb; }

        private bool cachedQueryMode;
        private bool cachedQueryTriggers;
        private GeneratedCharacterSize characterSize;
        public const float GRAVITY_SCALE = 1;

        private Vector2 framePosition;

        public PlayerStats Stats { get => stats; }
        public GeneratedCharacterSize CharacterSize { get => characterSize; }
        public bool CachedQueryMode { get => cachedQueryMode; }
        public Vector2 FramePosition { get => framePosition; }
        public Vector2 Up { get; private set; }
        public Vector2 Right { get; private set; }
        public Vector2 Down { get; private set; }
        public Vector2 Left { get; private set; }

        private void Awake()
        {
            constForce = GameObjectExtensions.GetOrAdd<ConstantForce2D>(gameObject);

            // Set up the player controller
            Setup();
        }

        public void OnValidate() => Setup();

        private void Setup()
        {
            // Generate the character size
            characterSize = stats.CharacterSize.GenerateCharacterSize();
            cachedQueryMode = Physics2D.queriesStartInColliders;

            //_wallDetectionBounds = new Bounds(
            //    new Vector3(0, characterSize.Height / 2),
            //    new Vector3(characterSize.StandingColliderSize.x + CharacterSize.COLLIDER_EDGE_RADIUS * 2 + Stats.WallDetectorRange, characterSize.Height - 0.1f));

            // Initialize the Rigidbody2D
            rb = GetComponent<Rigidbody2D>();
            rb.hideFlags = HideFlags.NotEditable;

            // Get the Colliders
            boxCollider = GetComponent<BoxCollider2D>();
            airborneCollider = GetComponent<CapsuleCollider2D>();

            // Initialize the Collision Handler
            collisionHandler ??= new CollisionHandler(this, boxCollider, airborneCollider);
        }
    }
}