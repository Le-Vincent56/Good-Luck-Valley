using UnityEngine;
using GoodLuckValley.Gameplay.Player.Motor;

namespace GoodLuckValley.Gameplay.Player.Interfaces
{
    /// <summary>
    /// Defines the contract for receiving and managing forces applied to a player,
    /// including external forces, impulses, and parameter modifications.
    /// </summary>
    public interface IPlayerForceReceiver
    {
        ForceHandle AddForce(ExternalForce force);
        void RemoveForce(ForceHandle handle);
        void ApplyImpulse(Vector2 impulse);
        void AddDecayingVelocity(Vector2 velocity);
        ModifierHandle AddParameterModifier(ParameterModifier modifier);
        void RemoveParameterModifier(ModifierHandle handle);
    }
}