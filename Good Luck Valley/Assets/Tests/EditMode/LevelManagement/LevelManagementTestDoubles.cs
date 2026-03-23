using System;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.SceneManagement.Adapters;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.SceneManagement.Enums;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Tests.EditMode.LevelManagement
{
    // --- Mock ISceneService for LevelManager tests ---

    public class MockSceneService : ISceneService
    {
        public SceneLoadState LoadState => SceneLoadState.Idle;
        public bool IsBusy => false;

        public event Action<string> OnSceneLoadStarted;
        public event Action<string, Scene> OnSceneLoadCompleted;
        public event Action<string, string> OnSceneLoadFailed;
        public event Action<Scene> OnSceneUnloaded;

#pragma warning disable CS1998
        public async Awaitable InitializeAsync()
        {
        }

        public async Awaitable<SceneLoadResult> LoadSceneAsync(
            string sceneID,
            IContainer parentContainer = null
        )
        {
            return SceneLoadResult.Failed("MockSceneService: not implemented");
        }

        public async Awaitable<bool> UnloadSceneAsync(Scene scene) => false;

        public async Awaitable<SceneLoadResult> LoadAdditiveSceneAsync(
            string sceneID,
            IContainer parentContainer
        )
        {
            return SceneLoadResult.Failed("MockSceneService: not implemented");
        }

        public async Awaitable<SceneLoadResult> LoadSceneAsync(
            int stableID,
            IContainer parentContainer = null
        )
        {
            return SceneLoadResult.Failed("MockSceneService: not implemented");
        }

        public async Awaitable<SceneLoadResult> LoadAdditiveSceneAsync(
            int stableID,
            IContainer parentContainer
        )
        {
            return SceneLoadResult.Failed("MockSceneService: not implemented");
        }
#pragma warning restore CS1998

        /// <summary>
        /// Suppress unused event warnings — events exist to satisfy the interface.
        /// </summary>
        internal void SuppressWarnings()
        {
            OnSceneLoadStarted?.Invoke(null);
            OnSceneLoadCompleted?.Invoke(null, default);
            OnSceneLoadFailed?.Invoke(null, null);
            OnSceneUnloaded?.Invoke(default);
        }
    }

    // --- Mock ITransitionService for LevelManager tests ---

    public class MockTransitionService : ITransitionService
    {
        public TransitionState State => TransitionState.Idle;
        public bool IsTransitioning => false;
        public TransitionCanvasAdapter CanvasAdapter => null;

        public event Action OnTransitionCoverStarted;
        public event Action OnTransitionCovered;
        public event Action OnTransitionCompleted;

#pragma warning disable CS1998
        public async Awaitable CoverAsync(
            ITransitionEffect effect = null,
            float minimumDurationSeconds = 0f
        )
        {
        }

        public async Awaitable RevealAsync()
        {
        }
#pragma warning restore CS1998

        public void SetCanvasAdapter(TransitionCanvasAdapter adapter)
        {
        }

        public void SetDefaultEffect(ITransitionEffect defaultEffect)
        {
        }

        /// <summary>
        /// Suppress unused event warnings — events exist to satisfy the interface.
        /// </summary>
        internal void SuppressWarnings()
        {
            OnTransitionCoverStarted?.Invoke();
            OnTransitionCovered?.Invoke();
            OnTransitionCompleted?.Invoke();
        }
    }
}