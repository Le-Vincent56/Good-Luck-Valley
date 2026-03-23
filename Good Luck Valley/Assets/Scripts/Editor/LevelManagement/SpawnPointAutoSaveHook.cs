using System.Collections.Generic;
using GoodLuckValley.World.LevelManagement.Adapters;
using GoodLuckValley.World.LevelManagement.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneUtility = GoodLuckValley.Core.Utilities.SceneUtility;

namespace GoodLuckValley.Editor.LevelManagement
{
    /// <summary>
    /// Automatically bakes spawn points when a level scene is saved.
    /// Subscribes to <see cref="EditorSceneManager.sceneSaved"/> on editor load.
    /// If a saved scene has <see cref="SpawnPointMarker"/> instances but no matching
    /// <see cref="LevelData"/>, logs a warning suggesting the designer create one.
    /// </summary>
    [InitializeOnLoad]
    public static class SpawnPointAutoSaveHook
    {
        static SpawnPointAutoSaveHook()
        {
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        private static void OnSceneSaved(Scene scene)
        {
            LevelData matchingLevel = FindLevelDataForScene(scene);

            if (matchingLevel)
            {
                SpawnPointBaker.BakeSpawnPointsFromOpenScene(matchingLevel, scene);
                return;
            }

            // Warn only if the scene has markers but no LevelData
            List<SpawnPointMarker> markers = SceneUtility.FindAllComponentsInScene<SpawnPointMarker>(scene);

            if (markers.Count > 0)
            {
                Debug.LogWarning(
                    $"[SpawnPointAutoSaveHook] Scene '{scene.name}' has {markers.Count} "
                    + "SpawnPointMarker(s) but no matching LevelData ScriptableObject. "
                    + "Consider creating one via Good Luck Valley > Level Tools > Create New Level."
                );
            }
        }

        /// <summary>
        /// Searches all LevelData assets in the project for one whose SceneID
        /// matches the given scene's name.
        /// </summary>
        private static LevelData FindLevelDataForScene(Scene scene)
        {
            string sceneName = scene.name;
            string[] guids = AssetDatabase.FindAssets("t:LevelData");

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                LevelData levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);

                if (!levelData || levelData.SceneID != sceneName) continue;
                
                return levelData;
            }

            return null;
        }
    }
}