using System;
using GoodLuckValley.Core.SceneManagement.Adapters;
using GoodLuckValley.Core.SceneManagement.Enums;
using GoodLuckValley.Core.SceneManagement.Exceptions;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using UnityEngine;

namespace GoodLuckValley.Core.SceneManagement.Services
{
    /// <summary>
    /// Orchestrates screen transitions with a state machine, transition locking,
    /// and minimum duration support. Pure C# service registered in the application container.
    /// State transitions are exposed internally for edit-mode unit testing.
    /// </summary>
    public sealed class TransitionService : ITransitionService
    {
        private TransitionState _state;
        private TransitionCanvasAdapter _canvasAdapter;
        private ITransitionEffect _defaultEffect;
        private ITransitionEffect _activeEffect;

        /// <summary>
        /// The current state of the transition.
        /// </summary>
        public TransitionState State => _state;

        /// <summary>
        /// True if a transition is in progress (state is not Idle).
        /// </summary>
        public bool IsTransitioning => State != TransitionState.Idle;
        
        /// <summary>
        /// The canvas adapter driving transition visuals. Set during initialization.
        /// </summary>
        public TransitionCanvasAdapter CanvasAdapter => _canvasAdapter;

        /// <summary>
        /// Fired when the cover animation begins.
        /// </summary>
        public event Action OnTransitionCoverStarted;

        /// <summary>
        /// Fired when the screen is fully covered (opaque).
        /// </summary>
        public event Action OnTransitionCovered;
        
        /// <summary>
        /// Fired when the reveal animation finishes and the screen is fully visible.
        /// </summary>
        public event Action OnTransitionCompleted;

        /// <summary>
        /// Sets the canvas adapter that drives transition visuals.
        /// Called during initialization after the transition scene loads.
        /// </summary>
        /// <param name="adapter">The adapter from the persistent transition scene.</param>
        public void SetCanvasAdapter(TransitionCanvasAdapter adapter) => _canvasAdapter = adapter;
        
        /// <summary>
        /// Sets the default transition effect used when CoverAsync is called
        /// without an explicit effect.
        /// </summary>
        /// <param name="defaultEffect">The default effect to use.</param>
        public void SetDefaultEffect(ITransitionEffect defaultEffect) => _defaultEffect = defaultEffect;

        /// <summary>
        /// Covers the screen using the provided effect (or the default if null).
        /// Leaves the screen covered until <see cref="RevealAsync"/> is called.
        /// Ignored if a transition is already in progress.
        /// </summary>
        /// <param name="effect">Optional override effect. Uses default if null.</param>
        /// <param name="minimumDurationSeconds">Minimum time to hold the cover before completion.</param>
        /// <returns>An awaitable task representing the asynchronous cover operation.</returns>
        public async Awaitable CoverAsync(ITransitionEffect effect = null, float minimumDurationSeconds = 0f)
        {
            ITransitionEffect selectedEffect = effect ?? _defaultEffect;

            if (!TryBeginCover(selectedEffect)) return;

            float startTime = Time.time;
            await _activeEffect.CoverAsync();

            float elapsed = Time.time - startTime;
            
            if (elapsed < minimumDurationSeconds)
                await Awaitable.WaitForSecondsAsync(minimumDurationSeconds - elapsed);

            CompleteCover();
        }

        /// <summary>
        /// Reveals the screen after a cover. Only valid when state is covered.
        /// </summary>
        /// <returns>An awaitable task representing the asynchronous reveal operation.</returns>
        public async Awaitable RevealAsync()
        {
            if (!TryBeginReveal()) return;

            await _activeEffect.RevealAsync();

            CompleteReveal();
        }

        /// <summary>
        /// Attempts to initiate the transition cover process with the specified effect.
        /// </summary>
        /// <param name="effect">The transition effect to use for covering. Must not be null.</param>
        /// <returns>True if the cover process starts successfully; otherwise, false if a transition is already in progress.</returns>
        /// <exception cref="SceneManagementException">
        /// Thrown if the provided effect is null and no default effect has been set.
        /// </exception>
        internal bool TryBeginCover(ITransitionEffect effect)
        {
            if (IsTransitioning) return false;

            if (effect == null)
            {
                throw new SceneManagementException(
                    "No transition effect available. Set a default effect via " +
                    "SetDefaultEffect() or pass one to CoverAsync()."
                );
            }

            _activeEffect = effect;
            _state = TransitionState.Covering;
            OnTransitionCoverStarted?.Invoke();
            return true;
        }

        /// <summary>
        /// Completes the cover phase, transitioning to the Covered state.
        /// </summary>
        internal void CompleteCover()
        {
            _state = TransitionState.Covered;
            OnTransitionCovered?.Invoke();
        }

        /// <summary>
        /// Attempts to begin the reveal phase. Returns false if not in the Covered state
        /// or if no active effect is set.
        /// </summary>
        /// <returns></returns>
        internal bool TryBeginReveal()
        {
            if (_state != TransitionState.Covered) return false;

            if (_activeEffect == null) return false;

            _state = TransitionState.Revealing;
            return true;
        }

        /// <summary>
        /// Completes the reveal phase, returning to the Idle state and clearing the active effect.
        /// </summary>
        internal void CompleteReveal()
        {
            _state = TransitionState.Idle;
            _activeEffect = null;
            OnTransitionCompleted?.Invoke();
        }
    }
}