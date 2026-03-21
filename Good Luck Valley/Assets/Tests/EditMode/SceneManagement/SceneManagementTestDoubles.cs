using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.SceneManagement.Enums;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Tests.EditMode.SceneManagement
{
    // --- Mock ITransitionEffect for TransitionService state machine tests ---

    public class MockTransitionEffect : ITransitionEffect
    {
        public int CoverCallCount { get; private set; }
        public int RevealCallCount { get; private set; }
        public int ResetCallCount { get; private set; }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Awaitable CoverAsync() => CoverCallCount++;
        
        public async Awaitable RevealAsync() => RevealCallCount++;
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        
        public void Reset() => ResetCallCount++;
    }

    public class MockSceneLoader : ISceneLoader
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Awaitable<SceneLoadResult> LoadSceneAsync(
            string address, 
            SceneLoadMode loadMode, 
            bool activateOnLoad = true
        )
        {
            return SceneLoadResult.Failed("MockSceneLoader: not implemented");
        }

        public async Awaitable<bool> UnloadSceneAsync(Scene scene)
        {
            return false;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}