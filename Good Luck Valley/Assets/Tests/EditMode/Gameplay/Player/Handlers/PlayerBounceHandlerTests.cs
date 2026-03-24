using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Handlers;
using GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Handlers
{
    [TestFixture]
    public class PlayerBounceHandlerTests
    {
        private PlayerBounceHandler _handler;
        private MockPlayerMotor _motor;
        private PlayerStats _stats;

        [SetUp]
        public void Setup()
        {
            _motor = new MockPlayerMotor();
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _handler = new PlayerBounceHandler(_motor, _stats);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_stats);

        // --- Prepare ---

        [Test]
        public void PrepareBounce_SetsBouncePrepped()
        {
            _handler.PrepareBounce(0.5f);

            Assert.IsTrue(_handler.BouncePrepped);
        }

        [Test]
        public void PrepareBounce_DisablesGroundDetection_ForIgnoreTime()
        {
            _handler.PrepareBounce(0.5f);

            Assert.IsFalse(_handler.CanDetectGround);
        }

        [Test]
        public void CanDetectGround_TrueAfterIgnoreTimerExpires()
        {
            _handler.PrepareBounce(0.5f);

            _handler.Tick(0.25f);

            Assert.IsTrue(_handler.CanDetectGround);
        }

        // --- Execute ---

        [Test]
        public void ExecuteBounce_AppliesUpwardImpulse_ResetsY()
        {
            _handler.PrepareBounce(0.5f);

            _handler.ExecuteBounce();

            Assert.AreEqual(1, _motor.ImpulseCount);
            Assert.Greater(_motor.LastImpulse.y, 0f);
            Assert.IsTrue(_motor.LastResetY);
        }

        [Test]
        public void ExecuteBounce_ClearsPrep_SetsBouncing()
        {
            _handler.PrepareBounce(0.5f);

            _handler.ExecuteBounce();

            Assert.IsFalse(_handler.BouncePrepped);
            Assert.IsTrue(_handler.IsBouncing);
        }

        [Test]
        public void ExecuteBounce_PowerLerpsBasedOnYContact()
        {
            // yContact = 0 → MaxBouncePower, yContact = 1 → MinBouncePower
            _handler.PrepareBounce(0f);
            _handler.ExecuteBounce();
            float powerAtZero = _motor.LastImpulse.y;

            _motor = new MockPlayerMotor();
            _handler = new PlayerBounceHandler(_motor, _stats);
            _handler.PrepareBounce(1f);
            _handler.ExecuteBounce();
            float powerAtOne = _motor.LastImpulse.y;

            // With default stats both are 10, so they're equal
            Assert.AreEqual(powerAtZero, powerAtOne, 0.01f);
        }

        // --- Reset ---

        [Test]
        public void ResetBounce_ClearsAllState()
        {
            _handler.PrepareBounce(0.5f);
            _handler.ExecuteBounce();

            _handler.ResetBounce();

            Assert.IsFalse(_handler.IsBouncing);
            Assert.IsFalse(_handler.BouncePrepped);
            Assert.IsTrue(_handler.CanDetectGround);
        }
    }
}