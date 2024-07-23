using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.TitleScreen
{
    public class TitleBackgroundFade : MonoBehaviour
    {
        private Image image;
        private Color color;
        private float fadeTime;
        private bool active;

        private void Awake()
        {
            // Get Image component and Color
            image = GetComponent<Image>();
            color = image.color;
            active = false;

            // Make the image invisible
            Color invisible = image.color;
            invisible.a = 0f;
            image.color = invisible;
        }

        /// <summary>
        /// Set the fade time for the background fade
        /// </summary>
        /// <param name="fadeTime"></param>
        public void SetFadeTime(float fadeTime) => this.fadeTime = fadeTime;

        /// <summary>
        /// Get whether or not the background fade is active
        /// </summary>
        /// <returns>True if the background fade is active, false if not</returns>
        public bool Active() => active;

        /// <summary>
        /// Coroutine to show the UI Object
        /// </summary>
        /// <returns></returns>
        public async Task Show(float fadeTime) => await FadeInImage(image, color, fadeTime, true);

        /// <summary>
        /// Coroutine to hide the UI object
        /// </summary>
        /// <returns></returns>
        public async Task Hide(float fadeTime) => await FadeOutImage(image, color, fadeTime, true);

        /// <summary>
        /// Fade in an Image object
        /// </summary>
        /// <param name="text">The Image object to fade in</param>
        /// <param name="duration">The duration of the fade</param>
        /// <returns></returns>
        protected async Task FadeInImage(Image image, Color imageColor, float duration, bool activate)
        {
            if (!image.gameObject.activeSelf && activate) image.gameObject.SetActive(true);

            // Set the elapsed time
            float elapsedTime = 0f;

            // Get the current color
            Color color = image.color;

            // Go through the duration
            while (elapsedTime < duration)
            {
                // Increment elapsed time
                elapsedTime += Time.deltaTime;

                // Clamp the opacity to the time
                color.a = Mathf.Clamp(elapsedTime / duration, 0f, imageColor.a);

                // Set the color
                image.color = color;

                // Yield to allow other tasks to happen
                await Task.Yield();
            }

            // Set the color to be fully visible
            color.a = imageColor.a;
            image.color = color;

            active = true;
        }

        /// <summary>
        /// Fade out a Image object
        /// </summary>
        /// <param name="text">The Image object to fade out</param>
        /// <param name="duration">The duration of the fade</param>
        /// <returns></returns>
        protected async Task FadeOutImage(Image image, Color imageColor, float duration, bool deactivate)
        {
            // Set the elapsed time
            float elapsedTime = 0f;

            // Get the current color
            Color color = image.color;

            // Go through the duration
            while (elapsedTime < duration)
            {
                // Increment elapsed time
                elapsedTime += Time.deltaTime;

                // Clamp the opacity to the time
                color.a = imageColor.a - Mathf.Clamp(elapsedTime / duration, 0f, imageColor.a);

                // Set the color
                image.color = color;

                // Yield to allow other tasks to happen
                await Task.Yield();
            }

            // Set the color to be fully visible
            color.a = 0f;
            image.color = color;

            active = false;

            // De-activate the game object
            if (deactivate) image.gameObject.SetActive(false);
        }
    }
}