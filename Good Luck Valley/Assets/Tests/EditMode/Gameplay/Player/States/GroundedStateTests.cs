using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Motor;
using GoodLuckValley.Gameplay.Player.States;
using GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.States
{
    [TestFixture]
    public class GroundedStateTests
    {
        private GroundedState _state;
        private MockPlayerContext _context;
        private MockStateMachineContext _fsm;
        private MockPlayerMotor _motor;
        private MockPlayerInput _input;
        private MockJumpHandler _jump;
        private MockBounceHandler _bounce;
        private PlayerStats _stats;
        private CharacterSize _characterSize;

        [SetUp]
        public void Setup()
        {
            _motor = new MockPlayerMotor();
            _input = new MockPlayerInput();
            _jump = new MockJumpHandler();
            _bounce = new MockBounceHandler();
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _characterSize = ScriptableObject.CreateInstance<CharacterSize>();
            _fsm = new MockStateMachineContext();

            _context = new MockPlayerContext
            {
                Motor = _motor,
                Input = _input,
                Jump = _jump,
                Bounce = _bounce,
                Stats = _stats,
                CharacterSize = _characterSize,
                IsGrounded = true,
                Collision = new CollisionData(
                    true, 0.5f, Vector2.up, 0f,
                    false, false, 0f, 0f, false)
            };

            _state = new GroundedState(_context, _fsm);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_stats);
            Object.DestroyImmediate(_characterSize);
        }

        // --- Enter ---

        [Test]
        public void Enter_SetsGravityZero_GroundedMode()
        {
            _state.Enter();

            Assert.AreEqual(0f, _motor.GravityScale, 0.01f);
            Assert.AreEqual(MovementMode.Grounded, _motor.CurrentMovementMode);
        }

        // --- Transitions ---

        [Test]
        public void FixedUpdate_JumpInput_TransitionsToJumpState()
        {
            _state.Enter();
            _input.JumpPressed = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(JumpState), _fsm.LastTransitionTarget);
            Assert.AreEqual(1, _jump.ExecuteNormalJumpCount);
        }

        [Test]
        public void FixedUpdate_GroundLost_TransitionsToFallState()
        {
            _state.Enter();
            _context.IsGrounded = false;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(FallState), _fsm.LastTransitionTarget);
            Assert.AreEqual(1, _jump.NotifyLeftGroundCount);
        }

        [Test]
        public void FixedUpdate_BouncePrepped_TransitionsToBounceState()
        {
            _bounce.BouncePrepped = true;
            _state.Enter();

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(BounceState), _fsm.LastTransitionTarget);
        }

        [Test]
        public void FixedUpdate_SteepSlope_TransitionsToSlideState()
        {
            _state.Enter();
            _context.IsGrounded = false;
            _context.IsOnSteepSlope = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(SlideState), _fsm.LastTransitionTarget);
        }

        [Test]
        public void FixedUpdate_NoTransition_WhenGroundedNoInput()
        {
            _state.Enter();
            _context.IsGrounded = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(0, _fsm.TransitionCount);
        }
    }
}