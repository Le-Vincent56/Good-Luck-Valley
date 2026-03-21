using UnityEngine;

namespace GoodLuckValley.World.LevelManagement.Data
{
    /// <summary>
    /// ScriptableObject that configures a screen transition's visual effect and timing.
    /// Assigned per-level in <see cref="LevelData"/> or per-trigger in
    /// <see cref="Adapters.LevelTransitionTrigger"/>. Falls back to
    /// <see cref="LevelRegistry.DefaultTransitionConfig"/> if neither is set.
    /// </summary>
    [CreateAssetMenu(fileName = "TransitionConfig", menuName = "Good Luck Valley/Levels/Transition Config")]
    public class TransitionConfig : ScriptableObject
    {
        [SerializeField] private TransitionEffectType effectType;
        [SerializeField] private Material effectMaterial;
        [SerializeField] private float minimumCoverDurationSeconds = 0.5f;
        [SerializeField] private float coverDurationSeconds = 0.4f;
        [SerializeField] private float revealDurationSeconds = 0.4f;

        /// <summary>
        /// The category of visual effect (fade, wipe, etc.).
        /// </summary>
        public TransitionEffectType EffectType => effectType;

        /// <summary>
        /// The shader material that drives the transition visual.
        /// Used by <see cref="Effects.ShaderTransitionEffect"/> to set on the
        /// <see cref="Core.SceneManagement.Adapters.TransitionCanvasAdapter"/>.
        /// </summary>
        public Material EffectMaterial => effectMaterial;

        /// <summary>
        /// Minimum time the screen stays fully covered before revealing.
        /// Ensures the transition doesn't feel too fast even if loading is instant.
        /// </summary>
        public float MinimumCoverDurationSeconds => minimumCoverDurationSeconds;

        /// <summary>
        /// Duration of the cover animation (screen going opaque).
        /// </summary>
        public float CoverDurationSeconds => coverDurationSeconds;

        /// <summary>
        /// Duration of the reveal animation (screen becoming transparent).
        /// </summary>
        public float RevealDurationSeconds => revealDurationSeconds;

        /// <summary>
        /// Creates a TransitionConfig instance for unit testing.
        /// </summary>
        internal static TransitionConfig CreateForTesting(
            float coverDuration = 0.4f,
            float revealDuration = 0.4f,
            float minimumCoverDuration = 0.5f
        )
        {
            TransitionConfig config = CreateInstance<TransitionConfig>();
            config.coverDurationSeconds = coverDuration;
            config.revealDurationSeconds = revealDuration;
            config.minimumCoverDurationSeconds = minimumCoverDuration;
            return config;
        }
    }
}