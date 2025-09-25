using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Audio.Ambience;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Cinematics;
using GoodLuckValley.Scenes;
using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley
{
    public class EndSequencer : MonoBehaviour
    {
        private SceneLoader sceneLoader;

        [SerializeField] private float postCutDuration;
        private CountdownTimer postCutTimer;

        private void Start()
        {
            // Get the scene loader
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

            // Craete the post-cut timer
            postCutTimer = new CountdownTimer(postCutDuration);
            postCutTimer.OnTimerStop += () =>
            {
                // Load the end scene
                sceneLoader.ChangeSceneGroupSystem(6, false);
            };
        }

        /// <summary>
        /// End the game
        /// </summary>
        public void End()
        {
            // Cut to black
            EventBus<CutToBlack>.Raise(new CutToBlack());

            // Stop the ambience
            AmbienceManager.Instance.StopAmbience();

            // Start the post-cut timer
            postCutTimer.Start();
        }
    }
}
