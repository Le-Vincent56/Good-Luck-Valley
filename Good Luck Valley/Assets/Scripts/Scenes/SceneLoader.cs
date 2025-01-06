using DG.Tweening;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Utilities.EventBus;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image loadingImage;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeOutDuration;
        [SerializeField] private float fadeInDuration;
        private Tween fadeTween;

        [Header("Scenes")]
        [SerializeField] private SceneGroupData sceneGroupData;
        private bool isLoading;
        public readonly SceneGroupManager manager = new SceneGroupManager();

        public SceneGroup[] SceneGroups { get => sceneGroupData.SceneGroups; }

        private void Awake()
        {
            // Register this as a Service
            ServiceLocator.Global.Register(this);
        }

        private async void Start()
        {
            await LoadSceneGroup(sceneGroupData.InitialScene);
        }

        private void OnEnable()
        {
            manager.OnSceneGroupLoaded += () => { HandleLoading(false); /*EventBusUtils.Debug();*/  };
        }

        private void OnDisable()
        {
            manager.OnSceneGroupLoaded -= () => HandleLoading(false);
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween
            fadeTween?.Kill();
        }

        /// <summary>
        /// Load a Scene Group
        /// </summary>
        public async Task LoadSceneGroup(int index)
        {
            // Exit case - the index is not valid
            if (index < 0 || index >= sceneGroupData.SceneGroups.Length) return;

            LoadingProgress progress = new LoadingProgress();

            // Start loading
            HandleLoading(true);

            await manager.LoadScenes(sceneGroupData.SceneGroups[index], progress);
        }

        /// <summary>
        /// Change from one Scene Group to another
        /// </summary>
        public void ChangeSceneGroup(int index, int lastMovingDirect)
        {
            // Exit case - the index is not valid
            if (index < 0 || index >= sceneGroupData.SceneGroups.Length) return;

            LoadingProgress progress = new LoadingProgress();

            // Start loading
            HandleLoading(true);

            // Fade in the loading image
            Fade(1f, fadeOutDuration, Ease.InQuad, async () => await manager.LoadScenes(sceneGroupData.SceneGroups[index], progress, true));
        }

        /// <summary>
        /// Handle loading for the Scene Loader
        /// </summary>
        private void HandleLoading(bool isLoading)
        {
            // Set isLoading
            this.isLoading = isLoading;

            // Check if loading
            if (!isLoading)
            {
                Fade(0f, fadeInDuration, Ease.OutQuad);
            }
        }

        /// <summary>
        /// Handle the fading for loading
        /// </summary>
        private void Fade(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = loadingImage.DOFade(endValue, duration);
            fadeTween.SetEase(easeType);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete = onComplete;
        }
    }
}
