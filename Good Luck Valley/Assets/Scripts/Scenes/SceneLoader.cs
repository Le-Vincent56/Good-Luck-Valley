using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private int sceneIndexToLoad;
        [SerializeField] private Image loadingImage;
        [SerializeField] private SceneGroup[] sceneGroups;

        private bool isLoading;

        public readonly SceneGroupManager manager = new SceneGroupManager();

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private async void Start()
        {
            await LoadSceneGroup(sceneIndexToLoad);
        }

        private void OnEnable()
        {
            manager.OnSceneGroupLoaded += () => HandleLoading(false);
        }

        private void OnDisable()
        {
            manager.OnSceneGroupLoaded -= () => HandleLoading(false);
        }

        /// <summary>
        /// Load a Scene Group
        /// </summary>
        public async Task LoadSceneGroup(int index)
        {
            // Exit case - the index is not valid
            if (index < 0 || index >= sceneGroups.Length) return;

            LoadingProgress progress = new LoadingProgress();

            // Start loading
            HandleLoading(true);

            // Load scenes
            await manager.LoadScenes(sceneGroups[index], progress);
        }

        /// <summary>
        /// Handle loading for the Scene Loader
        /// </summary>
        /// <param name="isLoading"></param>
        private void HandleLoading(bool isLoading)
        {
            // Set isLoading
            this.isLoading = isLoading;

            // Check if loading
            if (isLoading)
            {
                // Fade in the loading image
                Fade(1f, fadeDuration);
            }
            else
            {
                // Fade out the loading image
                Fade(0f, fadeDuration);
            }
        }

        /// <summary>
        /// Handle the fading for loading
        /// </summary>
        private void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = loadingImage.DOFade(endValue, duration);

            // Exit case - there is no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete = onComplete;
        }
    }
}
