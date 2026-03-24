using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Motor;
using GoodLuckValley.Gameplay.Player.States;
using GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.States
{
    [TestFixture]
    public class JumpStateTests
    {
        private JumpState _state;
        private MockPlayerContext _context;
        private MockStateMachineContext _fsm;
        private MockPlayerMotor _motor;
        private MockPlayerInput _input;
        private MockJumpHandler _jump;
        private MockBounceHandler _bounce;
        private MockWallHandler _wall;
        private PlayerStats _stats;
        private CharacterSize _characterSize;

        [SetUp]
        public void Setup()
        {
            _motor = new MockPlayerMotor();
            _input = new MockPlayerInput();
            _jump = new MockJumpHandler();
            _bounce = new MockBounceHandler();
            _wall = new MockWallHandler();
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _characterSize = ScriptableObject.CreateInstance<CharacterSize>();
            _fsm = new MockStateMachineContext();

            _context = new MockPlayerContext
            {
                Motor = _motor,
                Input = _input,
                Jump = _jump,
                Bounce = _bounce,
                Wall = _wall,
                Stats = _stats,
                CharacterSize = _characterSize,
                CurrentVelocity = new Vector2(0f, 10f)
            };

            _state = new JumpState(_context, _fsm);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_stats);
            Object.DestroyImmediate(_characterSize);
        }

        // --- Enter ---

        [Test]
        public void Enter_SetsJumpGravity_AirborneMode()
        {
            _state.Enter();

            Assert.AreEqual(_stats.JumpGravityScale, _motor.GravityScale, 0.01f);
            Assert.AreEqual(MovementMode.Airborne, _motor.CurrentMovementMode);
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
        public void FixedUpdate_BouncePrepped_TransitionsToBounceState()
        {
            _state.Enter();
            _bounce.BouncePrepped = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(BounceState), _fsm.LastTransitionTarget);
        }

        // --- End Jump Early ---

        [Test]
        public void FixedUpdate_JumpReleased_SetsEndJumpEarly()
        {
            _state.Enter();
            _input.JumpHeld = false;
            _jump.EndedJumpEarly = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(
                _stats.EndJumpEarlyExtraForceMultiplier,
                _motor.EndJumpEarlyMultiplier, 
                0.01f
            );
        }

        [Test]
        public void FixedUpdate_JumpStillHeld_NoEndJumpEarly()
        {
            _state.Enter();
            _input.JumpHeld = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(0f, _motor.EndJumpEarlyMultiplier, 0.01f);
        }

        // --- Exit ---

        [Test]
        public void Exit_ClearsEndJumpEarlyMultiplier()
        {
            _state.Enter();
            _motor.SetEndJumpEarlyMultiplier(3f);

            _state.Exit();

            Assert.AreEqual(0f, _motor.EndJumpEarlyMultiplier, 0.01f);
        }
    }
}