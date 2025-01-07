using Eflatun.SceneReference;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace GoodLuckValley.Scenes
{
    public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Environment, Tooling }

    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public SceneType SceneType;

        [ShowIf("IsActiveScene")] public Vector2 Entrance;
        [ShowIf("IsActiveScene")] public Vector2 Exit;

        public string Name => Reference.Name;

        /// <summary>
        /// Get the gate of a Scene
        /// </summary>
        public Vector2 GetGate(SceneGate gateType)
        {
            return gateType switch
            {
                SceneGate.Entrance => Entrance,
                SceneGate.Exit => Exit,
                _ => Vector2.zero,
            };
        }

        // Private method used by ShowIf
        private bool IsActiveScene()
        {
            return SceneType == SceneType.ActiveScene;
        }
    }
}
