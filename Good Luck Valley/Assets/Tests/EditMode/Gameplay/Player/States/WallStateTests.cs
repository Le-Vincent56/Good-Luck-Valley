using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.States;
using GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.States
{
    [TestFixture]
    public class WallStateTests
    {
        private WallState _state;
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
                CurrentVelocity = new Vector2(0f, -1f)
            };

            _wall.IsOnWall = true;
            _wall.WallDirection = 1;

            _state = new WallState(_context, _fsm);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_stats);
            Object.DestroyImmediate(_characterSize);
        }

        // --- Enter ---

        [Test]
        public void Enter_SetsWallGravity_WallMaxSpeed()
        {
            _state.Enter();

            Assert.AreEqual(_stats.WallSlideGravityScale, _motor.GravityScale, 0.01f);
            Assert.AreEqual(_stats.WallSlideMaxSpeed, _motor.MaxFallSpeed, 0.01f);
        }

        // --- Transitions ---

        [Test]
        public void FixedUpdate_WallLost_CoyoteExpired_TransitionsToFallState()
        {
            _state.Enter();
            _wall.IsOnWall = false;
            _wall.HasWallCoyoteTime = false;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(FallState), _fsm.LastTransitionTarget);
        }

        [Test]
        public void FixedUpdate_WallLost_CoyoteActive_DoesNotTransition()
        {
            _state.Enter();
            _wall.IsOnWall = false;
            _wall.HasWallCoyoteTime = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(0, _fsm.TransitionCount);
        }

        [Test]
        public void FixedUpdate_JumpPressed_TransitionsToJumpState_ExecutesWallJump()
        {
            _state.Enter();
            _input.JumpPressed = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(JumpState), _fsm.LastTransitionTarget);
            Assert.AreEqual(1, _wall.ExecuteWallJumpCount);
        }

        [Test]
        public void FixedUpdate_Grounded_TransitionsToGroundedState()
        {
            _state.Enter();
            _context.IsGrounded = true;

            _state.FixedUpdate(0.02f);

            Assert.AreEqual(typeof(GroundedState), _fsm.LastTransitionTarget);
        }

        // --- Exit ---

        [Test]
        public void Exit_ClearsWallVelocityOverride()
        {
            _state.Enter();
            _motor.SetWallVelocityOverride(-3f);

            _state.Exit();

            Assert.IsFalse(_motor.HasWallOverride);
        }
    }
}