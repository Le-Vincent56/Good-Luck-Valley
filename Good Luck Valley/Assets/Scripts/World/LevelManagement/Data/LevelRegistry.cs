using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.LevelManagement.Data
{
    /// <summary>
    /// ScriptableObject containing the ordered list of all levels and the
    /// starting level configuration. Central reference for level flow.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelRegistry", menuName = "Good Luck Valley/Levels/Level Registry")]
    public class LevelRegistry : ScriptableObject
    {
        [SerializeField] private List<LevelData> allLevels = new List<LevelData>();
        [SerializeField] private LevelData startingLevel;
        [SerializeField] private string startingSpawnPointID;
        [SerializeField] private TransitionConfig defaultTransitionConfig;
        private Dictionary<int, LevelData> _lookupByStableID;
        
        /// <summary>
        /// All levels in the game, in order.
        /// </summary>
        public IReadOnlyList<LevelData> AllLevels => allLevels;

        /// <summary>
        /// The first level loaded when starting a new game.
        /// </summary>
        public LevelData StartingLevel => startingLevel;

        /// <summary>
        /// The spawn point ID to use when loading the starting level.
        /// </summary>
        public string StartingSpawnPointID => startingSpawnPointID;

        /// <summary>
        /// System-wide default transition config. Used when neither the trigger
        /// nor the target level specifies a transition config.
        /// </summary>
        public TransitionConfig DefaultTransitionConfig => defaultTransitionConfig;

        /// <summary>
        /// Looks up a level by its scene ID.
        /// </summary>
        /// <param name="sceneID">The scene ID to search for.</param>
        /// <returns>The matching LevelData, or null if not found.</returns>
        public LevelData GetLevelBySceneID(string sceneID)
        {
            if (string.IsNullOrEmpty(sceneID)) return null;
            
            for (int i = 0; i < allLevels.Count; i++)
            {
                if (allLevels[i].SceneID == sceneID)
                {
                    return allLevels[i];
                }
            }

            return null;
        }
        
        /// <summary>
        /// Looks up a level by its stable ID. Uses an O(1) dictionary lookup
        /// built lazily on first access.
        /// </summary>
        /// <param name="stableID">The stable ID to search for.</param>
        /// <returns>The matching LevelData, or null if not found.</returns>
        public LevelData GetLevelByStableID(int stableID)
        {
            EnsureLookupBuilt();

            return _lookupByStableID.GetValueOrDefault(stableID);
        }

        /// <summary>
        /// Ensures that a lookup dictionary mapping stable IDs to levels is built and initialized.
        /// This method populates the dictionary with valid LevelData objects from the list of levels
        /// using their StableID as the key, only if the dictionary has not been created yet.
        /// </summary>
        private void EnsureLookupBuilt()
        {
            if (_lookupByStableID != null) return;

            _lookupByStableID = new Dictionary<int, LevelData>();

            for (int i = 0; i < allLevels.Count; i++)
            {
                LevelData level = allLevels[i];

                if (!level || level.StableID == 0) continue;
                
                _lookupByStableID[level.StableID] = level;
            }
        }

        /// <summary>
        /// Creates a LevelRegistry instance for unit testing.
        /// </summary>
        internal static LevelRegistry CreateForTesting(
            List<LevelData> allLevels,
            LevelData startingLevel = null,
            string startingSpawnPointId = null,
            TransitionConfig defaultTransitionConfig = null
        )
        {
            LevelRegistry registry = CreateInstance<LevelRegistry>();
            registry.allLevels = new List<LevelData>(allLevels);
            registry.startingLevel = startingLevel;
            registry.startingSpawnPointID = startingSpawnPointId;
            registry.defaultTransitionConfig = defaultTransitionConfig;
            return registry;
        }
    }
}