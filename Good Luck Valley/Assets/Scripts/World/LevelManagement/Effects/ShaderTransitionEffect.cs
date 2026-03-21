using GoodLuckValley.Core.SceneManagement.Adapters;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using UnityEngine;

namespace GoodLuckValley.World.LevelManagement.Effects
{
    /// <summary>
    /// Concrete <see cref="ITransitionEffect"/> that drives the
    /// <see cref="TransitionCanvasAdapter"/> with a shader material.
    /// Animates the material's _Progress property from 0-to-1 (cover) and
    /// 1-to-0 (reveal) over configurable durations.
    /// </summary>
    public sealed class ShaderTransitionEffect : ITransitionEffect
    {
        private readonly TransitionCanvasAdapter _canvas;
        private readonly Material _effectMaterial;
        private readonly float _coverDuration;
        private readonly float _revealDuration;

        /// <param name="canvas">The adapter driving the transition overlay.</param>
        /// <param name="effectMaterial">The shared material to instance for this effect.</param>
        /// <param name="coverDuration">Duration in seconds for the cover animation.</param>
        /// <param name="revealDuration">Duration in seconds for the reveal animation.</param>
        public ShaderTransitionEffect(
            TransitionCanvasAdapter canvas,
            Material effectMaterial,
            float coverDuration,
            float revealDuration
        )
        {
            _canvas = canvas;
            _effectMaterial = effectMaterial;
            _coverDuration = coverDuration;
            _revealDuration = revealDuration;
        }

        /// <summary>
        /// Animates the transition from transparent to fully opaque.
        /// Sets the material, shows the overlay, and animates _Progress 0-to-1.
        /// </summary>
        public async Awaitable CoverAsync()
        {
            _canvas.SetMaterial(_effectMaterial);
            _canvas.SetProgress(0f);
            _canvas.SetOverlayActive(true);

            float elapsed = 0f;

            while (elapsed < _coverDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / _coverDuration);
                _canvas.SetProgress(progress);
                
                await Awaitable.NextFrameAsync();
            }

            _canvas.SetProgress(1f);
        }

        /// <summary>
        /// Animates the transition from fully opaque back to transparent.
        /// Animates _Progress 1-to-0 and hides the overlay when done.
        /// </summary>
        public async Awaitable RevealAsync()
        {
            float elapsed = 0f;

            while (elapsed < _revealDuration)
            {
                elapsed += Time.deltaTime;
                float progress = 1f - Mathf.Clamp01(elapsed / _revealDuration);
                _canvas.SetProgress(progress);
                
                await Awaitable.NextFrameAsync();
            }

            _canvas.SetProgress(0f);
            _canvas.SetOverlayActive(false);
        }

        /// <summary>
        /// Immediately resets to transparent and hides the overlay.
        /// </summary>
        public void Reset()
        {
            _canvas.SetProgress(0f);
            _canvas.SetOverlayActive(false);
        }
    }
}