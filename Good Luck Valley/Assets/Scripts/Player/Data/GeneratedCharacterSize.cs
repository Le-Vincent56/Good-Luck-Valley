using System;
using UnityEngine;

namespace GoodLuckValley.Player.Data
{
    [Serializable]
    public struct GeneratedCharacterSize
    {
        // Standing
        public float Height;
        public float Width;
        public float StepHeight;
        public float RayInset;
        public Vector2 StandingColliderSize;
        public Vector2 StandingColliderCenter;

        // Crouching
        public Vector2 CrouchColliderSize;
        public float CrouchingHeight;
        public Vector2 CrouchingColliderCenter;
    }
}
