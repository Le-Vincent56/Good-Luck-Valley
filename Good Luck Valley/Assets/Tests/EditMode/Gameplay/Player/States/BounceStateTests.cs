using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Motor;
using GoodLuckValley.Gameplay.Player.States;
using GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.States
{
    [TestFixture]
    public class BounceStateTests
    {
        private BounceState _state;
        private MockPlayerContext _context;
        private MockStateMachineContext _fsm;
        private MockPlayerMotor _motor;
        private MockPlayerInput _input;
        private MockBounceHandler _bounce;
        private MockWallHandler _wall;
        private PlayerStats _stats;
        private CharacterSize _characterSize;

        [SetUp]
        public void Setup()
        {
            _motor = new MockPlayerMotor();
            _input = new MockPlayerInput();
            _bounce = new MockBounceHandler();
            _wall = new MockWallHandler();
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _characterSize = ScriptableObject.CreateInstance<CharacterSize>();
            _fsm = new MockStateMachineContext();

            _context = new MockPlayerContext
            {
                Motor = _motor,
                Input = _input,
                Bounce = _bounce,
                Wall = _wall,
                Stats = _stats,
                CharacterSize = _characterSize,
                CurrentVelocity = new Vector2(0f, 10f)
            };

            _state = new BounceState(_context, _fsm);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_stats);
            Object.DestroyImmediate(_characterSize);
        }

        // --- Enter ---

        [Test]
        public void Enter_SetsBounceGravity_ExecutesBounce()
        {
            _state.Enter();

            Assert.AreEqual(_stats.BounceGravityScale, _motor.GravityScale, 0.01f);
            Assert.AreEqual(MovementMode.Airborne, _motor.CurrentMovementMode);
            Assert.AreEqual(1, _bounce.ExecuteBounceCount);
        }

        // --- Transitions ---

        [Test]
        public void FixedUpdate_VelocityNegative_TransitionsToFallState()
        {
            _state.Enter();
            _context.CurrentVelocity = new Vector2(0f, -1f);

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(FallState), _fsm.LastTransitionTarget);
        }

        [Test]
        public void FixedUpdate_WallDetected_TransitionsToWallState()
        {
            _state.Enter();
            _context.DetectedWallDirection = 1;
            _wall.ShouldAttachResult = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(WallState), _fsm.LastTransitionTarget);
        }

        [Test]
        public void FixedUpdate_StillAscending_NoTransition()
        {
            _state.Enter();
            _context.CurrentVelocity = new Vector2(0f, 5f);

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(0, _fsm.TransitionCount);
        }
    }
}