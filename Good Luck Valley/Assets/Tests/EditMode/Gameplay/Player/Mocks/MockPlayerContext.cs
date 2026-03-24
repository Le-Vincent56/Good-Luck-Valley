using UnityEngine;
using GoodLuckValley.Core.Input.Interfaces;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;
using GoodLuckValley.Gameplay.Player.States;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks
{
    public class MockPlayerContext : IPlayerContext
    {
        public IPlayerMotor Motor { get; set; }
        public IPlayerInput Input { get; set; }
        public IJumpHandler Jump { get; set; }
        public IWallHandler Wall { get; set; }
        public IBounceHandler Bounce { get; set; }
        public ICrawlHandler Crawl { get; set; }
        public PlayerStats Stats { get; set; }
        public CharacterSize CharacterSize { get; set; }
        public CollisionData Collision { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsOnSteepSlope { get; set; }
        public int DetectedWallDirection { get; set; }
        public Vector2 CurrentVelocity { get; set; }
        public ColliderMode CurrentColliderMode { get; private set; }
        public IStateMachineContext<IPlayerState> StateMachine { get; set; }

        public void SetColliderMode(ColliderMode mode) => CurrentColliderMode = mode;
    }
}