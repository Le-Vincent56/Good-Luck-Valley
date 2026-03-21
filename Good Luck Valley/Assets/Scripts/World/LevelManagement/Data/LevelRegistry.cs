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
        [SerializeField] private List<LevelData> _allLevels = new List<LevelData>();
        [SerializeField] private LevelData _startingLevel;
        [SerializeField] private string _startingSpawnPointID;
        [SerializeField] private TransitionConfig _defaultTransitionConfig;

        /// <summary>
        /// All levels in the game, in order.
        /// </summary>
        public IReadOnlyList<LevelData> AllLevels => _allLevels;

        /// <summary>
        /// The first level loaded when starting a new game.
        /// </summary>
        public LevelData StartingLevel => _startingLevel;

        /// <summary>
        /// The spawn point ID to use when loading the starting level.
        /// </summary>
        public string StartingSpawnPointID => _startingSpawnPointID;

        /// <summary>
        /// System-wide default transition config. Used when neither the trigger
        /// nor the target level specifies a transition config.
        /// </summary>
        public TransitionConfig DefaultTransitionConfig => _defaultTransitionConfig;

        /// <summary>
        /// Looks up a level by its scene ID.
        /// </summary>
        /// <param name="sceneID">The scene ID to search for.</param>
        /// <returns>The matching LevelData, or null if not found.</returns>
        public LevelData GetLevelBySceneID(string sceneID)
        {
            if (string.IsNullOrEmpty(sceneID)) return null;

            // TODO: Replace string-based scene ID lookup with int-based identifier
            // (e.g., hashed IDs or ScriptableObject reference equality) for performance
            for (int i = 0; i < _allLevels.Count; i++)
            {
                if (_allLevels[i].SceneID == sceneID)
                {
                    return _allLevels[i];
                }
            }

            return null;
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
            registry._allLevels = new List<LevelData>(allLevels);
            registry._startingLevel = startingLevel;
            registry._startingSpawnPointID = startingSpawnPointId;
            registry._defaultTransitionConfig = defaultTransitionConfig;
            return registry;
        }
    }
}