using DG.Tweening;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Input;
using GoodLuckValley.Scenes;
using UnityEngine;

namespace GoodLuckValley.World.Cinematics
{
    public class CallToAction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Fields")]
        [SerializeField] private bool active;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private void Awake()
        {
            // Get the Scene Loader
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

            // Set active to false
            active = false;
        }

        private void OnEnable()
        {
            inputReader.Enable();
            inputReader.ContinueToMain += ContinueToMain;
        }

        private void OnDisable()
        {
            inputReader.ContinueToMain -= ContinueToMain;
            inputReader.Disable();
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
        }

        /// <summary>
        /// Continue to the Main Menu
        /// </summary>
        private void ContinueToMain(bool started)
        {
            // Exit case - not active
            if (!active) return;

            // Exit case - the button was pressed down
            if (started) return;

            // Change the scene to the main menu
            sceneLoader.ChangeSceneGroupSystem(0);

            // Disable the input reader
            inputReader.Disable();
        }

        /// <summary>
        /// Allow the Player to return to the Main Menu by activating
        /// </summary>
        public void Activate()
        {
            // Set to active
            active = true;

            // Fade in the input canvas
            Fade(1f, fadeDuration);
        }

        /// <summary>
        /// Fade the input Canvas Group
        /// </summary>
        private void Fade(float endValue, float duration)
        {
            // Exit case - kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Fade the canvas group
            fadeTween = canvasGroup.DOFade(endValue, duration);
        }
    }
}
