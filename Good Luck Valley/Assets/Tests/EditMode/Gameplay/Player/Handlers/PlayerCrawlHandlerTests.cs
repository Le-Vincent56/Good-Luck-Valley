using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Handlers;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Handlers
{
    [TestFixture]
    public class PlayerCrawlHandlerTests
    {
        private PlayerCrawlHandler _handler;
        private PlayerStats _stats;

        [SetUp]
        public void Setup()
        {
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _handler = new PlayerCrawlHandler(_stats);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_stats);

        // --- StartCrawl ---

        [Test]
        public void StartCrawl_SetsIsCrawling()
        {
            _handler.StartCrawl(0f);

            Assert.IsTrue(_handler.IsCrawling);
        }

        // --- EndCrawl ---

        [Test]
        public void EndCrawl_ClearsState()
        {
            _handler.StartCrawl(0f);

            _handler.EndCrawl();

            Assert.IsFalse(_handler.IsCrawling);
        }

        // --- SpeedModifier ---

        [Test]
        public void SpeedModifier_StartsAtOne()
        {
            _handler.StartCrawl(0f);

            Assert.AreEqual(1f, _handler.SpeedModifier, 0.01f);
        }

        [Test]
        public void SpeedModifier_RampsToTarget_OverSlowDownTime()
        {
            _handler.StartCrawl(0f);

            // Tick to halfway through slow down time
            float halfTime = _stats.CrouchSlowDownTime / 2f;
            _handler.Tick(halfTime, halfTime);

            float expected = Mathf.Lerp(1f, _stats.CrouchSpeedModifier, 0.5f);
            Assert.AreEqual(expected, _handler.SpeedModifier, 0.01f);
        }

        [Test]
        public void SpeedModifier_ReachesMinimum_AfterSlowDownTime()
        {
            _handler.StartCrawl(0f);

            _handler.Tick(
                _stats.CrouchSlowDownTime + 0.1f, 
                _stats.CrouchSlowDownTime + 0.1f
            );

            Assert.AreEqual(_stats.CrouchSpeedModifier, _handler.SpeedModifier, 0.01f);
        }

        [Test]
        public void SpeedModifier_ResetsOnNewCrawl()
        {
            _handler.StartCrawl(0f);
            _handler.Tick(_stats.CrouchSlowDownTime, _stats.CrouchSlowDownTime);
            _handler.EndCrawl();

            _handler.StartCrawl(1f);

            Assert.AreEqual(1f, _handler.SpeedModifier, 0.01f);
        }

        // --- Ceiling State ---

        [Test]
        public void CanStand_ReflectsCeilingState()
        {
            Assert.IsTrue(_handler.CanStand);

            _handler.UpdateCeilingState(true);

            Assert.IsFalse(_handler.CanStand);

            _handler.UpdateCeilingState(false);

            Assert.IsTrue(_handler.CanStand);
        }
    }
}