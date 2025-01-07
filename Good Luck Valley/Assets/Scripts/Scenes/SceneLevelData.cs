using Eflatun.SceneReference;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GoodLuckValley.Scenes
{
    [Serializable]
    public class LevelData
    {
        public Vector2 Entrance;
        public Vector2 Exit;
    }

    [CreateAssetMenu(fileName = "Scene Level Data", menuName = "Data/Scene Level Data")]
    public class SceneLevelData : SerializedScriptableObject
    {
        [SerializeField] private List<SceneData> sceneDatas;

        ///// <summary>
        ///// Get the Entrance of a Scene Group Active Scene
        ///// </summary>
        //public Vector2 GetEntrance(SceneReference sceneRef) => LevelGates[sceneRef].Entrance;

        ///// <summary>
        ///// Get the Exit of a Scene Group Active Scene
        ///// </summary>
        //public Vector2 GetExit(SceneReference sceneRef) => LevelGates[sceneRef].Exit;
    }
}
