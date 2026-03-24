using UnityEngine;

namespace GoodLuckValley.Gameplay.Player.Data
{
    /// <summary>
    /// A ScriptableObject that defines character size-related properties.
    /// This class is used to configure dimensions such as height, width,
    /// step height, crouch height, and various collider settings for a character.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterSize", menuName = "Good Luck Valley/Player/Character Size")]
    public class CharacterSize : ScriptableObject
    {
        [SerializeField] private float height = 1.8f;
        [SerializeField] private float width = 0.6f;
        [SerializeField] private float stepHeight = 0.5f;
        [SerializeField] private float crouchHeightPercent = 0.6f;
        [SerializeField] private float rayInset = 0.065f;
        [SerializeField] private float skinWidth = 0.02f;
        [SerializeField] private float colliderEdgeRadius = 0.05f;

        public float Height => height;
        public float Width => width;
        public float StepHeight => stepHeight;
        public float CrouchHeight => height * crouchHeightPercent;
        public float RayInset => rayInset;
        public float SkinWidth => skinWidth;
        public float ColliderEdgeRadius => colliderEdgeRadius;

        public Vector2 StandingColliderSize => new Vector2(
            width - 2f * colliderEdgeRadius,
            height - stepHeight - 2f * colliderEdgeRadius
        );

        public Vector2 StandingColliderCenter => new Vector2(
            0f,
            height - StandingColliderSize.y / 2f - colliderEdgeRadius
        );

        public Vector2 CrouchColliderSize => new Vector2(
            width - 2f * colliderEdgeRadius,
            CrouchHeight - stepHeight - 2f * colliderEdgeRadius
        );

        public Vector2 CrouchColliderCenter => new Vector2(
            0f,
            CrouchHeight - CrouchColliderSize.y / 2f - colliderEdgeRadius
        );

        public Vector2 AirborneColliderSize => new Vector2(
            width - 2f * skinWidth,
            height - 2f * skinWidth
        );

        public Vector2 AirborneColliderOffset => new Vector2(0f, height / 2f);
    }
}