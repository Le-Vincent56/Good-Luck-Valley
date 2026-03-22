using System.Collections;
using System.Reflection;
using GoodLuckValley.Core.SceneManagement.Adapters;
using GoodLuckValley.World.LevelManagement.Effects;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace GoodLuckValley.Tests.PlayMode.SceneManagement
{
    /// <summary>
    /// Play mode tests for <see cref="ShaderTransitionEffect"/>. Verifies that
    /// the effect correctly drives <see cref="TransitionCanvasAdapter"/> progress
    /// and overlay visibility through cover/reveal cycles.
    /// </summary>
    [TestFixture]
    public class TransitionEffectTests
    {
        private static readonly int _progressProperty = Shader.PropertyToID("_Progress");

        private GameObject _canvasGo;
        private TransitionCanvasAdapter _adapter;
        private Material _testMaterial;

        [SetUp]
        public void SetUp()
        {
            // Create Canvas (inactive to prevent Awake before fields are wired)
            _canvasGo = new GameObject("TestTransitionCanvas");
            _canvasGo.SetActive(false);

            Canvas canvas = _canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Create RawImage child
            GameObject imageGo = new GameObject("Overlay");
            imageGo.transform.SetParent(_canvasGo.transform);
            RawImage rawImage = imageGo.AddComponent<RawImage>();

            // Add adapter and wire the overlay field via reflection
            _adapter = _canvasGo.AddComponent<TransitionCanvasAdapter>();
            FieldInfo overlayField = typeof(TransitionCanvasAdapter)
                .GetField("_overlay", BindingFlags.NonPublic | BindingFlags.Instance);
            overlayField?.SetValue(_adapter, rawImage);

            // Create test material
            _testMaterial = new Material(Shader.Find("Sprites/Default"));

            // Now activate — Awake fires with _overlay properly wired
            _canvasGo.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            if (_canvasGo) Object.Destroy(_canvasGo);
            if (_testMaterial) Object.Destroy(_testMaterial);
        }

        [UnityTest]
        public IEnumerator CoverAsync_SetsProgressToOne()
        {
            ShaderTransitionEffect effect = new ShaderTransitionEffect(
                _adapter, 
                _testMaterial, 
                0.05f, 
                0.05f
            );

            yield return AwaitableTestHelper.RunAwaitable(effect.CoverAsync());

            float progress = _adapter.MaterialInstance.GetFloat(_progressProperty);
            Assert.AreEqual(
                1f, 
                progress, 
                0.01f,
                "Progress should be 1 after cover completes."
            );
        }

        [UnityTest]
        public IEnumerator CoverAsync_ShowsOverlay()
        {
            ShaderTransitionEffect effect = new ShaderTransitionEffect(
                _adapter, 
                _testMaterial, 
                0.05f, 
                0.05f
            );

            yield return AwaitableTestHelper.RunAwaitable(effect.CoverAsync());

            RawImage overlay = _canvasGo.GetComponentInChildren<RawImage>();
            Assert.IsTrue(overlay.enabled, "Overlay should be visible after cover.");
        }

        [UnityTest]
        public IEnumerator RevealAsync_SetsProgressToZero()
        {
            ShaderTransitionEffect effect = new ShaderTransitionEffect(
                _adapter, 
                _testMaterial, 
                0.05f, 
                0.05f
            );

            yield return AwaitableTestHelper.RunAwaitable(effect.CoverAsync());
            yield return AwaitableTestHelper.RunAwaitable(effect.RevealAsync());

            float progress = _adapter.MaterialInstance.GetFloat(_progressProperty);
            Assert.AreEqual(
                0f, 
                progress, 
                0.01f,
                "Progress should be 0 after reveal completes."
            );
        }

        [UnityTest]
        public IEnumerator RevealAsync_HidesOverlay()
        {
            ShaderTransitionEffect effect = new ShaderTransitionEffect(
                _adapter, 
                _testMaterial, 
                0.05f, 
                0.05f
            );

            yield return AwaitableTestHelper.RunAwaitable(effect.CoverAsync());
            yield return AwaitableTestHelper.RunAwaitable(effect.RevealAsync());

            RawImage overlay = _canvasGo.GetComponentInChildren<RawImage>();
            Assert.IsFalse(overlay.enabled, "Overlay should be hidden after reveal.");
        }

        [UnityTest]
        public IEnumerator Reset_SetsProgressToZeroAndHidesOverlay()
        {
            ShaderTransitionEffect effect = new ShaderTransitionEffect(
                _adapter, 
                _testMaterial, 
                0.05f, 
                0.05f
            );

            yield return AwaitableTestHelper.RunAwaitable(effect.CoverAsync());

            effect.Reset();

            float progress = _adapter.MaterialInstance.GetFloat(_progressProperty);
            RawImage overlay = _canvasGo.GetComponentInChildren<RawImage>();

            Assert.AreEqual(0f, progress, 0.01f, "Progress should be 0 after reset.");
            Assert.IsFalse(overlay.enabled, "Overlay should be hidden after reset.");
        }

        [UnityTest]
        public IEnumerator FullCoverRevealCycle_ReturnsToInitialState()
        {
            ShaderTransitionEffect effect = new ShaderTransitionEffect(
                _adapter, 
                _testMaterial, 
                0.05f, 
                0.05f
            );

            yield return AwaitableTestHelper.RunAwaitable(effect.CoverAsync());

            float coverProgress = _adapter.MaterialInstance.GetFloat(_progressProperty);
            Assert.AreEqual(1f, coverProgress, 0.01f);

            yield return AwaitableTestHelper.RunAwaitable(effect.RevealAsync());

            float revealProgress = _adapter.MaterialInstance.GetFloat(_progressProperty);
            RawImage overlay = _canvasGo.GetComponentInChildren<RawImage>();

            Assert.AreEqual(0f, revealProgress, 0.01f);
            Assert.IsFalse(overlay.enabled);
        }
    }
}