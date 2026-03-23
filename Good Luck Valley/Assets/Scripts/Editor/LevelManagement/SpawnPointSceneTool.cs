using System.Collections.Generic;
using GoodLuckValley.World.LevelManagement.Adapters;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneUtility = GoodLuckValley.Core.Utilities.SceneUtility;

namespace GoodLuckValley.Editor.LevelManagement
{
    /// <summary>
    /// Editor tool for adding <see cref="SpawnPointMarker"/> GameObjects to level
    /// scenes. Provides a main menu item and a Hierarchy context menu entry.
    /// Auto-generates a unique spawn point ID based on existing markers in the scene.
    /// </summary>
    public static class SpawnPointSceneTool
    {
        private const string SpawnPointPrefix = "SpawnPoint_";

        /// <summary>
        /// Creates a new SpawnPointMarker GameObject in the active scene with an
        /// auto-generated unique ID and selects it for immediate positioning.
        /// </summary>
        [MenuItem("Good Luck Valley/Level Tools/Add Spawn Point")]
        [MenuItem("GameObject/Good Luck Valley/Add Spawn Point", false, 10)]
        private static void AddSpawnPoint()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            List<SpawnPointMarker> existingMarkers = SceneUtility.FindAllComponentsInScene<SpawnPointMarker>(activeScene);

            string generatedID = GenerateUniqueID(existingMarkers);

            GameObject spawnPointObject = new GameObject(generatedID);
            SceneManager.MoveGameObjectToScene(spawnPointObject, activeScene);

            SpawnPointMarker marker = spawnPointObject.AddComponent<SpawnPointMarker>();

            // Set the spawn point ID via SerializedObject for proper dirty/undo tracking
            SerializedObject serializedMarker = new SerializedObject(marker);
            serializedMarker.FindProperty("spawnPointID").stringValue = generatedID;
            serializedMarker.FindProperty("faceRight").boolValue = true;
            serializedMarker.ApplyModifiedPropertiesWithoutUndo();

            Undo.RegisterCreatedObjectUndo(spawnPointObject, "Add Spawn Point");
            Selection.activeGameObject = spawnPointObject;

            EditorSceneManager.MarkSceneDirty(activeScene);
        }

        /// <summary>
        /// Generates a unique ID for a new spawn point, ensuring no duplicate IDs exist
        /// among the provided list of existing spawn point markers.
        /// </summary>
        /// <param name="existingMarkers">The list of spawn point markers currently in the scene,
        /// used to check for existing ID collisions.</param>
        /// <returns>A unique string identifier for the new spawn point.</returns>
        private static string GenerateUniqueID(List<SpawnPointMarker> existingMarkers)
        {
            HashSet<string> existingIDs = new HashSet<string>();

            for (int i = 0; i < existingMarkers.Count; i++)
            {
                existingIDs.Add(existingMarkers[i].SpawnPointID);
            }

            int nextIndex = existingMarkers.Count + 1;
            string candidateID = $"{SpawnPointPrefix}{nextIndex:D2}";

            while (existingIDs.Contains(candidateID))
            {
                nextIndex++;
                candidateID = $"{SpawnPointPrefix}{nextIndex:D2}";
            }

            return candidateID;
        }
    }
}