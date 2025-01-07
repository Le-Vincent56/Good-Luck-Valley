using System.Linq;
using UnityEngine;

namespace GoodLuckValley.Scenes
{
    [CreateAssetMenu(fileName = "Scene Group Data", menuName = "Data/Scene Group")]
    public class SceneGroupData : ScriptableObject
    {
        public SceneGroup[] SceneGroups;
        [HideInInspector] public int InitialScene;

        public SceneData GetActiveScene(int index) => SceneGroups[index].Scenes.FirstOrDefault(sceneData => sceneData.SceneType == SceneType.ActiveScene);
    }
}
