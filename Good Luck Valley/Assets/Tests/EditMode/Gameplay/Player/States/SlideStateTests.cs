using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.States;
using GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.States
{
    [TestFixture]
    public class SlideStateTests
    {
        private SlideState _state;
        private MockPlayerContext _context;
        private MockStateMachineContext _fsm;
        private MockPlayerMotor _motor;
        private PlayerStats _stats;
        private CharacterSize _characterSize;

        [SetUp]
        public void Setup()
        {
            _motor = new MockPlayerMotor();
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _characterSize = ScriptableObject.CreateInstance<CharacterSize>();
            _fsm = new MockStateMachineContext();

            _context = new MockPlayerContext
            {
                Motor = _motor,
                Stats = _stats,
                CharacterSize = _characterSize,
                IsOnSteepSlope = true,
                Collision = new CollisionData(
                    true, 0.5f, Vector2.up, 50f,
                    false, false, 0f, 0f, false)
            };

            _state = new SlideState(_context, _fsm);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_stats);
            Object.DestroyImmediate(_characterSize);
        }

        // --- Enter ---

        [Test]
        public void Enter_SetsSlideGravity_SlideMaxSpeed()
        {
            _state.Enter();

            Assert.AreEqual(_stats.SlideGravityScale, _motor.GravityScale, 0.01f);
            Assert.AreEqual(_stats.SlidingMaxSpeed, _motor.MaxFallSpeed, 0.01f);
        }

        // --- Transitions ---

        [Test]
        public void FixedUpdate_AngleNormalized_TransitionsToGroundedState()
        {
            _state.Enter();
            _context.Collision = new CollisionData(
                true, 0.5f, Vector2.up, 30f,
                false, false, 0f, 0f, false);
            _context.IsOnSteepSlope = false;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(GroundedState), _fsm.LastTransitionTarget);
        }

        [Test]
        public void FixedUpdate_GroundLost_TransitionsToFallState()
        {
            _state.Enter();
            _context.Collision = new CollisionData(
                false, 0f, Vector2.up, 0f,
                false, false, 0f, 0f, false);
            _context.IsOnSteepSlope = false;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(FallState), _fsm.LastTransitionTarget);
        }

        [Test]
        public void FixedUpdate_StillOnSteepSlope_NoTransition()
        {
            _state.Enter();
            _context.IsOnSteepSlope = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(0, _fsm.TransitionCount);
        }
    }
}