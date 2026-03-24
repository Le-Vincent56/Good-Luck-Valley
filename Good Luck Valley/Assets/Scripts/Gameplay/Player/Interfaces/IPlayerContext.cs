using UnityEngine;
using GoodLuckValley.Core.Input.Interfaces;
using GoodLuckValley.Core.StateMachine.Interfaces;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.States;

namespace GoodLuckValley.Gameplay.Player.Interfaces
{
    /// <summary>
    /// Defines the context for a player, providing access to core player
    /// components, player-specific data, and the current state of the player.
    /// </summary>
    public interface IPlayerContext
    {
        IPlayerMotor Motor { get; }
        IPlayerInput Input { get; }
        IJumpHandler Jump { get; }
        IWallHandler Wall { get; }
        IBounceHandler Bounce { get; }
        ICrawlHandler Crawl { get; }
        PlayerStats Stats { get; }
        CharacterSize CharacterSize { get; }
        CollisionData Collision { get; }
        bool IsGrounded { get; }
        bool IsOnSteepSlope { get; }
        int DetectedWallDirection { get; }
        Vector2 CurrentVelocity { get; }
        ColliderMode CurrentColliderMode { get; }
        IStateMachineContext<IPlayerState> StateMachine { get; }
        
        void SetColliderMode(ColliderMode mode);
    }
}