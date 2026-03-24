using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Motor;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Motor
{
    [TestFixture]
    public class PlayerMotorTests
    {
        private PlayerMotor _motor;
        private PlayerStats _stats;
        private const float DELTA_TIME = 0.02f;
        private const float GRAVITY_Y = -9.81f;

        [SetUp]
        public void Setup()
        {
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _motor = new PlayerMotor(_stats);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_stats);
        }

        private CollisionData FlatGround(float distance = 0.5f)
        {
            return new CollisionData(
                groundDetected: true, groundDistance: distance,
                groundNormal: Vector2.up, groundAngle: 0f,
                leftWallDetected: false, rightWallDetected: false,
                leftWallDistance: 0f, rightWallDistance: 0f,
                ceilingBlocked: false
            );
        }

        private CollisionData NoCollision()
        {
            return new CollisionData(
                groundDetected: false, groundDistance: 0f,
                groundNormal: Vector2.up, groundAngle: 0f,
                leftWallDetected: false, rightWallDetected: false,
                leftWallDistance: 0f, rightWallDistance: 0f,
                ceilingBlocked: false
            );
        }

        // --- Impulse Path ---

        [Test]
        public void Impulse_AppliedToVelocity()
        {
            _motor.BeginTick();
            _motor.ApplyImpulse(new Vector2(0f, 20f), false, false);

            MotorOutput output = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.AreEqual(20f, output.Velocity.y, 0.01f);
        }

        [Test]
        public void Impulse_WithResetY_ClearsYBeforeApplying()
        {
            _motor.BeginTick();
            _motor.ApplyImpulse(new Vector2(0f, 20f), false, true);

            MotorOutput output = _motor.CalculateVelocity(
                new Vector2(5f, -10f), 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.AreEqual(5f, output.Velocity.x, 0.01f);
            Assert.AreEqual(20f, output.Velocity.y, 0.01f);
        }

        [Test]
        public void Impulse_ShortCircuitsNormalMovement()
        {
            _motor.BeginTick();
            _motor.SetMovementMode(MovementMode.Grounded);
            _motor.ApplyImpulse(new Vector2(0f, 20f), false, true);

            MotorOutput output = _motor.CalculateVelocity(
                Vector2.zero, 
                new Vector2(1f, 0f), 
                FlatGround(),
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.AreEqual(0f, output.Velocity.x, 0.01f);
            Assert.AreEqual(20f, output.Velocity.y, 0.01f);
        }

        // --- Grounded Movement ---

        [Test]
        public void GroundedMovement_WithInput_AcceleratesTowardTarget()
        {
            _motor.BeginTick();
            _motor.SetMovementMode(MovementMode.Grounded);

            MotorOutput output = _motor.CalculateVelocity(
                Vector2.zero, 
                new Vector2(1f, 0f), 
                FlatGround(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.Greater(output.Velocity.x, 0f);
        }

        [Test]
        public void GroundedMovement_NoInput_DeceleratesToZero()
        {
            _motor.BeginTick();
            _motor.SetMovementMode(MovementMode.Grounded);

            MotorOutput output = _motor.CalculateVelocity(
                new Vector2(5f, 0f), 
                Vector2.zero, 
                FlatGround(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.Less(output.Velocity.x, 5f);
        }

        [Test]
        public void GroundedMovement_OpposingDirection_UsesDirectionCorrection()
        {
            _motor.BeginTick();
            _motor.SetMovementMode(MovementMode.Grounded);

            MotorOutput withCorrection = _motor.CalculateVelocity(
                new Vector2(5f, 0f), 
                new Vector2(-1f, 0f), 
                FlatGround(), 
                GRAVITY_Y,
                DELTA_TIME
            );

            _motor.BeginTick();
            MotorOutput withoutCorrection = _motor.CalculateVelocity(
                new Vector2(5f, 0f), 
                Vector2.zero, 
                FlatGround(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.Less(withCorrection.Velocity.x, withoutCorrection.Velocity.x);
        }

        // --- Airborne Movement ---

        [Test]
        public void AirborneMovement_UsesAirFriction()
        {
            _motor.BeginTick();
            _motor.SetMovementMode(MovementMode.Airborne);

            MotorOutput output = _motor.CalculateVelocity(
                Vector2.zero, 
                new Vector2(1f, 0f), 
                NoCollision(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.Greater(output.Velocity.x, 0f);
        }

        // --- Gravity ---

        [Test]
        public void Gravity_AppliedBasedOnScale()
        {
            _motor.BeginTick();
            _motor.SetGravityScale(2.8f);
            _motor.SetMovementMode(MovementMode.Airborne);

            MotorOutput output = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                NoCollision(),
                GRAVITY_Y, 
                DELTA_TIME
            );

            float expectedDelta = GRAVITY_Y * 2.8f * DELTA_TIME;
            Assert.AreEqual(expectedDelta, output.Velocity.y, 0.01f);
        }

        [Test]
        public void Gravity_ZeroWhenGroundedScale()
        {
            _motor.BeginTick();
            _motor.SetGravityScale(0f);
            _motor.SetMovementMode(MovementMode.Grounded);

            MotorOutput output = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                FlatGround(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.AreEqual(0f, output.Velocity.y, 0.01f);
        }

        [Test]
        public void EndJumpEarly_AppliesExtraDownForce_WhenAscending()
        {
            _motor.BeginTick();
            _motor.SetGravityScale(2.8f);
            _motor.SetEndJumpEarlyMultiplier(3f);
            _motor.SetMovementMode(MovementMode.Airborne);

            MotorOutput withEarly = _motor.CalculateVelocity(
                new Vector2(0f, 10f), 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            _motor.BeginTick();
            _motor.ClearEndJumpEarlyMultiplier();

            MotorOutput withoutEarly = _motor.CalculateVelocity(
                new Vector2(0f, 10f), 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.Less(withEarly.Velocity.y, withoutEarly.Velocity.y);
        }

        // --- Speed Caps ---

        [Test]
        public void MaxFallSpeed_CapsDownwardVelocity()
        {
            _motor.BeginTick();
            _motor.SetGravityScale(1.5f);
            _motor.SetMaxFallSpeed(11f);
            _motor.SetMovementMode(MovementMode.Airborne);

            MotorOutput output = _motor.CalculateVelocity(
                new Vector2(0f, -20f), 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.GreaterOrEqual(output.Velocity.y, -11f);
        }

        // --- External Forces ---

        [Test]
        public void PersistentForce_AppliedEachTick()
        {
            ForceHandle handle = _motor.AddForce(new ExternalForce(Vector2.right, 10f));
            _motor.BeginTick();
            _motor.SetMovementMode(MovementMode.Airborne);

            MotorOutput output = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, DELTA_TIME
            );

            Assert.Greater(output.Velocity.x, 0f);
        }

        [Test]
        public void PersistentForce_RemovedByHandle()
        {
            ForceHandle handle = _motor.AddForce(new ExternalForce(Vector2.right, 10f));
            _motor.RemoveForce(handle);
            _motor.BeginTick();
            _motor.SetGravityScale(0f);
            _motor.SetMovementMode(MovementMode.Airborne);

            MotorOutput output = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.AreEqual(0f, output.Velocity.x, 0.01f);
        }

        // --- Parameter Modifiers ---

        [Test]
        public void ParameterModifier_Multiply_AffectsGravity()
        {
            _motor.BeginTick();
            _motor.SetGravityScale(2.8f);
            _motor.SetMovementMode(MovementMode.Airborne);

            MotorOutput baseline = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            ModifierHandle handle = _motor.AddParameterModifier(new ParameterModifier(
                PlayerParameter.GravityScale,
                ModifierOperation.Multiply, 
                0.5f
            ));

            _motor.BeginTick();

            MotorOutput modified = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, 
                DELTA_TIME
            );

            Assert.AreEqual(baseline.Velocity.y * 0.5f, modified.Velocity.y, 0.1f);
        }

        // --- Wall Velocity Override ---

        [Test]
        public void WallVelocityOverride_ReplacesVerticalCalculation()
        {
            _motor.BeginTick();
            _motor.SetGravityScale(2.8f);
            _motor.SetWallVelocityOverride(-3f);
            _motor.SetMovementMode(MovementMode.Airborne);

            MotorOutput output = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, DELTA_TIME
            );

            Assert.AreEqual(-3f, output.Velocity.y, 0.01f);
        }

        // --- Decaying Velocity ---

        [Test]
        public void DecayingVelocity_DecaysOverTime()
        {
            _motor.AddDecayingVelocity(new Vector2(10f, 0f));
            _motor.BeginTick();
            _motor.SetGravityScale(0f);
            _motor.SetMovementMode(MovementMode.Airborne);

            MotorOutput tick1 = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, DELTA_TIME
            );

            _motor.BeginTick();

            MotorOutput tick2 = _motor.CalculateVelocity(
                Vector2.zero, 
                Vector2.zero, 
                NoCollision(), 
                GRAVITY_Y, DELTA_TIME
            );

            Assert.Greater(tick1.Velocity.x, tick2.Velocity.x);
        }
    }
}