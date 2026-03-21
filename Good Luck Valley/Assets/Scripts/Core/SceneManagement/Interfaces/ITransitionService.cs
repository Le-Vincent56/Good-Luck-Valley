using System;
using GoodLuckValley.Core.SceneManagement.Adapters;
using GoodLuckValley.Core.SceneManagement.Enums;
using UnityEngine;

namespace GoodLuckValley.Core.SceneManagement.Interfaces
{
    /// <summary>
    /// Orchestrates screen transitions. Manages transition state, enforces
    /// transition locking (ignores requests while transitioning), and supports
    /// a default effect with per-call overrides.
    /// </summary>
    public interface ITransitionService
    {
        /// <summary>
        /// The current state of the transition.
        /// </summary>
        TransitionState State { get; }
        
        /// <summary>
        /// True if a transition is in progress (state is not Idle).
        /// </summary>
        bool IsTransitioning { get; }
        
        /// <summary>
        /// The canvas adapter driving transition visuals. Set during initialization.
        /// </summary>
        TransitionCanvasAdapter CanvasAdapter { get; }

        /// <summary>
        /// Fired when the cover animation begins.
        /// </summary>
        event Action OnTransitionCoverStarted;

        /// <summary>
        /// Fired when the screen is fully covered (opaque).
        /// </summary>
        event Action OnTransitionCovered;

        /// <summary>
        /// Fired when the reveal animation finishes and the screen is fully visible.
        /// </summary>
        event Action OnTransitionCompleted;

        /// <summary>
        /// Covers the screen using the provided effect (or the default if null).
        /// Leaves the screen covered until <see cref="RevealAsync"/> is called.
        /// Ignored if a transition is already in progress.
        /// </summary>
        /// <param name="effect">Optional override effect. Uses default if null.</param>
        /// <param name="minimumDurationSeconds">Minimum time to hold the cover before completion.</param>
        /// <returns>An awaitable task representing the asynchronous cover operation.</returns>
        Awaitable CoverAsync(ITransitionEffect effect = null, float minimumDurationSeconds = 0f);

        /// <summary>
        /// Reveals the screen after a cover. Only valid when state is covered.
        /// </summary>
        /// <returns>An awaitable task representing the asynchronous reveal operation.</returns>
        Awaitable RevealAsync();

        /// <summary>
        /// Sets the canvas adapter that drives transition visuals.
        /// Called during initialization after the transition scene loads.
        /// </summary>
        /// <param name="adapter">The adapter from the persistent transition scene.</param>
        void SetCanvasAdapter(TransitionCanvasAdapter adapter);

        /// <summary>
        /// Sets the default transition effect used when CoverAsync is called
        /// without an explicit effect.
        /// </summary>
        /// <param name="defaultEffect">The default effect to use.</param>
        void SetDefaultEffect(ITransitionEffect defaultEffect);
    }
}