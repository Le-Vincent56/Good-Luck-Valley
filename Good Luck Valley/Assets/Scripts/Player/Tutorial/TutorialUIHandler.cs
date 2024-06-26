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

        private void Awake()
        {
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

        public async void Show(string name)
        {
            // Get the index of the name
            int index = indexDictionary[name];

            if (index >= panels.Length)
            {
                Debug.Log("TutorialUIHandler.Show: Index is larger than the amount of indices within the Panels array");
                return;
            }

            Text[] texts = panels[index].GetComponentsInChildren<Text>();
            Image[] images = panels[index].GetComponentsInChildren<Image>();

            // Fade in every text and image
            //StartCoroutine(Fade())

            panels[index].SetActive(true);
        }

        public void Hide()
        {
            // Get the index of the name
            int index = indexDictionary[name];

            if (index >= panels.Length)
            {
                Debug.Log("TutorialUIHandler.Show: Index is larger than the amount of indices within the Panels array");
                return;
            }

            Text[] texts = panels[index].GetComponentsInChildren<Text>();
            Image[] images = panels[index].GetComponentsInChildren<Image>();

            // Fade out every text and image

            panels[index].SetActive(false);
        }

        //private IEnumerator Fade(Text[] texts, Image[] images, float targetAlpha, float duration)
        //{
        //    float elapsedTime = 0f;

        //    while(elapsedTime < duration)
        //    {

        //    }
        //}
    }
}