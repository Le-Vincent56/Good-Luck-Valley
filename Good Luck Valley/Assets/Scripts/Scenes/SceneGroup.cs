using System;
using System.Collections.Generic;
using System.Linq;

namespace GoodLuckValley.Scenes
{
    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "New Scene Group";
        public List<SceneData> Scenes;

        /// <summary>
        /// Get the name of the first Scene matching the given SceneType
        /// </summary>
        public string FindSceneNameByType(SceneType sceneType)
        {
            return Scenes.FirstOrDefault(scene => scene.SceneType == sceneType)?.Reference.Name;
        }
    }
}
