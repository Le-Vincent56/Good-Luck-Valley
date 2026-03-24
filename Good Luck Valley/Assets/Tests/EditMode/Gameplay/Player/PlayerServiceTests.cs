using NUnit.Framework;
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Handlers;
using GoodLuckValley.Gameplay.Player.Interfaces;
using GoodLuckValley.Gameplay.Player.Motor;
using GoodLuckValley.Gameplay.Player.Services;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player
{
    [TestFixture]
    public class PlayerServiceTests
    {
        private PlayerService _service;
        private PlayerMotor _motor;
        private PlayerJumpHandler _jumpHandler;
        private PlayerWallHandler _wallHandler;
        private PlayerBounceHandler _bounceHandler;
        private PlayerCrawlHandler _crawlHandler;
        private PlayerStats _stats;
        private CharacterSize _characterSize;

        private Mocks.MockPlayerInput _input;

        private const float DeltaTime = 0.02f;
        private const float GravityY = -9.81f;

        [SetUp]
        public void Setup()
        {
            _stats = ScriptableObject.CreateInstance<PlayerStats>();
            _characterSize = ScriptableObject.CreateInstance<CharacterSize>();
            _input = new Mocks.MockPlayerInput();
            _motor = new PlayerMotor(_stats);
            _jumpHandler = new PlayerJumpHandler(_motor, _input, _stats);
            _wallHandler = new PlayerWallHandler(_motor, _input, _stats);
            _bounceHandler = new PlayerBounceHandler(_motor, _stats);
            _crawlHandler = new PlayerCrawlHandler(_stats);

            _service = new PlayerService(
                _motor, 
                _jumpHandler, 
                _wallHandler, 
                _bounceHandler, 
                _crawlHandler,
                _input, 
                _stats, 
                _characterSize
            );
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_stats);
            Object.DestroyImmediate(_characterSize);
        }

        private CollisionData FlatGround(float distance = 0.5f)
        {
            return new CollisionData(
                groundDetected: true, 
                groundDistance: distance,
                groundNormal: Vector2.up, 
                groundAngle: 0f,
                leftWallDetected: false, 
                rightWallDetected: false,
                leftWallDistance: 0f, 
                rightWallDistance: 0f,
                ceilingBlocked: false
            );
        }

        private CollisionData NoCollision()
        {
            return new CollisionData(
                groundDetected: false, 
                groundDistance: 0f,
                groundNormal: Vector2.up, 
                groundAngle: 0f,
                leftWallDetected: false, 
                rightWallDetected: false,
                leftWallDistance: 0f, 
                rightWallDistance: 0f,
                ceilingBlocked: false
            );
        }

        // --- IsGrounded ---

        [Test]
        public void IsGrounded_True_WhenGroundDetected_WalkableAngle_NoClearance_CanDetect()
        {
            _service.FixedUpdate(DeltaTime, 0f, FlatGround(), Vector2.zero, GravityY);

            Assert.IsTrue(_service.IsGrounded);
        }

        [Test]
        public void IsGrounded_False_WhenNoGround()
        {
            _service.FixedUpdate(DeltaTime, 0f, NoCollision(), Vector2.zero, GravityY);

            Assert.IsFalse(_service.IsGrounded);
        }

        [Test]
        public void IsGrounded_False_WhenInJumpClearance()
        {
            // First tick grounded to set up
            _service.FixedUpdate(
                DeltaTime, 
                0f, 
                FlatGround(), 
                Vector2.zero, 
                GravityY
            );

            // Trigger a jump — clearance is set during FSM tick
            _input.JumpPressed = true;
            _service.FixedUpdate(
                DeltaTime, 
                DeltaTime, 
                FlatGround(), 
                Vector2.zero,
                GravityY
            );

            // Third tick — IsGrounded computed at start now sees active clearance
            _service.FixedUpdate(
                DeltaTime, 
                DeltaTime * 2f, 
                FlatGround(), 
                Vector2.zero,
                GravityY
            );

            Assert.IsFalse(_service.IsGrounded);
        }

        [Test]
        public void IsGrounded_False_WhenBounceIgnoring()
        {
            _bounceHandler.PrepareBounce(0.5f);

            _service.FixedUpdate(DeltaTime, 0f, FlatGround(), Vector2.zero, GravityY);

            Assert.IsFalse(_service.IsGrounded);
        }

        // --- ForceReceiver ---

        [Test]
        public void ForceReceiver_DelegatesToMotor()
        {
            IPlayerForceReceiver receiver = _service.ForceReceiver;

            Assert.IsNotNull(receiver);

            ForceHandle handle = receiver.AddForce(new ExternalForce(Vector2.right, 5f));
            receiver.RemoveForce(handle);
        }

        // --- AbilityControl ---

        [Test]
        public void AbilityControl_RestoreAirJump_DelegatesToJumpHandler()
        {
            // Consume the air jump first
            _jumpHandler.NotifyGrounded();
            _jumpHandler.ExecuteAirJump();
            int before = _jumpHandler.AirJumpsRemaining;

            _service.AbilityControl.RestoreAirJump();

            Assert.AreEqual(before + 1, _jumpHandler.AirJumpsRemaining);
        }

        [Test]
        public void AbilityControl_PrepareBounce_DelegatesToBounceHandler()
        {
            _service.AbilityControl.PrepareBounce(0.5f);

            Assert.IsTrue(_bounceHandler.BouncePrepped);
        }

        // --- FixedUpdate Returns MotorOutput ---

        [Test]
        public void FixedUpdate_ReturnsMotorOutput()
        {
            MotorOutput output = _service.FixedUpdate(
                DeltaTime, 
                0f, 
                FlatGround(),
                Vector2.zero, 
                GravityY
            );

            // Should not throw, output is valid
            Assert.IsNotNull(output);
        }
    }
}