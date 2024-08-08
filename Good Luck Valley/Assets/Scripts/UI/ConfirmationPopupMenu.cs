using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GoodLuckValley.UI
{
    public class ConfirmationPopupMenu : MonoBehaviour
    {
        public struct ImageData
        {
            public Image Image;
            public Color Color;

            public ImageData(Image image)
            {
                Image = image;
                Color = image.color;
            }
        }

        public struct TextData
        {
            public Text Text;
            public Color Color;

            public TextData(Text text)
            {
                Text = text;
                Color = text.color;
            }
        }

        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event playButtonEnter;
        [SerializeField] private AK.Wwise.Event playButtonExit;

        [SerializeField] private Text displayText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private float fadeDuration;
        protected List<ImageData> imageDatas = new List<ImageData>();
        protected List<TextData> textDatas = new List<TextData>();

        /// <summary>
        /// Show the popup menu
        /// </summary>
        /// <param name="displaytext">The display text</param>
        /// <param name="confirmAction">The code that will run on confirmation</param>
        /// <param name="cancelAction">The code that will run on cancellation</param>
        public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
        {
            // Set the display text
            this.displayText.text = displayText;

            // Remove any existening listeners just to make sure there aren't any previous ones hanging around
            // This only removes listeners added through code
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            // Assign the onClick listeners
            confirmButton.onClick.AddListener(() =>
            {
                DeactivateMenu(true);
                confirmAction();
            });

            cancelButton.onClick.AddListener(() =>
            {
                DeactivateMenu(false);
                cancelAction();
            });
        }

        public async void DeactivateMenu(bool enter)
        {
            if(enter)
                // Play the button enter sound
                playButtonEnter.Post(gameObject);
            else
                // Play the button exit sound
                playButtonExit.Post(gameObject);

            await Hide();
        }

        /// <summary>
        /// Instantiate UI Lists
        /// </summary>
        public virtual void InstantiateUILists()
        {
            // Store images and texts into lists
            List<Image> images = GetComponentsInChildren<Image>(true).ToList();
            List<Text> texts = GetComponentsInChildren<Text>(true).ToList();

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
        public virtual void MakeElementsInvisible()
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
        /// Coroutine to show the UI Object
        /// </summary>
        /// <returns></returns>
        public virtual async Task Show(List<ImageData> images = null, List<TextData> texts = null, bool activateAll = true) { await FadeIn(images, texts, activateAll); }

        /// <summary>
        /// Coroutine to hide the UI object
        /// </summary>
        /// <returns></returns>
        public virtual async Task Hide(List<ImageData> images = null, List<TextData> texts = null, bool deactivateAll = true) { await FadeOut(images, texts, deactivateAll); }

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
                    imageTasks.Add(FadeInImage(imageData.Image, imageData.Color, fadeDuration, activateAll));
                }

                // Add each text fade-in as a task
                foreach (TextData textData in textDatas)
                {
                    textTasks.Add(FadeInText(textData.Text, textData.Color, fadeDuration, activateAll));
                }
            }
            else
            {
                // Add each image fade-in as a task
                foreach (ImageData imageData in images)
                {
                    imageTasks.Add(FadeInImage(imageData.Image, imageData.Color, fadeDuration, activateAll));
                }

                // Add each text fade-in as a task
                foreach (TextData textData in texts)
                {
                    textTasks.Add(FadeInText(textData.Text, textData.Color, fadeDuration, activateAll));
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
                    imageTasks.Add(FadeOutImage(imageData.Image, imageData.Color, fadeDuration, deactivateAll));
                }

                // Add each text fade-out as a task
                foreach (TextData textData in textDatas)
                {
                    textTasks.Add(FadeOutText(textData.Text, textData.Color, fadeDuration, deactivateAll));
                }
            }
            else
            {
                // Add each image fade-out as a task
                foreach (ImageData imageData in images)
                {
                    imageTasks.Add(FadeOutImage(imageData.Image, imageData.Color, fadeDuration, deactivateAll));
                }

                // Add each text fade-out as a task
                foreach (TextData textData in texts)
                {
                    textTasks.Add(FadeOutText(textData.Text, textData.Color, fadeDuration, deactivateAll));
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