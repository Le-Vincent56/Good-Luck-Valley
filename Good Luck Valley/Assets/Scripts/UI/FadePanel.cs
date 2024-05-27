using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI
{
    public class FadePanel : MonoBehaviour
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

        #region FIELDS
        private float fadeDuration = 0.2f;
        private List<ImageData> imageDatas = new List<ImageData>();
        private List<TextData> textDatas = new List<TextData>();
        #endregion

        public void Awake()
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

        public void ShowUI() => StartCoroutine(FadeIn());

        public void HideUI() => StartCoroutine(FadeOut());

        public void HideUIHard()
        {
            foreach(ImageData image in imageDatas)
            {
                image.Image.color = new Color(image.Color.r, image.Color.g, image.Color.b, 0f);
                image.Image.gameObject.SetActive(false);
            }

            foreach (TextData text in textDatas)
            {
                text.Text.color = new Color(text.Color.r, text.Color.g, text.Color.b, 0f);
                text.Text.gameObject.SetActive(false);
            }
        }

        private IEnumerator FadeIn()
        {
            List<Coroutine> fadeCoroutines = new List<Coroutine>();

            foreach (ImageData imageData in imageDatas)
            {
                fadeCoroutines.Add(StartCoroutine(FadeInElement(imageData.Image, imageData.Color, fadeDuration)));
            }

            foreach (TextData textData in textDatas)
            {
                fadeCoroutines.Add(StartCoroutine(FadeInElement(textData.Text, textData.Color, fadeDuration)));
            }

            foreach (Coroutine coroutine in fadeCoroutines)
            {
                yield return coroutine;
            }
        }

        private IEnumerator FadeInElement(Graphic uiElement, Color targetColor, float duration)
        {
            // Activate the game object if not already active
            if (!uiElement.gameObject.activeSelf) uiElement.gameObject.SetActive(true);

            // Create a timer and establish the starting color
            float elapsedTime = 0f;
            Color startColor = uiElement.color;

            // Loop while the timer is less than the duration
            while (elapsedTime < duration)
            {
                // Increase by unscaled deltaTime (pause menu freezes scaled deltaTime)
                elapsedTime += Time.unscaledDeltaTime;

                // Adjust alpha
                float alpha = Mathf.Clamp(elapsedTime / duration, 0f, targetColor.a);

                // Set alpha
                uiElement.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                yield return null;
            }

            // Ensure the final color is set to the final color
            uiElement.color = targetColor;
        }

        private IEnumerator FadeOut()
        {
            List<Coroutine> fadeCoroutines = new List<Coroutine>();

            foreach (ImageData imageData in imageDatas)
            {
                fadeCoroutines.Add(StartCoroutine(FadeOutElement(imageData.Image, imageData.Color, fadeDuration)));
            }

            foreach (TextData textData in textDatas)
            {
                fadeCoroutines.Add(StartCoroutine(FadeOutElement(textData.Text, textData.Color, fadeDuration)));
            }

            foreach (Coroutine coroutine in fadeCoroutines)
            {
                yield return coroutine;
            }
        }

        private IEnumerator FadeOutElement(Graphic uiElement, Color startColor, float duration)
        {
            // Create a timer and establish the starting color
            float elapsedTime = 0f;
            Color color = uiElement.color;

            // Loop while the timer is less than the duration
            while (elapsedTime < duration)
            {
                // Increase by unscaled deltaTime (pause menu freezes scaled deltaTime)
                elapsedTime += Time.unscaledDeltaTime;

                // Adjust alpha
                float alpha = startColor.a - Mathf.Clamp(elapsedTime / duration, 0f, startColor.a);

                // Set alpha
                uiElement.color = new Color(color.r, color.g, color.b, alpha);

                yield return null;
            }

            // Ensure the final color is set to the final color
            uiElement.color = new Color(color.r, color.g, color.b, 0f);

            // Deactivate the game object
            uiElement.gameObject.SetActive(false);
        }
    }
}