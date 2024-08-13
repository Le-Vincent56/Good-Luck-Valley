using GoodLuckValley.Events;
using GoodLuckValley.SceneManagement;
using System.Collections;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class CameraFade : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onTransitionBeginLate;

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Fields")]
        [SerializeField] private float fadeDuration;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            // Get components
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Black out the fade camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void Blackout(Component sender, object data)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        }

        /// <summary>
        /// Fade in the camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void PlayFadeIn(Component sender, object data)
        {
            // Stop and nullify any current fade coroutine
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            // Start the fade in coroutine
            fadeCoroutine = StartCoroutine(FadeIn());
        }

        /// <summary>
        /// Fade out the camera
        /// </summary>
        /// <param name="sdender"></param>
        /// <param name="data"></param>
        public void PlayFadeOut(Component sender, object data)
        {
            // Stop and nullify any current fade coroutine
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
            
            // Start the fade out coroutine
            fadeCoroutine = StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Coroutine for fading out
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOut()
        {
            // Set the initial progress data
            float startAlpha = 0f;
            float rate = 1f / fadeDuration;
            float progress = 0f;
            Color color = spriteRenderer.color;

            // Continue fading in the background
            while (progress < 1.0f)
            {
                // Lerp the alpha value
                color.a = Mathf.Lerp(startAlpha, 1f, progress);
                spriteRenderer.color = color;

                // Increase the progress
                progress += rate * Time.unscaledDeltaTime;

                yield return null;
            }

            // Set the final color
            color.a = 1f;
            spriteRenderer.color = color;

            // Trigger any late transition begin effects (what should happen AFTER the
            // sceen turns black)
            // Calls to:
            //  - PlayerSFXMaster.StopConstantEvents();
            onTransitionBeginLate.Raise(this, null);

            // Change the scene
            SceneLoader.Instance.ChangeScene();
        }
        

        /// <summary>
        /// Coroutine for fading in
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeIn()
        {
            // Set initial progress data
            float startAlpha = 1f;
            float rate = 1f / fadeDuration;
            float progress = 0f;
            Color color = spriteRenderer.color;

            // Continue fading in the background
            while (progress < 1.0f)
            {
                // Lerp the alpha value
                color.a = Mathf.Lerp(startAlpha, 0f, progress);
                spriteRenderer.color = color;

                // Increase the progress
                progress += rate * Time.unscaledDeltaTime;

                yield return null;
            }

            // Set the final color
            color.a = 0f;
            spriteRenderer.color = color;

            // Trigger any late transition begin effects (what should happen AFTER the
            // sceen turns black)
            // Calls to:
            //  - PlayerSFXMaster.StopConstantEvents();
            onTransitionBeginLate.Raise(this, null);

            // End the transition
            SceneLoader.Instance.EndTransition();
        }
    }
}