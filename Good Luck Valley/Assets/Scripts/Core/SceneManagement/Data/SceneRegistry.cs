using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Core.SceneManagement.Data
{
    /// <summary>
    /// ScriptableObject that maps scene IDs to Addressable references and DI installer type names.
    /// Central configuration for the scene management system.
    /// </summary>
    [CreateAssetMenu(fileName = "SceneRegistry", menuName = "Good Luck Valley/Scene Management/Scene Registry")]
    public class SceneRegistry : ScriptableObject
    {
        [SerializeField] private List<SceneEntry> _entries = new List<SceneEntry>();
        [SerializeField] private string transitionSceneID;
        [SerializeField] private string initialSceneID;
        
        /// <summary>
        /// All configured scene entries.
        /// </summary>
        public IReadOnlyList<SceneEntry> Entries => _entries;
        
        /// <summary>
        /// The scene ID of the persistent transition scene (loaded at app start, never unloads)
        /// </summary>
        public string TransitionSceneID => transitionSceneID;
        
        /// <summary>
        /// The scene ID of the initial scene to load after initialization (e.g., main menu).
        /// </summary>
        public string InitialSceneID => initialSceneID;

        /// <summary>
        /// Looks up a scene entry by its ID.
        /// </summary>
        /// <param name="sceneID">The scene ID to search for.</param>
        /// <returns>The matching entry, or null if not found.</returns>
        public SceneEntry GetEntry(string sceneID)
        {
            if (string.IsNullOrEmpty(sceneID)) return null;

            // TODO: Replace string-based scene ID lookup with int-based identifier
            // (e.g., hashed IDs or ScriptableObject reference equality) for performance
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].SceneID != sceneID) continue;
                
                return _entries[i];
            }

            return null;
        }

        /// <summary>
        /// Creates a SceneRegistry instance for unit testing with pre-configured entries.
        /// </summary>
        /// <param name="entries">The list of SceneEntry objects to initialize the registry with.</param>
        /// <param name="transitionSceneID">Optional ID of the transition scene. Defaults to null.</param>
        /// <param name="initialSceneID">Optional ID of the initial scene. Defaults to null.</param>
        /// <returns>A new instance of SceneRegistry configured with the provided values.</returns>
        internal static SceneRegistry CreateForTesting(
            List<SceneEntry> entries,
            string transitionSceneID = null,
            string initialSceneID = null
        )
        {
            SceneRegistry registry = CreateInstance<SceneRegistry>();
            registry._entries = new List<SceneEntry>(entries);
            registry.transitionSceneID = transitionSceneID;
            registry.initialSceneID = initialSceneID;
            return registry;
        }
    }
}