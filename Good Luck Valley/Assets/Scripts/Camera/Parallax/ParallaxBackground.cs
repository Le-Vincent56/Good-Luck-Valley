using GoodLuckValley.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    [ExecuteInEditMode]
    public class ParallaxBackground : MonoBehaviour
    {
        public ParallaxCamera parallaxCamera;
        List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

        private void Start()
        {
            // Get the parallax camera
            if (parallaxCamera == null)
                parallaxCamera = Camera.main.gameObject.GetOrAdd<ParallaxCamera>();

            // Subscribe to events
            if (parallaxCamera != null)
                parallaxCamera.OnCameraTranslate += Move;

            // Set the layers
            SetLayers();
        }

        private void OnEnable()
        {
            // Subscribe to events
            if (parallaxCamera != null)
                parallaxCamera.OnCameraTranslate += Move;
        }

        private void OnDisable()
        {
            // Unsubscribe to events
            if(parallaxCamera != null)
                parallaxCamera.OnCameraTranslate -= Move;
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

                if(layer != null)
                {
                    layer.name = "Layer-" + i;
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
           foreach(ParallaxLayer layer in parallaxLayers)
            {
                layer.Move(delta);
            }
        }
    }
}