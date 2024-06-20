using GoodLuckValley.Events;
using GoodLuckValley.SceneManagement;
using System.Collections;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class CameraFade : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Fields")]
        [SerializeField] private float fadeDuration;
        [SerializeField] private float currentAlpha;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            // Get components
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Set the current alpha
            currentAlpha = spriteRenderer.color.a;
        }

        /// <summary>
        /// Fade in the camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void PlayFadeIn(Component sender, object data)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            // If there is boolean data sent and it is true, set the color opacity to full
            if (data is bool newScene && newScene)
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

            fadeCoroutine = StartCoroutine(Fade(0f, true));
        }

        /// <summary>
        /// Fade out the camera
        /// </summary>
        /// <param name="sdender"></param>
        /// <param name="data"></param>
        public void PlayFadeOut(Component sender, object data)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(Fade(1f, false));
        }

        /// <summary>
        /// Coroutine to handle fading
        /// </summary>
        /// <param name="targetAlpha">The alpha  value (0-1) to fade towards</param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha, bool fadeIn)
        {
            float startAlpha = spriteRenderer.color.a;
            float elapsedTime = 0f;
            Color color = spriteRenderer.color;

            // Calculate the adjusted fade duration based on the current alpha
            float adjustedFadeDuration = Mathf.Abs(targetAlpha - startAlpha) * fadeDuration;

            while(elapsedTime < adjustedFadeDuration)
            {
                // Increment elapsed time
                elapsedTime += Time.deltaTime;

                // Get time t
                float t = elapsedTime / adjustedFadeDuration;

                // Fade sprite color
                color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                spriteRenderer.color = color;

                // Run async
                yield return null;
            }

            // Ensure final values are set
            color.a = targetAlpha;
            spriteRenderer.color = color;

            // Update current alpha
            currentAlpha = targetAlpha;

            // Nullify the fade coroutine
            fadeCoroutine = null;

            // Change the scene if fading out
            if (!fadeIn)
                SceneLoader.Instance.ChangeScene();
            else
                // End the transition if fading in
                SceneLoader.Instance.EndTransition();
        }
    }
}