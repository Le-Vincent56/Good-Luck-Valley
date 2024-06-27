using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Tutorial
{
    public class TutorialUIHandler : MonoBehaviour
    {

        Dictionary<string, int> indexDictionary;
        [SerializeField] private GameObject[] panels;
        Coroutine fadeCoroutine;

        private void Awake()
        {
            // Initialize dictionary
            indexDictionary = new Dictionary<string, int>()
            {
                { "Move", 0 },
                { "Jump", 1 },
                { "Slide", 2 },
                { "Fast Fall", 3 },
                { "Crawl", 4 },
                { "Interact", 5 },
                { "Aim", 6 },
                { "Throw", 7 },
                { "Peek", 8 },
                { "Total Recall", 9 },
                { "Single Recall", 10 },
                { "Quick Bounce", 11 },
                { "Chain Bounce", 12 },
            };
        }

        public void Show(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not string) return;

            // Cast data
            string dictKey = (string)data;

            // Get the value of the key
            int index = indexDictionary[dictKey];

            // Set the game object to active
            panels[index].SetActive(true);

            // Check if the index is larger than the length of the array
            if (index >= panels.Length)
            {
                Debug.Log("TutorialUIHandler.Show: Index is larger than the amount of indices within the Panels array");
                return;
            }

            // Gather UI elements
            Text[] texts = panels[index].GetComponentsInChildren<Text>();
            Image[] images = panels[index].GetComponentsInChildren<Image>();

            // Stop any current fade coroutine
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            // Fade in every text and image
            StartCoroutine(Fade(index, texts, images, 1f, 0.4f));
        }

        public void Hide(string name)
        {
            // Get the value of the key
            int index = indexDictionary[name];

            // Check if the index is larger than the length of the array
            if (index >= panels.Length)
            {
                Debug.Log("TutorialUIHandler.Show: Index is larger than the amount of indices within the Panels array");
                return;
            }

            // Gather UI elements
            Text[] texts = panels[index].GetComponentsInChildren<Text>();
            Image[] images = panels[index].GetComponentsInChildren<Image>();

            // Stop any current fade coroutine
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            // Fade out every text and image
            StartCoroutine(Fade(index, texts, images, 0f, 0.4f));
        }

        private IEnumerator Fade(int index, Text[] texts, Image[] images, float targetAlpha, float duration)
        {
            // Get the starting alphas for each Text and Image element
            float[] startAlphasText = new float[texts.Length];
            float[] startAlphasImages = new float[images.Length];

            // Fill the Text alpha array
            for(int i = 0; i < texts.Length; i++)
            {
                if (texts[i] == null) continue;
                
                startAlphasText[i] = texts[i].color.a;
            }

            // Fill the Image alpha array
            for(int i = 0; i < images.Length; i++)
            {
                if (texts[i] == null) continue;

                startAlphasImages[i] = images[i].color.a;
            }

            // Set initial timer value
            float elapsedTime = 0f;

            // Check if the timer is less than the duration
            while (elapsedTime < duration)
            {
                // If so, increment the timer and get a t value
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                // Lerp Text alpha values
                for(int i = 0; i < texts.Length; i++)
                {
                    if (texts[i] == null) continue;

                    Color color = texts[i].color;
                    color.a = Mathf.Lerp(startAlphasText[i], targetAlpha, t);
                    texts[i].color = color;
                }
                
                // Lerp Image alpha values
                for(int i = 0; i < images.Length; i++)
                {
                    if (images[i] == null) continue;

                    Color color = texts[i].color;
                    color.a = Mathf.Lerp(startAlphasImages[i], targetAlpha, t);
                    texts[i].color = color;
                }

                yield return null;
            }

            // Ensure the final Text alphas are set
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] != null)
                {
                    Color color = texts[i].color;
                    color.a = targetAlpha;
                    texts[i].color = color;
                }
            }
            
            // Ensure the final Image alphas are set
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] != null)
                {
                    Color color = images[i].color;
                    color.a = targetAlpha;
                    images[i].color = color;
                }
            }

            // Check if the target alpha is 0
            if(targetAlpha == 0f)
            {
                // If so, deactivate the game object
                panels[index].SetActive(false);
            }
        }
    }
}