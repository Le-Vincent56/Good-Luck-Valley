using System;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Motor;
using GoodLuckValley.Gameplay.Player.States;

namespace GoodLuckValley.Gameplay.Player.Interfaces
{
    /// <summary>
    /// Defines the contract for services that manage the player's movement, state,
    /// and interaction with the environment in the gameplay system.
    /// </summary>
    public interface IPlayerService
    {
        Vector2 Position { get; }
        Vector2 Velocity { get; }
        bool IsGrounded { get; }
        ColliderMode CurrentColliderMode { get; }
        IPlayerForceReceiver ForceReceiver { get; }
        IPlayerAbilityControl AbilityControl { get; }
        
        event Action<IPlayerState, IPlayerState> OnStateChanged;

        MotorOutput FixedUpdate(
            float deltaTime, 
            float currentTime,
            CollisionData collision, 
            Vector2 currentVelocity, 
            float globalGravityY
        );
        void Update(float deltaTime);
    }
}