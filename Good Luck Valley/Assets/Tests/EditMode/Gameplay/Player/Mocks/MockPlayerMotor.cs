using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;
using GoodLuckValley.Gameplay.Player.Motor;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks
{
    public class MockPlayerMotor : IPlayerMotor, IPlayerForceReceiver
    {
        public float GravityScale { get; private set; }
        public float MaxFallSpeed { get; private set; } = -1f;
        public MovementMode CurrentMovementMode { get; private set; }
        public float EndJumpEarlyMultiplier { get; private set; }
        public bool HasWallOverride { get; private set; }
        public float WallYVelocity { get; private set; }

        public Vector2 LastImpulse { get; private set; }
        public bool LastResetX { get; private set; }
        public bool LastResetY { get; private set; }
        public int ImpulseCount { get; private set; }

        public List<ExternalForce> ActiveForces { get; } = new List<ExternalForce>();
        public List<ParameterModifier> ActiveModifiers { get; } = new List<ParameterModifier>();

        private int _nextForceID;
        private int _nextModifierID;

        public void SetGravityScale(float scale) => GravityScale = scale;
        public void SetMaxFallSpeed(float maxSpeed) => MaxFallSpeed = maxSpeed;
        public void ClearMaxFallSpeed() => MaxFallSpeed = -1f;
        public void SetMovementMode(MovementMode mode) => CurrentMovementMode = mode;

        public void ApplyImpulse(Vector2 impulse, bool resetVelocityX, bool resetVelocityY)
        {
            LastImpulse = impulse;
            LastResetX = resetVelocityX;
            LastResetY = resetVelocityY;
            ImpulseCount++;
        }

        public void SetEndJumpEarlyMultiplier(float multiplier) => EndJumpEarlyMultiplier = multiplier;

        public void ClearEndJumpEarlyMultiplier() => EndJumpEarlyMultiplier = 0f;

        public void SetWallVelocityOverride(float yVelocity)
        {
            WallYVelocity = yVelocity;
            HasWallOverride = true;
        }

        public void ClearWallVelocityOverride() => HasWallOverride = false;

        public void BeginTick() { }

        public MotorOutput CalculateVelocity(
            Vector2 currentVelocity, 
            Vector2 moveInput,
            CollisionData collision, 
            float globalGravityY, 
            float deltaTime
        )
        {
            return new MotorOutput(currentVelocity);
        }

        public ForceHandle AddForce(ExternalForce force)
        {
            ActiveForces.Add(force);
            return new ForceHandle(_nextForceID++);
        }

        public void RemoveForce(ForceHandle handle) { }

        public void ApplyImpulse(Vector2 impulse) => ApplyImpulse(impulse, false, false);

        public void AddDecayingVelocity(Vector2 velocity) { }

        public ModifierHandle AddParameterModifier(ParameterModifier modifier)
        {
            ActiveModifiers.Add(modifier);
            return new ModifierHandle(_nextModifierID++);
        }

        public void RemoveParameterModifier(ModifierHandle handle) { }
    }
}