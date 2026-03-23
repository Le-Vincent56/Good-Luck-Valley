using System;
using System.Collections.Generic;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.SceneManagement.Adapters;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.SceneManagement.Exceptions;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using GoodLuckValley.Core.Utilities;
using GoodLuckValley.World.LevelManagement.Adapters;
using GoodLuckValley.World.LevelManagement.Data;
using GoodLuckValley.World.LevelManagement.Effects;
using GoodLuckValley.World.LevelManagement.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.World.LevelManagement.Services
{
    /// <summary>
    /// Game-specific level orchestration. Manages the load/unload/transition
    /// cycle for gameplay levels, including spawn point resolution and
    /// transition effect creation from <see cref="TransitionConfig"/>.
    /// Pure C# service — registered in the Gameplay Session Container
    /// via constructor injection.
    /// </summary>
    public sealed class LevelManager : ILevelManager
    {
        private readonly ISceneService _sceneService;
        private readonly ITransitionService _transitionService;
        private readonly LevelRegistry _levelRegistry;

        private IContainer _gameplaySessionContainer;
        private LevelData _currentLevel;
        private Scene _currentLevelScene;

        /// <summary>
        /// The currently loaded level's data, or null if no level is loaded.
        /// </summary>
        public LevelData CurrentLevel => _currentLevel;

        /// <summary>
        /// The currently loaded level's scene, or default if no level is loaded.
        /// </summary>
        public Scene CurrentLevelScene => _currentLevelScene;

        /// <summary>
        /// Fired when a level transition begins.
        /// </summary>
        public event Action<string> OnLevelTransitionStarted;

        /// <summary>
        /// Fired when a level finishes loading and the player is positioned.
        /// </summary>
        public event Action<LevelData> OnLevelLoaded;

        public LevelManager(
            ISceneService sceneService,
            ITransitionService transitionService,
            LevelRegistry levelRegistry
        )
        {
            _sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
            _transitionService = transitionService ?? throw new ArgumentNullException(nameof(transitionService));
            _levelRegistry = levelRegistry ?? throw new ArgumentNullException(nameof(levelRegistry));
        }

        /// <summary>
        /// Loads the starting level from LevelRegistry. Called during gameplay
        /// session initialization. The screen should already be covered.
        /// </summary>
        /// <param name="gameplaySessionContainer">
        /// The gameplay session container that level containers are scoped under.
        /// </param>
        public async Awaitable LoadFirstLevelAsync(IContainer gameplaySessionContainer)
        {
            _gameplaySessionContainer = gameplaySessionContainer ??
                                        throw new ArgumentNullException(nameof(gameplaySessionContainer));

            LevelData startingLevel = _levelRegistry.StartingLevel;

            if (!startingLevel)
                throw new SceneManagementException("No starting level configured in LevelRegistry.");

            SceneLoadResult result = await _sceneService.LoadAdditiveSceneAsync(
                startingLevel.StableID,
                _gameplaySessionContainer
            );

            if (!result.Success)
                throw new SceneManagementException(
                    $"Failed to load starting level '{startingLevel.SceneID}'': {result.ErrorMessage}");

            _currentLevel = startingLevel;
            _currentLevelScene = result.Scene;

            // TODO: Find SpawnPointMarker by _levelRegistry.StartingSpawnPointId
            // and position player. Requires Gameplay layer integration.

            OnLevelLoaded?.Invoke(_currentLevel);
        }

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
        public async Awaitable TransitionToLevelAsync(
            LevelData targetLevel,
            string targetSpawnPointID,
            TransitionConfig transitionConfig = null)
        {
            if (_transitionService.IsTransitioning)
                return;

            if (!targetLevel)
                throw new SceneManagementException("Target level is null.");

            OnLevelTransitionStarted?.Invoke(targetLevel.SceneID);

            // Resolve transition config: per-call override -> level default -> registry default
            TransitionConfig config = transitionConfig
                                      ?? targetLevel.DefaultEntryTransition
                                      ?? _levelRegistry.DefaultTransitionConfig;

            // Create transition effect from config
            ITransitionEffect effect = CreateTransitionEffect(config);

            float minimumDuration = config
                ? config.MinimumCoverDurationSeconds
                : 0f;

            // Cover the screen
            await _transitionService.CoverAsync(effect, minimumDuration);

            // Unload the current level
            if (_currentLevelScene.IsValid())
                await _sceneService.UnloadSceneAsync(_currentLevelScene);

            // Load the new level via stable ID
            SceneLoadResult result = await _sceneService.LoadAdditiveSceneAsync(
                targetLevel.StableID, _gameplaySessionContainer);

            if (!result.Success)
            {
                _currentLevel = null;
                _currentLevelScene = default;

                await _transitionService.RevealAsync();

                throw new SceneManagementException(
                    $"Failed to load level '{targetLevel.SceneID}': {result.ErrorMessage}");
            }

            _currentLevel = targetLevel;
            _currentLevelScene = result.Scene;

            // TODO: Find SpawnPointMarker by targetSpawnPointID and position player.
            // Requires Gameplay layer integration (IPlayerStateCapture).

            // Reveal the screen
            await _transitionService.RevealAsync();

            OnLevelLoaded?.Invoke(_currentLevel);
        }

        /// <summary>
        /// Unloads the current level scene and disposes its container.
        /// Does not perform a screen transition.
        /// </summary>
        public async Awaitable UnloadCurrentLevelAsync()
        {
            if (_currentLevelScene.IsValid())
                await _sceneService.UnloadSceneAsync(_currentLevelScene);

            _currentLevel = null;
            _currentLevelScene = default;
        }

        /// <summary>
        /// Creates a <see cref="ShaderTransitionEffect"/> from the given config.
        /// Returns null if no config or no material is available.
        /// </summary>
        private ITransitionEffect CreateTransitionEffect(TransitionConfig config)
        {
            if (!config || !config.EffectMaterial)
                return null;

            TransitionCanvasAdapter canvasAdapter = _transitionService.CanvasAdapter;

            if (!canvasAdapter)
                return null;

            return new ShaderTransitionEffect(
                canvasAdapter,
                config.EffectMaterial,
                config.CoverDurationSeconds,
                config.RevealDurationSeconds
            );
        }
    }
}