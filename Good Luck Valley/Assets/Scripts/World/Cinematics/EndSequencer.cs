using GoodLuckValley.Architecture.ServiceLocator;
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
                // Load the main menu
                sceneLoader.ChangeSceneGroupSystem(0);
            };
        }

        public void End()
        {
            // Cut to black
            EventBus<CutToBlack>.Raise(new CutToBlack());

            // Start the post-cut timer
            postCutTimer.Start();
        }
    }
}
