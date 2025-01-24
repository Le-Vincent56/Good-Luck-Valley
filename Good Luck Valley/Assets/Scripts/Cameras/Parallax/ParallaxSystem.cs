using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    [BurstCompile]
    public class ParallaxSystem : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3 cameraStartPosition;
        [SerializeField] private ParallaxLayer[] layers;

        private NativeArray<ParallaxData> layersNative;

        private void Awake()
        {
            // Store the main Camera
            mainCamera = Camera.main;

            // Set the starting camera position
            cameraStartPosition = mainCamera.transform.position;

            // Initialize the Parallax System
            InitializeParallax();
        }

        private void OnDestroy()
        {
            // Dispose of the Native Layers if destroyed
            if(layersNative.IsCreated) layersNative.Dispose();
        }

        private void LateUpdate()
        {
            // Exit case - the Native Array is not created
            if(!layersNative.IsCreated) return;

            // Create the Parallax Job
            ParallaxJob parallaxJob = new ParallaxJob
            {
                ParallaxData = layersNative,
                CameraStartPos = cameraStartPosition,
                CameraCurrentPos = mainCamera.transform.position
            };

            // Create the Parallax Job Handle
            JobHandle parallaxJobHandle = parallaxJob.Schedule(layersNative.Length, 64);

            // Wait for the Job Handle to complete
            parallaxJobHandle.Complete();

            // Set Parallax Layer positions
            for (int i = 0; i < layersNative.Length; i++)
            {
                layers[i].transform.position = layersNative[i].CurrentPosition;
            }
        }

        /// <summary>
        /// Initialize the Parallax System
        /// </summary>
        private void InitializeParallax()
        {
            // Dispose the Native Array is already created
            if (layersNative.IsCreated) layersNative.Dispose();

            // Store the Parallax Layers
            layers = GetComponentsInChildren<ParallaxLayer>();

            // Create the Native Array of Parallax Data
            layersNative = new NativeArray<ParallaxData>(layers.Length, Allocator.Persistent);

            // Iterate through each Parallax Layer
            for (int i = 0; i < layers.Length; i++)
            {
                // Initialize the Parallax Layer
                layers[i].Initialize();

                // Store the Parallax Layer Data
                layersNative[i] = new ParallaxData
                {
                    StartPosition = layers[i].StartPosition,
                    Multiplier = layers[i].Multiplier,
                    HorizontalOnly = layers[i].HorizontalOnly
                };
            };
        }
    }
}
