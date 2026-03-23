using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.LevelManagement.Data
{
    /// <summary>
    /// ScriptableObject containing metadata for a single level. The scene ID
    /// links to a <see cref="Core.SceneManagement.Data.SceneEntry"/> in the
    /// <see cref="Core.SceneManagement.Data.SceneRegistry"/>. Contains baked spawn
    /// point data snapshotted from scene <see cref="Adapters.SpawnPointMarker"/>
    /// instances at edit time. Runtime positioning still uses live markers as
    /// the source of truth.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "Good Luck Valley/Levels/Level Data")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private string sceneID;
        [SerializeField] private string displayName;
        [SerializeField] private string areaID;
        [SerializeField] private int stableID;
        [SerializeField] private List<BakedSpawnPoint> bakedSpawnPoints = new List<BakedSpawnPoint>();
        [SerializeField] private TransitionConfig defaultEntryTransition;
        [SerializeField] private long lastBakeTimestamp;
        
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
        /// Deterministic integer hash of <see cref="SceneID"/>, computed via
        /// <see cref="HashUtility.ComputeStableHash"/> at edit time.
        /// </summary>
        public int StableID => stableID;

        /// <summary>
        /// Spawn points baked from <see cref="Adapters.SpawnPointMarker"/> instances
        /// in the scene at edit time. Provides ID, position, and facing data without
        /// requiring the scene to be loaded. Runtime positioning still uses the
        /// live <see cref="Adapters.SpawnPointMarker"/> as the source of truth.
        /// </summary>
        public IReadOnlyList<BakedSpawnPoint> BakedSpawnPoints => bakedSpawnPoints;

        /// <summary>
        /// Default transition config when entering this level. Nullable —
        /// falls back to <see cref="LevelRegistry.DefaultTransitionConfig"/>.
        /// </summary>
        public TransitionConfig DefaultEntryTransition => defaultEntryTransition;

        /// <summary>
        /// UTC Unix timestamp in milliseconds of the last spawn point bake.
        /// Used by editor tooling to detect stale baked data.
        /// </summary>
        public long LastBakeTimestamp => lastBakeTimestamp;
        
        /// <summary>
        /// Updates the stable ID for the level. This ID is used for identifying levels in a persistent and consistent manner.
        /// </summary>
        /// <param name="id">The new stable ID value to assign to the level.</param>
        internal void SetStableID(int id) => stableID = id;
        
        /// <summary>
        /// Sets the last bake timestamp. Called by <see cref="Editor.LevelManagement.SpawnPointBaker"/>
        /// after baking spawn points.
        /// </summary>
        internal void SetLastBakeTimestamp(long timestamp)
        {
            lastBakeTimestamp = timestamp;
        }

        /// <summary>
        /// Sets the baked spawn points for the level. These are predefined points used
        /// for determining where entities can spawn within the level during runtime.
        /// </summary>
        /// <param name="points">A list of baked spawn points to assign. If null, it will reset to an empty list.</param>
        internal void SetBakedSpawnPoints(List<BakedSpawnPoint> points)
        {
            bakedSpawnPoints = points ?? new List<BakedSpawnPoint>();
        }

        /// <summary>
        /// Creates a LevelData instance for unit testing.
        /// </summary>
        internal static LevelData CreateForTesting(
            string sceneID,
            string displayName = "",
            string areaID = "",
            List<BakedSpawnPoint> bakedSpawnPoints = null,
            TransitionConfig defaultEntryTransition = null,
            int stableID = 0,
            long lastBakeTimestamp = 0
        )
        {
            LevelData data = CreateInstance<LevelData>();
            data.sceneID = sceneID;
            data.displayName = displayName;
            data.areaID = areaID;
            data.stableID = stableID;
            data.bakedSpawnPoints = bakedSpawnPoints ?? new List<BakedSpawnPoint>();
            data.defaultEntryTransition = defaultEntryTransition;
            data.lastBakeTimestamp = lastBakeTimestamp;
            return data;
        }
    }
}