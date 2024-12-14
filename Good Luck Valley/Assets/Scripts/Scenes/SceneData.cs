using Eflatun.SceneReference;
using System;

namespace GoodLuckValley.Scenes
{
    public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Environment, Tooling }

    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public SceneType SceneType;
        public string Name => Reference.Name;
    }
}
