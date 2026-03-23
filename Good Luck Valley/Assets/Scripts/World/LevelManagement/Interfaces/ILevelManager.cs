using System;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.World.LevelManagement.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.World.LevelManagement.Interfaces
{
    /// <summary>
    /// Game-specific level orchestration. Manages level loading, transitions,
    /// and spawn point resolution. Sits in the World layer, consuming
    /// Foundation's ISceneService and ITransitionService.
    /// </summary>
    public interface ILevelManager
    {
        /// <summary>
        /// The currently loaded level's data, or null if no level is loaded.
        /// </summary>
        LevelData CurrentLevel { get; }

        /// <summary>
        /// The currently loaded level's scene, or default if no level is loaded.
        /// </summary>
        Scene CurrentLevelScene { get; }

        /// <summary>
        /// Fired when a level transition begins.
        /// </summary>
        event Action<string> OnLevelTransitionStarted;

        /// <summary>
        /// Fired when a level finishes loading and the player is positioned.
        /// </summary>
        event Action<LevelData> OnLevelLoaded;

        /// <summary>
        /// Loads the starting level from LevelRegistry. Called during gameplay
        /// session initialization. The screen should already be covered.
        /// </summary>
        /// <param name="gameplaySessionContainer">
        /// The gameplay session container that level containers are scoped under.
        /// </param>
        Awaitable LoadFirstLevelAsync(IContainer gameplaySessionContainer);

        /// <summary>
        /// Transitions from the current level to a new one. Covers the screen,
        /// unloads the current level, loads the target, finds the spawn point,
        /// and reveals. Ignored if a transition is already in progress.
        /// </summary>
        /// <param name="targetLevel">The LevelData of the target level.</param>
        /// <param name="targetSpawnPointID">Spawn point ID to position the player at.</param>
        /// <param name="transitionConfig">
        /// Optional override. Falls back to the target level's default, then to
        /// LevelRegistry's default.
        /// </param>
        Awaitable TransitionToLevelAsync(
            LevelData targetLevel,
            string targetSpawnPointID,
            TransitionConfig transitionConfig = null
        );

        /// <summary>
        /// Unloads the current level scene and disposes its container.
        /// Does not perform a screen transition.
        /// </summary>
        Awaitable UnloadCurrentLevelAsync();
    }
}