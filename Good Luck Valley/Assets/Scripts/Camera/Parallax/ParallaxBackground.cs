using GoodLuckValley.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    [ExecuteInEditMode]
    public class ParallaxBackground : MonoBehaviour
    {
        public ParallaxCamera parallaxCamera;
        [SerializeField] List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();
        private bool isSubscribed = false;
        [SerializeField] private int timesMoved;

        private void Start()
        {
            // Get the parallax camera
            if (parallaxCamera == null)
                parallaxCamera = Camera.main.gameObject.GetOrAdd<ParallaxCamera>();
                

            // Set the layers
            SetLayers();

            // Subscribe to events if not already subscribed
            SubscribeToEvent();
        }

        private void OnEnable()
        {
            // Subscribe to events if not already subscribed
            SubscribeToEvent();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvent();
        }

        private void SubscribeToEvent()
        {
            if (!isSubscribed && parallaxCamera != null)
            {
                parallaxCamera.OnCameraTranslate += Move;
                isSubscribed = true;
            }
        }

        private void UnsubscribeFromEvent()
        {
            if (isSubscribed && parallaxCamera != null)
            {
                parallaxCamera.OnCameraTranslate -= Move;
                isSubscribed = false;
            }
        }

        /// <summary>
        /// Set the layers for parallax
        /// </summary>
        private void SetLayers()
        {
            // Clear the current layers
            parallaxLayers.Clear();

            // Loop through each child
            for (int i = 0; i < transform.childCount; i++)
            {
                ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

                if (layer != null)
                {
                    parallaxLayers.Add(layer);
                }
            }
        }

        /// <summary>
        /// Move the parallax layers
        /// </summary>
        /// <param name="delta">The base amount to move</param>
        private void Move(float delta)
        {
            timesMoved++;
            foreach (ParallaxLayer layer in parallaxLayers)
            {
                layer.Move(delta);
            }
        }
    }
}