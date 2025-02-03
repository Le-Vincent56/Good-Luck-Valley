using GoodLuckValley.Architecture.Singletons;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley
{
    public class Bootstrapper : PersistentSingleton<Bootstrapper>
    {
        private static readonly int sceneIndex = 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            //Application.targetFrameRate = 120;

#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[sceneIndex].path);
#endif
        }
    }
}
