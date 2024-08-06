using UnityEngine;
using GoodLuckValley.Patterns.StateMachine;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GoodLuckValley.UI.TitleScreen.Start.States
{
    public class DeleteState : IState
    {
        protected readonly DeleteOverlayController controller;
        protected readonly Animator animator;

        protected readonly List<Image> images;
        protected readonly List<Color> colors;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int DeletingHash = Animator.StringToHash("Deleting");

        protected const float crossFadeDuration = 0.1f;

        protected const float fadeTime = 0.2f;

        public DeleteState(DeleteOverlayController controller, Animator animator, GameObject backgroundObj, GameObject animatedObj)
        {
            this.controller = controller;
            this.animator = animator;

            images = new List<Image>() { backgroundObj.GetComponent<Image>(), animatedObj.GetComponent<Image>() };
            colors = new List<Color>() { images[0].color, images[1].color };
        }

        public virtual void OnEnter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void OnExit()
        {
        }

        /// <summary>
        /// Coroutine to show the UI Object
        /// </summary>
        /// <returns></returns>
        public async Task Show(float fadeTime) => await FadeInImages(images, colors, fadeTime, false);

        /// <summary>
        /// Coroutine to hide the UI object
        /// </summary>
        /// <returns></returns>
        public async Task Hide(float fadeTime) => await FadeOutImages(images, colors, fadeTime, false);

        public async Task ShowBackground(float fadeTime) => await FadeInImage(images[0], colors[0], fadeTime, false);

        public async Task HideBackground(float fadeTime) => await FadeOutImage(images[0], colors[0], fadeTime, false);

        public async Task ShowAnimation(float fadeTime) => await FadeInImage(images[1], colors[1], fadeTime, false);

        public async Task HideAnimation(float fadeTime) => await FadeOutImage(images[1], colors[1], fadeTime, false);

        /// <summary>
        /// Fade in an Image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageColor"></param>
        /// <param name="duration"></param>
        /// <param name="activate"></param>
        /// <returns></returns>
        protected async Task FadeInImage(Image image, Color imageColor, float duration, bool activate)
        {
            if (!image.gameObject.activeSelf && activate) image.gameObject.SetActive(true);

            // Set the elapsed time
            float elapsedTime = 0f;

            // Get the current color
            Color color = imageColor;

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
        }

        /// <summary>
        /// Fade out out an Image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageColor"></param>
        /// <param name="duration"></param>
        /// <param name="deactivate"></param>
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

            // De-activate the game object
            if (deactivate) image.gameObject.SetActive(false);
        }

        /// <summary>
        /// Fade in an Image object
        /// </summary>
        /// <param name="text">The Image object to fade in</param>
        /// <param name="duration">The duration of the fade</param>
        /// <returns></returns>
        protected async Task FadeInImages(List<Image> images, List<Color> colors, float duration, bool activate)
        {
            for(int i = 0; i < images.Count; i++)
            {
                if (!images[i].gameObject.activeSelf && activate) images[i].gameObject.SetActive(true);

                // Set the elapsed time
                float elapsedTime = 0f;

                // Get the current color
                Color color = images[i].color;

                // Go through the duration
                while (elapsedTime < duration)
                {
                    // Increment elapsed time
                    elapsedTime += Time.deltaTime;

                    // Clamp the opacity to the time
                    color.a = Mathf.Clamp(elapsedTime / duration, 0f, colors[i].a);

                    // Set the color
                    images[i].color = color;

                    // Yield to allow other tasks to happen
                    await Task.Yield();
                }

                // Set the color to be fully visible
                color.a = colors[i].a;
                images[i].color = color;
            }
        }

        /// <summary>
        /// Fade out a Image object
        /// </summary>
        /// <param name="text">The Image object to fade out</param>
        /// <param name="duration">The duration of the fade</param>
        /// <returns></returns>
        protected async Task FadeOutImages(List<Image> images, List<Color> colors, float duration, bool deactivate)
        {
            for(int i = 0; i < images.Count; i++)
            {
                // Set the elapsed time
                float elapsedTime = 0f;

                // Get the current color
                Color color = images[i].color;

                // Go through the duration
                while (elapsedTime < duration)
                {
                    // Increment elapsed time
                    elapsedTime += Time.deltaTime;

                    // Clamp the opacity to the time
                    color.a = colors[i].a - Mathf.Clamp(elapsedTime / duration, 0f, colors[i].a);

                    // Set the color
                    images[i].color = color;

                    // Yield to allow other tasks to happen
                    await Task.Yield();
                }

                // Set the color to be fully visible
                color.a = 0f;
                images[i].color = color;

                // De-activate the game object
                if (deactivate) images[i].gameObject.SetActive(false);
            }
        }
    }
}