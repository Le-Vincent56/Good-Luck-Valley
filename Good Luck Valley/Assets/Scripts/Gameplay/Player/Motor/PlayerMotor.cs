using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.Motor
{
    /// <summary>
    /// The PlayerMotor class encapsulates player motor mechanics, including movement handling,
    /// gravity scaling, force application, and collision adjustments. It combines input processing
    /// and physical properties to control how the player moves and interacts with the environment.
    /// </summary>
    public class PlayerMotor : IPlayerMotor, IPlayerForceReceiver
    {
        private const float SlopeBlendThreshold = 0.7f;

        private readonly PlayerStats _stats;

        // Persistent state (cross-tick)
        private readonly List<ActiveForce> _externalForces = new List<ActiveForce>();
        private readonly List<ActiveModifier> _parameterModifiers = new List<ActiveModifier>();

        private Vector2 _decayingVelocity;
        private int _nextForceID;
        private int _nextModifierID;

        // Per-state configuration
        private float _gravityScale;
        private float _maxFallSpeed = -1f;
        private MovementMode _movementMode;
        private float _endJumpEarlyMultiplier;

        // Per-tick accumulation
        private Vector2 _pendingImpulse;
        private bool _hasImpulse;
        private bool _resetVelocityX;
        private bool _resetVelocityY;
        private float _wallYVelocity;
        private bool _hasWallVelocityOverride;

        /// <summary>
        /// Represents an active external force applied to the player motor and its associated identifier.
        /// </summary>
        private struct ActiveForce
        {
            public int ID;
            public ExternalForce Force;
        }

        /// <summary>
        /// Represents a modifier that is actively applied to a player's parameter.
        /// </summary>
        private struct ActiveModifier
        {
            public int ID;
            public ParameterModifier Modifier;
        }

        public PlayerMotor(PlayerStats stats) => _stats = stats;
        
        /// <summary>
        /// Adjusts the gravity scale for the player motor, modifying the effect of gravity on the player's movement.
        /// </summary>
        /// <param name="scale">The gravity scale to apply. Values greater than 1 increase gravitational pull,
        /// while values between 0 and 1 reduce it. A value of 0 negates gravity entirely.</param>
        public void SetGravityScale(float scale) => _gravityScale = scale;

        /// <summary>
        /// Sets the maximum fall speed limit for the player motor, restricting the player's downward velocity to the specified value.
        /// </summary>
        /// <param name="maxSpeed">The maximum downward speed the player can achieve. A value greater than zero is ignored, as fall speed is downward.</param>
        public void SetMaxFallSpeed(float maxSpeed) => _maxFallSpeed = maxSpeed;

        /// <summary>
        /// Clears the maximum fall speed limit for the player motor, effectively removing any restriction
        /// on the player's downward velocity.
        /// </summary>
        public void ClearMaxFallSpeed() => _maxFallSpeed = -1f;

        /// <summary>
        /// Sets the current movement mode of the player motor. The movement mode determines
        /// how the player's movement behaves, such as whether the player is grounded or airborne.
        /// </summary>
        /// <param name="mode">The movement mode to set. Possible values are defined in the <see cref="MovementMode"/> enum.</param>
        public void SetMovementMode(MovementMode mode) => _movementMode = mode;

        /// <summary>
        /// Applies an impulse to the player motor, influencing its velocity.
        /// This method can also reset specific components of the velocity before applying the impulse.
        /// </summary>
        /// <param name="impulse">The vector representing the impulse to apply. The impulse affects the direction and magnitude of the velocity change.</param>
        /// <param name="resetVelocityX">Indicates whether the current x-axis velocity should be reset before applying the impulse.</param>
        /// <param name="resetVelocityY">Indicates whether the current y-axis velocity should be reset before applying the impulse.</param>
        public void ApplyImpulse(Vector2 impulse, bool resetVelocityX, bool resetVelocityY)
        {
            _pendingImpulse += impulse;
            _hasImpulse = true;
            _resetVelocityX |= resetVelocityX;
            _resetVelocityY |= resetVelocityY;
        }

        /// <summary>
        /// Sets the multiplier that modifies the downward force applied when a jump is ended early.
        /// This multiplier influences how strongly the player's upward motion is dampened during an early jump termination.
        /// </summary>
        /// <param name="multiplier">The multiplier to apply to the downward force. A higher value results in a stronger downward pull.</param>
        public void SetEndJumpEarlyMultiplier(float multiplier) => _endJumpEarlyMultiplier = multiplier;

        /// <summary>
        /// Clears the multiplier that modifies the downward force applied when a jump is ended early.
        /// Resets the multiplier to its default value, effectively disabling any previously set early jump behavior.
        /// </summary>
        public void ClearEndJumpEarlyMultiplier() => _endJumpEarlyMultiplier = 0f;

        /// <summary>
        /// Sets an override for the player's vertical wall velocity.
        /// Applies a specified y-velocity value, replacing the default vertical behavior
        /// when the override is active.
        /// </summary>
        /// <param name="yVelocity">The y-velocity to apply as the override.</param>
        public void SetWallVelocityOverride(float yVelocity)
        {
            _wallYVelocity = yVelocity;
            _hasWallVelocityOverride = true;
        }

        /// <summary>
        /// Removes any active override on the player's wall velocity.
        /// Resets the internal flag tracking wall velocity overrides, ensuring that
        /// default wall interaction behavior is restored.
        /// </summary>
        public void ClearWallVelocityOverride() => _hasWallVelocityOverride = false;

        /// <summary>
        /// Initializes the player motor state at the start of a simulation tick.
        /// Resets any pending forces, impulses, or velocity overrides that may influence
        /// the player's movement during this frame.
        /// </summary>
        public void BeginTick()
        {
            _pendingImpulse = Vector2.zero;
            _hasImpulse = false;
            _resetVelocityX = false;
            _resetVelocityY = false;
            _hasWallVelocityOverride = false;
        }

        /// <summary>
        /// Calculates the updated velocity of the player motor based on the current velocity, movement input,
        /// environmental factors, and external forces.
        /// </summary>
        /// <param name="currentVelocity">The current velocity of the player motor.</param>
        /// <param name="moveInput">The input values representing the player's movement intent.</param>
        /// <param name="collision">The collision data representing information about physical interactions with the environment.</param>
        /// <param name="globalGravityY">The global gravitational force applied on the Y-axis.</param>
        /// <param name="deltaTime">The time elapsed since the last frame, used for frame-dependent calculations.</param>
        /// <returns>A <see cref="MotorOutput"/> structure containing the calculated velocity of the player motor.</returns>
        public MotorOutput CalculateVelocity(
            Vector2 currentVelocity,
            Vector2 moveInput,
            CollisionData collision,
            float globalGravityY,
            float deltaTime
        )
        {
            Vector2 velocity = currentVelocity;

            // 1. Impulse path (short-circuits normal movement)
            if (_hasImpulse)
            {
                if (_resetVelocityX) velocity.x = 0f;
                if (_resetVelocityY) velocity.y = 0f;
                velocity += _pendingImpulse;
                velocity += _decayingVelocity;
                DecayTransientVelocity(velocity, deltaTime);
                ClearPendingImpulse();
                return new MotorOutput(velocity);
            }

            // 2. Horizontal movement
            if (!_hasWallVelocityOverride)
            {
                if (_movementMode == MovementMode.Grounded)
                {
                    velocity = CalculateGroundedMovement(
                        velocity, 
                        moveInput, 
                        collision,
                        deltaTime
                    );
                }
                else
                {
                    velocity.x = CalculateAirborneMovement(
                        velocity.x,
                        moveInput.x,
                        deltaTime
                    );
                }
            }

            // 3. Vertical movement
            if (_hasWallVelocityOverride)
            {
                velocity.y = _wallYVelocity;
            }
            else
            {
                float effectiveGravityScale = GetEffectiveParameter(PlayerParameter.GravityScale, _gravityScale);
                velocity.y += globalGravityY * effectiveGravityScale * deltaTime;

                if (_endJumpEarlyMultiplier > 0f && velocity.y > 0f)
                    velocity.y -= _endJumpEarlyMultiplier * deltaTime;
            }

            // 4. External forces
            for (int i = 0; i < _externalForces.Count; i++)
            {
                ExternalForce force = _externalForces[i].Force;
                velocity += force.Direction * force.Magnitude * deltaTime;
            }

            velocity += _decayingVelocity;
            DecayTransientVelocity(velocity, deltaTime);

            // 5. Speed caps
            if (_maxFallSpeed >= 0f && velocity.y < -_maxFallSpeed)
                velocity.y = -_maxFallSpeed;

            return new MotorOutput(velocity);
        }
        
        /// <summary>
        /// Adds an external force to the player motor and returns a handle allowing for future operations on that force.
        /// </summary>
        /// <param name="force">
        /// The external force to be added, which contains the properties of the force being applied.
        /// </param>
        /// <returns>
        /// A handle identifying the added force, used for managing or removing the force later.
        /// </returns>
        public ForceHandle AddForce(ExternalForce force)
        {
            int id = _nextForceID++;
            _externalForces.Add(new ActiveForce { ID = id, Force = force });
            return new ForceHandle(id);
        }

        /// <summary>
        /// Removes a specific external force from the player motor using the provided force handle.
        /// </summary>
        /// <param name="handle">
        /// The handle used to identify the specific force to be removed from the active forces.
        /// </param>
        public void RemoveForce(ForceHandle handle)
        {
            for (int i = _externalForces.Count - 1; i >= 0; i--)
            {
                if (_externalForces[i].ID != handle.ID) continue;
                
                _externalForces.RemoveAt(i);
                return;
            }
        }

        /// <summary>
        /// Applies an impulse to the player motor, modifying the player's velocity instantaneously.
        /// This method allows specifying whether to reset the X and/or Y velocity before applying the impulse.
        /// </summary>
        /// <param name="impulse">
        /// The 2D vector representing the impulse force to be applied, including both magnitude and direction.
        /// </param>
        public void ApplyImpulse(Vector2 impulse) => ApplyImpulse(impulse, false, false);

        /// <summary>
        /// Adds a decaying velocity to the player motor, which gradually reduces over time.
        /// This velocity influences the player's movement and diminishes based on internal decay logic.
        /// </summary>
        /// <param name="velocity">
        /// The velocity to be added. This is represented as a vector, with both magnitude and direction.
        /// </param>
        public void AddDecayingVelocity(Vector2 velocity) => _decayingVelocity += velocity;

        /// <summary>
        /// Adds a parameter modifier to the player motor, affecting behavior such as gravity scale or movement speed.
        /// The modifier influences the player's parameters and is stored for later removal or adjustment.
        /// </summary>
        /// <param name="modifier">
        /// The parameter modifier to be added. This defines the parameter to be modified, the operation to apply,
        /// and the factor or value for the modification.
        /// </param>
        /// <returns>
        /// A handle that uniquely represents the added modifier. This handle can later be used to remove
        /// or identify the modifier.
        /// </returns>
        public ModifierHandle AddParameterModifier(ParameterModifier modifier)
        {
            int id = _nextModifierID++;
            _parameterModifiers.Add(new ActiveModifier
            {
                ID = id, 
                Modifier = modifier
            });
            return new ModifierHandle(id);
        }

        /// <summary>
        /// Removes a parameter modifier associated with the given handle from the player motor.
        /// The modifier is identified using its unique handle ID and is removed from the active modifiers list.
        /// </summary>
        /// <param name="handle">
        /// The handle representing the modifier to be removed. The handle contains a unique identifier (ID) used to locate and remove the corresponding modifier.
        /// </param>
        public void RemoveParameterModifier(ModifierHandle handle)
        {
            for (int i = _parameterModifiers.Count - 1; i >= 0; i--)
            {
                if (_parameterModifiers[i].ID != handle.ID) continue;
                
                _parameterModifiers.RemoveAt(i);
                return;
            }
        }


        /// <summary>
        /// Calculates the grounded movement velocity of the player based on input, collision data,
        /// effective movement parameters such as speed, acceleration, and friction, while accounting
        /// for slope adjustments and ground conditions. Ensures smooth transitions and control over
        /// ground movement dynamics.
        /// </summary>
        /// <param name="velocity">The current velocity of the player.</param>
        /// <param name="moveInput">
        /// The direction and intensity of the player's movement input, typically in the range [-1, 1] for horizontal movement.
        /// </param>
        /// <param name="collision">
        /// Data representing the collision state of the player, including ground detection,
        /// slope information, and ground normals.
        /// </param>
        /// <param name="deltaTime">
        /// The elapsed time in seconds since the last update, used to calculate interpolated movement adjustments.
        /// </param>
        /// <returns>
        /// The updated velocity of the player after applying grounded movement adjustments based on input,
        /// ground conditions, and effective movement parameters.
        /// </returns>
        private Vector2 CalculateGroundedMovement(
            Vector2 velocity,
            Vector2 moveInput,
            CollisionData collision,
            float deltaTime
        )
        {
            float effectiveSpeed = GetEffectiveParameter(PlayerParameter.MovementSpeed, _stats.BaseSpeed);
            float hasInput = Mathf.Abs(moveInput.x) > 0.01f ? 1f : 0f;
            float targetSpeed = hasInput * effectiveSpeed;
            float effectiveAcceleration = GetEffectiveParameter(PlayerParameter.Acceleration, _stats.Acceleration);
            float effectiveFriction = GetEffectiveParameter(PlayerParameter.Friction, _stats.Friction);

            float step = hasInput > 0f ? effectiveAcceleration : effectiveFriction;

            // Direction correction: faster turnaround when input opposes velocity
            if (hasInput > 0f && Mathf.Abs(velocity.x) > 0.01f && !Mathf.Approximately(Mathf.Sign(moveInput.x), Mathf.Sign(velocity.x)))
                step *= _stats.DirectionCorrectionMultiplier;

            // Slope-adjusted target direction
            Vector2 targetDirection = new Vector2(moveInput.x, 0f);
            if (collision is { GroundDetected: true, GroundAngle: > 0.01f } && collision.GroundAngle <= _stats.MaxWalkableSlope)
                targetDirection.y = moveInput.x * -collision.GroundNormal.x / collision.GroundNormal.y;

            Vector2 targetVelocity = targetDirection.sqrMagnitude > 0.001f
                ? targetDirection.normalized * targetSpeed
                : Vector2.zero;

            // Blend smooth vs direct based on slope angle
            float slopePoint = Mathf.InverseLerp(0f, _stats.MaxWalkableSlope, collision.GroundAngle);

            if (slopePoint > SlopeBlendThreshold)
            {
                velocity = Vector2.MoveTowards(velocity, targetVelocity, step * deltaTime);
            }
            else
            {
                Vector2 smooth = new Vector2(
                    Mathf.MoveTowards(velocity.x, targetVelocity.x, step * deltaTime),
                    Mathf.MoveTowards(velocity.y, targetVelocity.y, step * deltaTime)
                );
                Vector2 direct = Vector2.MoveTowards(
                    velocity, 
                    targetVelocity, 
                    step * deltaTime
                );
                float blend = slopePoint / SlopeBlendThreshold;
                velocity = Vector2.Lerp(smooth, direct, blend);
            }

            return velocity;
        }

        /// <summary>
        /// Calculates the horizontal velocity adjustment for airborne movement by considering
        /// the target speed, air friction, and acceleration. This method ensures that the
        /// horizontal movement transitions smoothly towards the target velocity based on the
        /// given input and elapsed time.
        /// </summary>
        /// <param name="currentX">
        /// The current horizontal velocity of the player.
        /// </param>
        /// <param name="moveInputX">
        /// The horizontal input provided by the player, typically in the range [-1, 1].
        /// </param>
        /// <param name="deltaTime">
        /// The elapsed time in seconds since the last update, controlling the step size
        /// for acceleration and movement adjustments.
        /// </param>
        /// <returns>
        /// The updated horizontal velocity after applying calculated adjustments for
        /// acceleration and air friction.
        /// </returns>
        private float CalculateAirborneMovement(
            float currentX, 
            float moveInputX, 
            float deltaTime
        )
        {
            float effectiveSpeed = GetEffectiveParameter(PlayerParameter.MovementSpeed, _stats.BaseSpeed);
            float targetSpeed = moveInputX * effectiveSpeed;
            float effectiveAirFriction = GetEffectiveParameter(PlayerParameter.AirFriction, _stats.AirFrictionMultiplier);
            float effectiveAcceleration = GetEffectiveParameter(PlayerParameter.Acceleration, _stats.Acceleration);
            float step = effectiveAcceleration * effectiveAirFriction;

            return Mathf.MoveTowards(currentX, targetSpeed, step * deltaTime);
        }

        /// <summary>
        /// Calculates the effective value of a specific player parameter by applying active modifiers.
        /// Modifiers can be applied as additive, multiplicative, or overridden operations. The final value
        /// is the result of sequentially applying all matching modifiers to the base value.
        /// </summary>
        /// <param name="parameter">
        /// Specifies the player parameter for which the effective value is calculated. Each parameter
        /// corresponds to a specific attribute of the player motor (e.g., gravity scale, movement speed).
        /// </param>
        /// <param name="baseValue">
        /// The base value of the parameter before any modifiers are applied.
        /// This serves as the starting point for modifier computations.
        /// </param>
        /// <returns>
        /// The effective value of the specified player parameter after applying all matching modifiers.
        /// This value reflects the modified state based on in-game conditions and active operations.
        /// </returns>
        private float GetEffectiveParameter(PlayerParameter parameter, float baseValue)
        {
            float result = baseValue;

            for (int i = 0; i < _parameterModifiers.Count; i++)
            {
                ActiveModifier active = _parameterModifiers[i];
                if (active.Modifier.Parameter != parameter) continue;

                switch (active.Modifier.Operation)
                {
                    case ModifierOperation.Add:
                        result += active.Modifier.Value;
                        break;
                    
                    case ModifierOperation.Multiply:
                        result *= active.Modifier.Value;
                        break;
                    
                    case ModifierOperation.Override:
                        result = active.Modifier.Value;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gradually reduces the transient velocity applied to the player motor over time.
        /// This method adjusts the decaying velocity vector based on friction, air friction, and
        /// external velocity decay rate, ensuring a consistent reduction. It applies an accelerated
        /// decay if the transient velocity opposes the current movement direction.
        /// </summary>
        /// <param name="currentVelocity">
        /// The current velocity of the player motor,
        /// used to determine the decay behavior of the transient velocity.
        /// </param>
        /// <param name="deltaTime">
        /// The time elapsed since the last frame,
        /// used to calculate the rate of velocity decay.
        /// </param>
        private void DecayTransientVelocity(Vector2 currentVelocity, float deltaTime)
        {
            if (_decayingVelocity.sqrMagnitude < 0.001f)
            {
                _decayingVelocity = Vector2.zero;
                return;
            }

            float decayRate = _stats.Friction * _stats.AirFrictionMultiplier * _stats.ExternalVelocityDecayRate;

            // 5x faster decay when opposing movement direction
            if (Vector2.Dot(_decayingVelocity, currentVelocity) < 0f)
                decayRate *= 5f;

            _decayingVelocity = Vector2.MoveTowards(
                _decayingVelocity, 
                Vector2.zero,
                decayRate * deltaTime
            );
        }

        /// <summary>
        /// Clears any pending impulse applied to the player motor.
        /// This method resets the pending impulse vector to zero, disables the impulse flag,
        /// and resets the flags responsible for determining if the horizontal or vertical velocity
        /// should be overridden. It ensures that no impulse forces will be applied in the subsequent
        /// velocity calculations.
        /// </summary>
        private void ClearPendingImpulse()
        {
            _pendingImpulse = Vector2.zero;
            _hasImpulse = false;
            _resetVelocityX = false;
            _resetVelocityY = false;
        }
    }
}