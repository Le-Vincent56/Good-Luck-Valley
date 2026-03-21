using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.LevelManagement.Data
{
    /// <summary>
    /// ScriptableObject containing metadata for a single level. The scene ID
    /// links to a <see cref="Core.SceneManagement.Data.SceneEntry"/> in the
    /// <see cref="Core.SceneManagement.Data.SceneRegistry"/>. Spawn point positions
    /// are sourced at runtime from <see cref="Adapters.SpawnPointMarker"/> MonoBehaviours
    /// in the scene — the IDs stored here are for editor validation only.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "Good Luck Valley/Levels/Level Data")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private string sceneID;
        [SerializeField] private string displayName;
        [SerializeField] private string areaID;
        [SerializeField] private List<string> spawnPointIDs = new List<string>();
        [SerializeField] private TransitionConfig defaultEntryTransition;

        /// <summary>
        /// The scene ID matching the entry in SceneRegistry.
        /// </summary>
        public string SceneID => sceneID;

        /// <summary>
        /// Human-readable level name for UI display.
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// Identifier for the world area this level belongs to (e.g., "Forest", "Cave").
        /// Used for grouping and world-state tracking.
        /// </summary>
        public string AreaID => areaID;

        /// <summary>
        /// Known spawn point IDs in this level. For editor validation only —
        /// runtime positions come from <see cref="Adapters.SpawnPointMarker"/>.
        /// </summary>
        public IReadOnlyList<string> SpawnPointIDs => spawnPointIDs;

        /// <summary>
        /// Default transition config when entering this level. Nullable —
        /// falls back to <see cref="LevelRegistry.DefaultTransitionConfig"/>.
        /// </summary>
        public TransitionConfig DefaultEntryTransition => defaultEntryTransition;

        /// <summary>
        /// Creates a LevelData instance for unit testing.
        /// </summary>
        internal static LevelData CreateForTesting(
            string sceneID,
            string displayName = "",
            string areaID = "",
            List<string> spawnPointIDs = null,
            TransitionConfig defaultEntryTransition = null
        )
        {
            LevelData data = CreateInstance<LevelData>();
            data.sceneID = sceneID;
            data.displayName = displayName;
            data.areaID = areaID;
            data.spawnPointIDs = spawnPointIDs ?? new List<string>();
            data.defaultEntryTransition = defaultEntryTransition;
            return data;
        }
    }
}