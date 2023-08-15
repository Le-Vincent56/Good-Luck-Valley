using HiveMind.Mushroom;
using System.Collections.Generic;
using UnityEngine;

namespace HiveMind.UI
{
    public class MushroomIndicatorManager : MonoBehaviour
    {
        #region REFERENCES
        private Camera cam;
        private MushroomManager mushroomManager;
        private GameObject player;
        #endregion

        #region FIELDS
        [SerializeField] List<MushroomIndicator> indicatorList;
        private float camWidth;
        private float camHeight;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
            mushroomManager = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
            player = GameObject.Find("Player");

            // Create bounds
            camHeight = cam.orthographicSize;
            camWidth = camHeight * cam.aspect;

            // Fill list
            indicatorList.Add(GameObject.Find("Mushroom Indicator 1").GetComponent<MushroomIndicator>());
            indicatorList.Add(GameObject.Find("Mushroom Indicator 2").GetComponent<MushroomIndicator>());
            indicatorList.Add(GameObject.Find("Mushroom Indicator 3").GetComponent<MushroomIndicator>());
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = player.transform.position;

            // If there are no Mushrooms in mushroomManager's MushroomList,
            // reset all the Indicators
            if (mushroomManager.MushroomList.Count == 0)
            {
                foreach (MushroomIndicator indicator in indicatorList)
                {
                    indicator.LinkedToMushroom = false;
                    indicator.MushroomToIndicate = null;
                }
            }

            // Check if there are Mushrooms in mushroomManager's MushroomList
            if (mushroomManager.MushroomList.Count > 0)
            {
                // Go through the list of Mushrooms within the mushroomManager's MushroomList
                for (int i = 0; i < mushroomManager.MushroomList.Count; i++)
                {
                    // If the Indicator at indicatorList[i] is not linked to anything, link it to the Mushroom
                    // at mushroomManager.MushroomList[i]
                    if (!indicatorList[i].LinkedToMushroom && mushroomManager.MushroomList[i].GetComponent<Shroom>().IsShroom)
                    {
                        indicatorList[i].MushroomToIndicate = mushroomManager.MushroomList[i];
                        indicatorList[i].LinkedToMushroom = true;
                    }
                    else
                    {
                        // Otherwise, if the Indicator at indicatorList[i] is linked to a Mushroom, check if it is the same
                        // Mushroom, as it might have changed - if it is not, change it
                        if (indicatorList[i].MushroomToIndicate != mushroomManager.MushroomList[i] && mushroomManager.MushroomList[i].GetComponent<Shroom>().IsShroom)
                        {
                            indicatorList[i].MushroomToIndicate = mushroomManager.MushroomList[i];
                        }
                    }
                }

                // If indexCount is less than indicatorList.Count - meaning there are less shrooms
                // than indicators on the screen - reset the remaining indicators
                if (mushroomManager.MushroomList.Count < indicatorList.Count)
                {
                    int remainingIndexes = indicatorList.Count - mushroomManager.MushroomList.Count;
                    for (int i = mushroomManager.MushroomList.Count; i < indicatorList.Count; i++)
                    {
                        indicatorList[i].LinkedToMushroom = false;
                        indicatorList[i].MushroomToIndicate = null;
                    }
                }
            }

            CheckIndicatorBounds();
        }

        /// <summary>
        /// Check if any Mushrooms are offscreen
        /// </summary>
        public void CheckIndicatorBounds()
        {
            // Set default Main Camera bounds
            float leftBound = player.transform.position.x - camWidth;
            float rightBound = player.transform.position.x + camWidth;
            float lowerBound = player.transform.position.y - camHeight;
            float upperBound = player.transform.position.y + camHeight;

            // Go through each Mushroom in mushroomManager's MushroomList
            foreach (GameObject mushroom in mushroomManager.MushroomList)
            {
                // Check if any Mushrooms are out of the Camera bounds
                if (mushroom.transform.position.x < leftBound || mushroom.transform.position.x > rightBound || mushroom.transform.position.y < lowerBound || mushroom.transform.position.y > upperBound)
                {
                    // If so, then set OnScreen to false
                    mushroom.GetComponent<Shroom>().OnScreen = false;
                }
                else
                {
                    // Otherwise, set OnScreen to true
                    mushroom.GetComponent<Shroom>().OnScreen = true;
                }
            }


        }
    }
}
