using GoodLuckValley.Patterns.StateMachine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.TitleScreen.Settings.Controls.States
{
    public class ControlsState : IState
    {
        protected struct ImageData
        {
            public Image Image;
            public Color Color;

            public ImageData(Image image)
            {
                Image = image;
                Color = image.color;
            }
        }

        protected struct TextData
        {
            public Text Text;
            public Color Color;

            public TextData(Text text)
            {
                Text = text;
                Color = text.color;
            }
        }

        protected readonly ControlsSettingController controller;
        protected readonly Animator animator;
        protected readonly GameObject panel;

        protected readonly List<ImageData> imageDatas;
        protected readonly List<TextData> textDatas;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int BindingHash = Animator.StringToHash("Rebinding");

        protected const float crossFadeDuration = 0.1f;

        protected const float fadeTime = 0.2f;

        public ControlsState(ControlsSettingController controller, GameObject panel)
        {
            imageDatas = new List<ImageData>();
            textDatas = new List<TextData>();

            this.controller = controller;
            this.panel = panel;

            animator = panel.GetComponentInChildren<Animator>();

            InstantiateUILists();
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
        /// Instantiate UI Lists
        /// </summary>
        public virtual void InstantiateUILists()
        {
            // Store images and texts into lists
            List<Image> images = panel.GetComponentsInChildren<Image>(true).ToList();
            List<Text> texts = panel.GetComponentsInChildren<Text>(true).ToList();

            // Add ImageDatas
            foreach (Image image in images)
            {
                imageDatas.Add(new ImageData(image));
            }

            // Add TextDatas
            foreach (Text text in texts)
            {
                textDatas.Add(new TextData(text));
            }
        }

        /// <summary>
        /// Make all Images and Text invisible
        /// </summary>
        protected virtual void MakeElementsInvisible()
        {
            // Loop through each Image Data
            foreach (ImageData imageData in imageDatas)
            {
                // Make the color invisible
                Color invisible = imageData.Color;
                invisible.a = 0f;
                imageData.Image.color = invisible;
            }

            // Loop through each Text Data
            foreach (TextData textData in textDatas)
            {
                // Make the color invisible
                Color invisible = textData.Color;
                invisible.a = 0f;
                textData.Text.color = invisible;
            }
        }

        /// <summary>
        /// Coroutine to show the keybind panel
        /// </summary>
        /// <returns></returns>
        public async Task Show() => await FadeIn(imageDatas, textDatas, true);

        /// <summary>
        /// Coroutine to hide the keybind panel
        /// </summary>
        /// <returns></returns>
        public async Task Hide() => await FadeOut(imageDatas, textDatas, false);

        /// <summary>
        /// Fade in Images and Texts
        /// </summary>
        /// <returns></returns>
        protected virtual async Task FadeIn(List<ImageData> images, List<TextData> texts, bool activateAll)
        {
            List<Task> imageTasks = new List<Task>();
            List<Task> textTasks = new List<Task>();

            // Check whether or not to use the default elements
            if (images == null || texts == null)
            {
                // Add each image fade-in as a task
                foreach (ImageData imageData in imageDatas)
                {
                    imageTasks.Add(FadeInImage(imageData.Image, imageData.Color, fadeTime, activateAll));
                }

                // Add each text fade-in as a task
                foreach (TextData textData in textDatas)
                {
                    textTasks.Add(FadeInText(textData.Text, textData.Color, fadeTime, activateAll));
                }
            }
            else
            {
                // Add each image fade-in as a task
                foreach (ImageData imageData in images)
                {
                    imageTasks.Add(FadeInImage(imageData.Image, imageData.Color, fadeTime, activateAll));
                }

                // Add each text fade-in as a task
                foreach (TextData textData in texts)
                {
                    textTasks.Add(FadeInText(textData.Text, textData.Color, fadeTime, activateAll));
                }
            }

            // Await until all tasks are finished
            await Task.WhenAll(imageTasks);
            await Task.WhenAll(textTasks);
        }

        /// <summary>
        /// Fade out Images and Texts
        /// </summary>
        /// <returns></returns>
        protected virtual async Task FadeOut(List<ImageData> images, List<TextData> texts, bool deactivateAll)
        {
            List<Task> imageTasks = new List<Task>();
            List<Task> textTasks = new List<Task>();

            // Check whether or not to use the default elements
            if (images == null || texts == null)
            {
                // Add each image fade-out as a task
                foreach (ImageData imageData in imageDatas)
                {
                    imageTasks.Add(FadeOutImage(imageData.Image, imageData.Color, fadeTime, deactivateAll));
                }

                // Add each text fade-out as a task
                foreach (TextData textData in textDatas)
                {
                    textTasks.Add(FadeOutText(textData.Text, textData.Color, fadeTime, deactivateAll));
                }
            }
            else
            {
                // Add each image fade-out as a task
                foreach (ImageData imageData in images)
                {
                    imageTasks.Add(FadeOutImage(imageData.Image, imageData.Color, fadeTime, deactivateAll));
                }

                // Add each text fade-out as a task
                foreach (TextData textData in texts)
                {
                    textTasks.Add(FadeOutText(textData.Text, textData.Color, fadeTime, deactivateAll));
                }
            }

            // Await until all tasks are finished
            await Task.WhenAll(imageTasks);
            await Task.WhenAll(textTasks);
        }

        /// <summary>
        /// Fade in a Text object
        /// </summary>
        /// <param name="text">The Text object to fade in</param>
        /// <param name="duration">The duration of the fade</param>
        /// <returns></returns>
        protected async Task FadeInText(Text text, Color textColor, float duration, bool activate)
        {
            if (!text.gameObject.activeSelf && activate) text.gameObject.SetActive(true);

            // Set the elapsed time
            float elapsedTime = 0f;

            // Get the current color
            Color color = text.color;

            // Go through the duration
            while (elapsedTime < duration)
            {
                // Increment elapsed time
                elapsedTime += Time.deltaTime;

                // Clamp the opacity to the time
                color.a = Mathf.Clamp(elapsedTime / duration, 0f, textColor.a);

                // Set the color
                text.color = color;

                // Yield to allow other tasks to happen
                await Task.Yield();
            }

            // Set the color to be fully visible
            color.a = textColor.a;
            text.color = color;
        }

        /// <summary>
        /// Fade out a Text object
        /// </summary>
        /// <param name="text">The Text object to fade out</param>
        /// <param name="duration">The duration of the fade</param>
        /// <returns></returns>
        protected async Task FadeOutText(Text text, Color textColor, float duration, bool deactivate)
        {
            // Set the elapsed time
            float elapsedTime = 0f;

            // Get the current color
            Color color = text.color;

            // Go through the duration
            while (elapsedTime < duration)
            {
                // Increment elapsed time
                elapsedTime += Time.deltaTime;

                // Clamp the opacity to the time
                color.a = textColor.a - Mathf.Clamp(elapsedTime / duration, 0f, textColor.a);

                // Set the color
                text.color = color;

                // Yield to allow other tasks to happen
                await Task.Yield();
            }

            // Set the color to be fully visible
            color.a = 0f;
            text.color = color;

            // De-activate the game object
            if (deactivate) text.gameObject.SetActive(false);
        }

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

            // De-activate the game object
            if (deactivate) image.gameObject.SetActive(false);
        }
    }
}