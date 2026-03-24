using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Handlers;
using GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Handlers
{
    [TestFixture]
    public class PlayerWallHandlerTests
    {
        private PlayerWallHandler _handler;
        private MockPlayerMotor _motor;
        private MockPlayerInput _input;
        private PlayerStats _stats;

        [SetUp]
        public void Setup()
        {
            _motor = new MockPlayerMotor();
            _input = new MockPlayerInput();
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _handler = new PlayerWallHandler(_motor, _input, _stats);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_stats);

        private CollisionData RightWall(float distance = 0.05f)
        {
            return new CollisionData(
                groundDetected: false, groundDistance: 0f,
                groundNormal: Vector2.up, groundAngle: 0f,
                leftWallDetected: false, rightWallDetected: true,
                leftWallDistance: 0f, rightWallDistance: distance,
                ceilingBlocked: false);
        }

        private CollisionData LeftWall(float distance = 0.05f)
        {
            return new CollisionData(
                groundDetected: false, groundDistance: 0f,
                groundNormal: Vector2.up, groundAngle: 0f,
                leftWallDetected: true, rightWallDetected: false,
                leftWallDistance: distance, rightWallDistance: 0f,
                ceilingBlocked: false);
        }

        private CollisionData NoCollision()
        {
            return new CollisionData(
                groundDetected: false, groundDistance: 0f,
                groundNormal: Vector2.up, groundAngle: 0f,
                leftWallDetected: false, rightWallDetected: false,
                leftWallDistance: 0f, rightWallDistance: 0f,
                ceilingBlocked: false);
        }

        // --- ShouldAttachToWall ---

        [Test]
        public void ShouldAttachToWall_WhenWallDetected_NotGrounded_CooldownExpired()
        {
            _input.Move = new Vector2(1f, 0f);

            bool result = _handler.ShouldAttachToWall(RightWall(), _input.Move, false);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldAttachToWall_False_WhenGrounded()
        {
            _input.Move = new Vector2(1f, 0f);

            bool result = _handler.ShouldAttachToWall(RightWall(), _input.Move, true);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldAttachToWall_False_DuringReattachCooldown()
        {
            _input.Move = new Vector2(1f, 0f);

            _handler.AttachToWall(1);
            _handler.DetachFromWall();
            _handler.Tick(0.1f, 0.1f);

            bool result = _handler.ShouldAttachToWall(RightWall(), _input.Move, false);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldAttachToWall_True_AfterCooldownExpired()
        {
            _input.Move = new Vector2(1f, 0f);

            _handler.AttachToWall(1);
            _handler.DetachFromWall();
            _handler.Tick(0.25f, 0.25f);

            bool result = _handler.ShouldAttachToWall(RightWall(), _input.Move, false);

            Assert.IsTrue(result);
        }

        // --- Wall Jump ---

        [Test]
        public void ExecuteWallJump_AppliesImpulseAwayFromWall()
        {
            _handler.AttachToWall(1);

            _handler.ExecuteWallJump();

            Assert.AreEqual(1, _motor.ImpulseCount);
            Assert.AreEqual(-1f * _stats.WallJumpPowerX, _motor.LastImpulse.x, 0.01f);
            Assert.AreEqual(_stats.WallJumpPowerY, _motor.LastImpulse.y, 0.01f);
            Assert.IsTrue(_motor.LastResetX);
            Assert.IsTrue(_motor.LastResetY);
        }

        [Test]
        public void ExecuteWallJump_DetachesFromWall()
        {
            _handler.AttachToWall(1);

            _handler.ExecuteWallJump();

            Assert.IsFalse(_handler.IsOnWall);
            Assert.IsTrue(_handler.IsWallJumping);
        }

        // --- Input Nerf ---

        [Test]
        public void AdjustMoveInput_ReversesInput_DuringLossWindow()
        {
            _handler.AttachToWall(1);
            _handler.ExecuteWallJump();
            _handler.Tick(0.01f, 0.01f);

            Vector2 adjusted = _handler.AdjustMoveInput(new Vector2(1f, 0f));

            Assert.Less(adjusted.x, 0f);
        }

        [Test]
        public void AdjustMoveInput_GraduallyRestoresInput_DuringRecovery()
        {
            _handler.AttachToWall(1);
            _handler.ExecuteWallJump();

            // Past total input loss, into recovery
            _handler.Tick(_stats.WallJumpTotalInputLossTime + 0.25f,
                _stats.WallJumpTotalInputLossTime + 0.25f);

            Vector2 adjusted = _handler.AdjustMoveInput(new Vector2(1f, 0f));

            // Should be partially restored but not fully
            Assert.Greater(adjusted.x, -1f);
            Assert.Less(adjusted.x, 1f);
        }

        [Test]
        public void AdjustMoveInput_FullyRestored_AfterRecoveryComplete()
        {
            _handler.AttachToWall(1);
            _handler.ExecuteWallJump();

            float totalTime = _stats.WallJumpTotalInputLossTime +
                              _stats.WallJumpInputLossReturnTime + 0.1f;
            _handler.Tick(totalTime, totalTime);

            Vector2 adjusted = _handler.AdjustMoveInput(new Vector2(1f, 0f));

            Assert.AreEqual(1f, adjusted.x, 0.01f);
        }

        // --- Wall Coyote Time ---

        [Test]
        public void HasWallCoyoteTime_TrueAfterDetach_FalseAfterExpiry()
        {
            _handler.AttachToWall(1);
            _handler.DetachFromWall();
            _handler.Tick(0.1f, 0.1f);

            Assert.IsTrue(_handler.HasWallCoyoteTime);

            _handler.Tick(0.25f, 0.35f);

            Assert.IsFalse(_handler.HasWallCoyoteTime);
        }

        // --- Wall Velocity ---

        [Test]
        public void CalculateWallVelocity_ClimbsDown_WhenHoldingDown()
        {
            _handler.AttachToWall(1);

            float yVel = _handler.CalculateWallVelocity(
                new Vector2(0f, -0.5f), 0f, 0.02f);

            Assert.Less(yVel, 0f);
        }

        [Test]
        public void CalculateWallVelocity_FallsWithAcceleration_WhenNoInput()
        {
            _handler.AttachToWall(1);

            float yVel = _handler.CalculateWallVelocity(
                Vector2.zero, 0f, 0.02f);

            Assert.Less(yVel, 0f);
        }

        // --- NotifyGrounded ---

        [Test]
        public void NotifyGrounded_ClearsWallState()
        {
            _handler.AttachToWall(1);
            _handler.ExecuteWallJump();

            _handler.NotifyGrounded();

            Assert.IsFalse(_handler.IsOnWall);
            Assert.IsFalse(_handler.IsWallJumping);
        }
    }
}