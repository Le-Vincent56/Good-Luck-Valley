using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.TitleScreen.States
{
    public class MenuState : IState
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

        protected TitleScreenController menu;
        protected StateMachine stateMachine;
        protected GameObject uiObject;
        protected MenuCursor cursor;
        protected List<ImageData> imageDatas = new List<ImageData>();
        protected List<TextData> textDatas = new List<TextData>();
        protected List<Graphic> exclusionList = new List<Graphic>();

        public bool FadeInOut { get; private set; }
        public float FadeDuration { get; private set; }
        public GameObject UIObject { get { return uiObject; } set { uiObject = value; } }

        public MenuState(TitleScreenController menu, StateMachine stateMachine, bool fadeInOut, Exclusions exclusions, GameObject uiObject, MenuCursor cursor)
        {
            this.menu = menu;
            this.stateMachine = stateMachine;
            this.uiObject = uiObject;
            this.cursor = cursor;
            FadeInOut = fadeInOut;
            FadeDuration = 0.3f;

            AddExcludeds(exclusions.Objects);

            InstantiateUILists();
        }

        /// <summary>
        /// Enter a Menu State
        /// </summary>
        public virtual async void OnEnter()
        {
            await Show();
        }

        /// <summary>
        /// Exit a Menu State
        /// </summary>
        public virtual async void OnExit()
        {
            await Hide();
        }

        /// <summary>
        /// Update Menu State logic
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Update the Menu State logic at a fixed rate
        /// </summary>
        public virtual void FixedUpdate() { }

        /// <summary>
        /// Add a list of excluded objects to the excluded list
        /// </summary>
        /// <param name="listOfExcluded">The list of objects to exclude</param>
        public void AddExcludeds(List<Graphic> listOfExcluded)
        {
            // Add all of the objects to the list of excluded objects
            if(listOfExcluded != null)
                exclusionList.AddRange(listOfExcluded);
        }

        /// <summary>
        /// Coroutine to show the UI Object
        /// </summary>
        /// <returns></returns>
        protected virtual async Task Show(List<ImageData> images = null, List<TextData> texts = null, bool activateAll = true) { if (FadeInOut) await FadeIn(images, texts, activateAll); }

        /// <summary>
        /// Coroutine to hide the UI object
        /// </summary>
        /// <returns></returns>
        protected virtual async Task Hide(List<ImageData> images = null, List<TextData> texts = null, bool deactivateAll = true) { if (FadeInOut) await FadeOut(images, texts, deactivateAll); }

        /// <summary>
        /// Instantiate UI Lists
        /// </summary>
        public virtual void InstantiateUILists()
        {
            // Store images and texts into lists
            List<Image> images = uiObject.GetComponentsInChildren<Image>(true).ToList();
            List<Text> texts = uiObject.GetComponentsInChildren<Text>(true).ToList();

            // Add ImageDatas
            foreach (Image image in images)
            {
                if(!exclusionList.Contains(image))
                    imageDatas.Add(new ImageData(image));
            }

            // Add TextDatas
            foreach (Text text in texts)
            {
                if(!exclusionList.Contains(text))
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
                    imageTasks.Add(FadeInImage(imageData.Image, imageData.Color, FadeDuration, activateAll));
                }

                // Add each text fade-in as a task
                foreach (TextData textData in textDatas)
                {
                    textTasks.Add(FadeInText(textData.Text, textData.Color, FadeDuration, activateAll));
                }
            }
            else
            {
                // Add each image fade-in as a task
                foreach (ImageData imageData in images)
                {
                    imageTasks.Add(FadeInImage(imageData.Image, imageData.Color, FadeDuration, activateAll));
                }

                // Add each text fade-in as a task
                foreach (TextData textData in texts)
                {
                    textTasks.Add(FadeInText(textData.Text, textData.Color, FadeDuration, activateAll));
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
                    imageTasks.Add(FadeOutImage(imageData.Image, imageData.Color, FadeDuration, deactivateAll));
                }

                // Add each text fade-out as a task
                foreach (TextData textData in textDatas)
                {
                    textTasks.Add(FadeOutText(textData.Text, textData.Color, FadeDuration, deactivateAll));
                }
            }
            else
            {
                // Add each image fade-out as a task
                foreach (ImageData imageData in images)
                {
                    imageTasks.Add(FadeOutImage(imageData.Image, imageData.Color, FadeDuration, deactivateAll));
                }

                // Add each text fade-out as a task
                foreach (TextData textData in texts)
                {
                    textTasks.Add(FadeOutText(textData.Text, textData.Color, FadeDuration, deactivateAll));
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
