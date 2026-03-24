using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Handlers;
using GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Handlers
{
    [TestFixture]
    public class PlayerJumpHandlerTests
    {
        private PlayerJumpHandler _handler;
        private MockPlayerMotor _motor;
        private MockPlayerInput _input;
        private PlayerStats _stats;

        [SetUp]
        public void Setup()
        {
            _motor = new MockPlayerMotor();
            _input = new MockPlayerInput();
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _handler = new PlayerJumpHandler(_motor, _input, _stats);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_stats);

        // --- Coyote Time ---

        [Test]
        public void HasCoyoteTime_TrueAfterLeftGround_FalseAfterExpiry()
        {
            _handler.NotifyLeftGround();
            _handler.Tick(0.05f, 0.05f);

            Assert.IsTrue(_handler.HasCoyoteTime);

            _handler.Tick(0.15f, 0.2f);

            Assert.IsFalse(_handler.HasCoyoteTime);
        }

        [Test]
        public void HasCoyoteTime_FalseByDefault()
        {
            Assert.IsFalse(_handler.HasCoyoteTime);
        }

        // --- Jump Clearance ---

        [Test]
        public void IsInJumpClearance_TrueAfterJumpExecuted_FalseAfterExpiry()
        {
            _handler.NotifyJumpExecuted(JumpType.Normal);
            _handler.Tick(0.1f, 0.1f);

            Assert.IsTrue(_handler.IsInJumpClearance);

            _handler.Tick(0.2f, 0.3f);

            Assert.IsFalse(_handler.IsInJumpClearance);
        }

        // --- Execute Normal Jump ---

        [Test]
        public void ExecuteNormalJump_AppliesUpwardImpulseWithResetY()
        {
            _handler.ExecuteNormalJump();

            Assert.AreEqual(1, _motor.ImpulseCount);
            Assert.AreEqual(new Vector2(0f, _stats.JumpPower), _motor.LastImpulse);
            Assert.IsTrue(_motor.LastResetY);
            Assert.IsFalse(_motor.LastResetX);
        }

        // --- Air Jumps ---

        [Test]
        public void ExecuteAirJump_ConsumesAirJump_ReducesCount()
        {
            _handler.NotifyGrounded();
            int before = _handler.AirJumpsRemaining;

            _handler.ExecuteAirJump();

            Assert.AreEqual(before - 1, _handler.AirJumpsRemaining);
            Assert.AreEqual(1, _motor.ImpulseCount);
        }

        [Test]
        public void HasAirJump_FalseWhenAllConsumed()
        {
            _handler.NotifyGrounded();

            for (int i = 0; i < _stats.MaxAirJumps; i++)
            {
                _handler.ExecuteAirJump();
            }

            Assert.IsFalse(_handler.HasAirJump);
        }

        [Test]
        public void NotifyGrounded_RestoresAirJumps()
        {
            _handler.NotifyGrounded();
            _handler.ExecuteAirJump();

            _handler.NotifyGrounded();

            Assert.AreEqual(_stats.MaxAirJumps, _handler.AirJumpsRemaining);
        }

        // --- Restore/Grant Air Jumps ---

        [Test]
        public void RestoreAirJump_AddsOneUpToMax()
        {
            _handler.NotifyGrounded();
            _handler.ExecuteAirJump();
            int after = _handler.AirJumpsRemaining;

            _handler.RestoreAirJump();

            Assert.AreEqual(after + 1, _handler.AirJumpsRemaining);
        }

        [Test]
        public void RestoreAirJump_DoesNotExceedMax()
        {
            _handler.NotifyGrounded();

            _handler.RestoreAirJump();

            Assert.AreEqual(_stats.MaxAirJumps, _handler.AirJumpsRemaining);
        }

        [Test]
        public void GrantAirJumps_AddsExactCount()
        {
            _handler.GrantAirJumps(3);

            Assert.AreEqual(3, _handler.AirJumpsRemaining);
        }

        // --- Ended Jump Early ---

        [Test]
        public void NotifyJumpReleased_SetsEndedEarly_WhenAscending()
        {
            _input.JumpHeld = false;
            _handler.NotifyJumpExecuted(JumpType.Normal);

            _handler.NotifyJumpReleased();

            Assert.IsTrue(_handler.EndedJumpEarly);
        }

        [Test]
        public void NotifyJumpReleased_DoesNotSetEndedEarly_WhenJumpHeld()
        {
            _input.JumpHeld = true;
            _handler.NotifyJumpExecuted(JumpType.Normal);

            _handler.NotifyJumpReleased();

            Assert.IsFalse(_handler.EndedJumpEarly);
        }

        [Test]
        public void NotifyGrounded_ClearsEndedEarly()
        {
            _input.JumpHeld = false;
            _handler.NotifyJumpExecuted(JumpType.Normal);
            _handler.NotifyJumpReleased();

            _handler.NotifyGrounded();

            Assert.IsFalse(_handler.EndedJumpEarly);
        }

        // --- Jump Type ---

        [Test]
        public void LastJumpType_TracksTypeCorrectly()
        {
            _handler.NotifyJumpExecuted(JumpType.Wall);

            Assert.AreEqual(JumpType.Wall, _handler.LastJumpType);
        }

        [Test]
        public void LastJumpType_DefaultsToNone()
        {
            Assert.AreEqual(JumpType.None, _handler.LastJumpType);
        }
    }
}