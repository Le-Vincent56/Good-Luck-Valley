using UnityEngine;

namespace GoodLuckValley.Gameplay.Player.Motor
{
    /// <summary>
    /// Represents a force acting on an object with a specific direction and magnitude.
    /// </summary>
    public readonly struct ExternalForce
    {
        public readonly Vector2 Direction;
        public readonly float Magnitude;

        public ExternalForce(Vector2 direction, float magnitude)
        {
            Direction = direction;
            Magnitude = magnitude;
        }
    }
}