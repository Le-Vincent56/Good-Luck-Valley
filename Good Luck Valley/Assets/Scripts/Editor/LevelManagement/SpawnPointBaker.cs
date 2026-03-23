using System;
using System.Collections.Generic;
using System.IO;
using GoodLuckValley.Core.Utilities;
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
    /// Static utility for baking spawn point data from scene
    /// <see cref="SpawnPointMarker"/> instances into <see cref="LevelData"/>
    /// ScriptableObjects. Called by the manual bake button in
    /// <see cref="LevelDataEditor"/> and the auto-save hook in
    /// <see cref="SpawnPointAutoSaveHook"/>.
    /// </summary>
    public static class SpawnPointBaker
    {
        /// <summary>
        /// Bakes spawn points from the scene matching the given LevelData's SceneID.
        /// Opens the scene additively if not already loaded, bakes, then closes it
        /// if it was not previously open.
        /// </summary>
        /// <param name="levelData">The LevelData to bake spawn points into.</param>
        public static void BakeSpawnPoints(LevelData levelData)
        {
            if (!levelData)
            {
                Debug.LogError("[SpawnPointBaker] LevelData is null.");
                return;
            }

            if (string.IsNullOrEmpty(levelData.SceneID))
            {
                Debug.LogError($"[SpawnPointBaker] LevelData '{levelData.name}' has no SceneID.");
                return;
            }

            string scenePath = FindSceneAssetPath(levelData.SceneID);

            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogError(
                    $"[SpawnPointBaker] Could not find scene asset for SceneID '{levelData.SceneID}'."
                );
                return;
            }

            Scene scene = SceneManager.GetSceneByPath(scenePath);
            bool wasAlreadyOpen = scene.IsValid() && scene.isLoaded;

            if (!wasAlreadyOpen)
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

            BakeSpawnPointsFromOpenScene(levelData, scene);

            if (!wasAlreadyOpen)
                EditorSceneManager.CloseScene(scene, true);
        }

        /// <summary>
        /// Bakes spawn points from an already-open scene into the given LevelData.
        /// Used by <see cref="SpawnPointAutoSaveHook"/> when the scene is already loaded.
        /// </summary>
        /// <param name="levelData">The LevelData to bake spawn points into.</param>
        /// <param name="scene">The loaded scene containing <see cref="SpawnPointMarker"/> instances.</param>
        public static void BakeSpawnPointsFromOpenScene(LevelData levelData, Scene scene)
        {
            if (!levelData || !scene.IsValid())
                return;

            List<SpawnPointMarker> markers = SceneUtility.FindAllComponentsInScene<SpawnPointMarker>(scene);
            List<BakedSpawnPoint> bakedPoints = new List<BakedSpawnPoint>();

            for (int i = 0; i < markers.Count; i++)
            {
                SpawnPointMarker marker = markers[i];

                bakedPoints.Add(new BakedSpawnPoint(
                    marker.SpawnPointID,
                    HashUtility.ComputeStableHash(marker.SpawnPointID),
                    marker.Position,
                    marker.FaceRight
                ));
            }

            levelData.SetBakedSpawnPoints(bakedPoints);
            levelData.SetLastBakeTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            EditorUtility.SetDirty(levelData);

            Debug.Log($"[SpawnPointBaker] Baked {bakedPoints.Count} spawn point(s) into '{levelData.name}'.");
        }

        /// <summary>
        /// Finds the asset path of a scene matching the given scene ID by searching
        /// the AssetDatabase for SceneAsset files whose name matches exactly.
        /// </summary>
        /// <param name="sceneID">The scene ID to search for.</param>
        /// <returns>The asset path of the matching scene, or null if not found.</returns>
        internal static string FindSceneAssetPath(string sceneID)
        {
            string[] guids = AssetDatabase.FindAssets($"t:SceneAsset {sceneID}");

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                string name = Path.GetFileNameWithoutExtension(path);

                if (name == sceneID)
                    return path;
            }

            return null;
        }
    }
}