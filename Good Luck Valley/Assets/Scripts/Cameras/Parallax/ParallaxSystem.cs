using Cysharp.Threading.Tasks;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Scenes;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    [BurstCompile]
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ParallaxSystem : MonoBehaviour, ILoadingTask
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3 cameraStartPosition;
        [SerializeField] private List<ParallaxLayer> layers;
        private SceneLoader sceneLoader;

        private NativeArray<ParallaxData> layersNative;

        private void OnValidate()
        {
            // Set up Parallax to be used in the Editor
            Setup();
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            if (sceneLoader == null) sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
            sceneLoader.QueryTasks += RegisterTask;
        }

        private void OnDisable()
        {
            // Dispose of the Native Layers if disabled
            if (layersNative.IsCreated) layersNative.Dispose();
        }

        private void OnDestroy()
        {
            // Dispose of the Native Layers if destroyed
            if(layersNative.IsCreated) layersNative.Dispose();

            if (sceneLoader == null) return;
            sceneLoader.QueryTasks -= RegisterTask;
        }

        private void Update()
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
        /// Set up the parallax
        /// </summary>
        private void Setup()
        {
            // Dispose of the Native Layers if not playing
            if (layersNative.IsCreated) layersNative.Dispose();

            // Store the main Camera
            mainCamera = Camera.main;

            // Set the starting camera position
            cameraStartPosition = mainCamera.transform.position;

            // Initialize the Parallax System
            InitializeParallax();
        }

        /// <summary>
        /// Initialize the Parallax System
        /// </summary>
        private void InitializeParallax()
        {
            // Dispose the Native Array is already created
            if (layersNative.IsCreated) layersNative.Dispose();

            // Create the Parallax Layers list
            layers = new List<ParallaxLayer>();

            // Store the Parallax Layers
            GetComponentsInChildren(layers);

            // Create the Native Array of Parallax Data
            layersNative = new NativeArray<ParallaxData>(layers.Count, Allocator.Persistent);

            // Iterate through each Parallax Layer
            for (int i = 0; i < layers.Count; i++)
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

        /// <summary>
        /// Register a Layer to the Parallax System
        /// </summary>
        public void RegisterLayer(ParallaxLayer layer)
        {
            // Exit case - the Parallax Layer is already registered
            if (layers.Contains(layer)) return;

            // Add the layer
            layers.Add(layer);

            // Update the layers
            UpdateLayers();
        }

        /// <summary>
        /// Deregister a Layer to the Parallax System
        /// </summary>
        public void DeregisterLayer(ParallaxLayer layer)
        {
            // Exit case - the Parallax Layer is not registered
            if (!layers.Contains(layer)) return;

            // Remove the layer
            layers.Remove(layer);

            // Update the layers
            UpdateLayers();
        }

        /// <summary>
        /// Update the Layers Native Array
        /// </summary>
        public void UpdateLayers()
        {
            // Check if the Native Array is created
            if (layersNative.IsCreated)
                // Dispose of the array to prepare for a new one
                layersNative.Dispose();

            // Create the Native Array of Parallax Data
            layersNative = new NativeArray<ParallaxData>(layers.Count, Allocator.Persistent);

            // Iterate through each Parallax Layer
            for (int i = 0; i < layers.Count; i++)
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

        /// <summary>
        /// Register this task to the Scene Loader
        /// </summary>
        public void RegisterTask() => sceneLoader.RegisterPreTask(SetParallaxTask, 1);

        /// <summary>
        /// Set the Parallax Task
        /// </summary>
        public UniTask SetParallaxTask()
        {
            // Set up the parallax
            Setup();

            // Register this as a service
            ServiceLocator.ForSceneOf(this).Register(this);

            return UniTask.NextFrame();
        }
    }
}
