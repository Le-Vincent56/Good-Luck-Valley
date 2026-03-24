using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Motor;
using GoodLuckValley.Gameplay.Player.States;
using GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.States
{
    [TestFixture]
    public class FallStateTests
    {
        private FallState _state;
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
                CurrentVelocity = new Vector2(0f, -5f)
            };

            _state = new FallState(_context, _fsm);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_stats);
            Object.DestroyImmediate(_characterSize);
        }

        // --- Enter ---

        [Test]
        public void Enter_SetsFallGravity_MaxFallSpeed()
        {
            _state.Enter();

            Assert.AreEqual(_stats.FallGravityScale, _motor.GravityScale, 0.01f);
            Assert.AreEqual(_stats.FallingMaxSpeed, _motor.MaxFallSpeed, 0.01f);
            Assert.AreEqual(MovementMode.Airborne, _motor.CurrentMovementMode);
        }

        // --- Transitions ---

        [Test]
        public void FixedUpdate_Grounded_TransitionsToGroundedState()
        {
            _state.Enter();
            _context.IsGrounded = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(GroundedState), _fsm.LastTransitionTarget);
        }

        [Test]
        public void FixedUpdate_SteepSlope_TransitionsToSlideState()
        {
            _state.Enter();
            _context.IsOnSteepSlope = true;
            _context.IsGrounded = false;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(SlideState), _fsm.LastTransitionTarget);
        }

        [Test]
        public void FixedUpdate_CoyoteJump_TransitionsToJumpState()
        {
            _state.Enter();
            _input.JumpPressed = true;
            _jump.HasCoyoteTime = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(JumpState), _fsm.LastTransitionTarget);
            Assert.AreEqual(1, _jump.ExecuteNormalJumpCount);
        }

        [Test]
        public void FixedUpdate_AirJump_TransitionsToJumpState()
        {
            _state.Enter();
            _input.JumpPressed = true;
            _jump.HasCoyoteTime = false;
            _jump.HasAirJump = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(JumpState), _fsm.LastTransitionTarget);
            Assert.AreEqual(1, _jump.ExecuteAirJumpCount);
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

        // --- Fast Fall ---

        [Test]
        public void FixedUpdate_DownHeld_AppliesFastFallGravity()
        {
            _state.Enter();
            _input.Move = new Vector2(0f, -1f);

            _state.FixedUpdate(0.02f);

            float expectedGravity = _stats.FallGravityScale * _stats.FastFallMultiplier;
            Assert.AreEqual(expectedGravity, _motor.GravityScale, 0.01f);
        }

        [Test]
        public void FixedUpdate_NoDownInput_UsesNormalFallGravity()
        {
            _state.Enter();
            _input.Move = Vector2.zero;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(_stats.FallGravityScale, _motor.GravityScale, 0.01f);
        }
    }
}