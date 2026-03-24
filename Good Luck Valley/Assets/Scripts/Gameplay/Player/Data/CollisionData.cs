using UnityEngine;

namespace GoodLuckValley.Gameplay.Player.Data
{
    /// <summary>
    /// Represents the collision detection data for an object, including information about ground,
    /// walls, and ceiling. This struct provides details such as distances, angles, and detection flags
    /// for determining the state of collision.
    /// </summary>
    public readonly struct CollisionData
    {
        public readonly bool GroundDetected;
        public readonly float GroundDistance;
        public readonly Vector2 GroundNormal;
        public readonly float GroundAngle;
        public readonly bool LeftWallDetected;
        public readonly bool RightWallDetected;
        public readonly float LeftWallDistance;
        public readonly float RightWallDistance;
        public readonly bool CeilingBlocked;

        public CollisionData(
            bool groundDetected, 
            float groundDistance, 
            Vector2 groundNormal, 
            float groundAngle,
            bool leftWallDetected, 
            bool rightWallDetected,
            float leftWallDistance, 
            float rightWallDistance,
            bool ceilingBlocked
        )
        {
            GroundDetected = groundDetected;
            GroundDistance = groundDistance;
            GroundNormal = groundNormal;
            GroundAngle = groundAngle;
            LeftWallDetected = leftWallDetected;
            RightWallDetected = rightWallDetected;
            LeftWallDistance = leftWallDistance;
            RightWallDistance = rightWallDistance;
            CeilingBlocked = ceilingBlocked;
        }
    }
}