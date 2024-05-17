using Cinemachine;
using HiveMind.Events;
using HiveMind.SaveData;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace HivevMind.Cinematics
{
    public class CameraManager : MonoBehaviour, IData
    {
        #region REFERENCES
        [SerializeField] private CameraScriptableObj cameraEvent;
        [SerializeField] private CutsceneScriptableObj cutsceneEvent;
        [SerializeField] private PauseScriptableObj pauseEvent;
        [SerializeField] private JournalScriptableObj journalEvent;
        [SerializeField] private DisableScriptableObj disableEvent;
        private Camera mainCam;
        private CinemachineVirtualCamera mainCineVirt;
        private CinemachineVirtualCamera lotusCam;
        [SerializeField] private PlayableDirector camDirector; // Initialized in Inspector
        #endregion

        #region FIELDS
        [SerializeField] private bool gameCam;

        [Header("Cutscenes")]
        [SerializeField] private bool playCutscene = true;
        [SerializeField] private bool usingLotusCutscene = true;
        private bool deactivateLotusCam = false;
        private bool loadedData;
        #endregion

        #region PROPERTIES
        public bool UsingLotusCutscene { get { return usingLotusCutscene; } set { usingLotusCutscene = value; } }
        public bool PlayCutscene { get { return playCutscene; } }
        #endregion

        private void OnEnable()
        {
            cutsceneEvent.startLotusCutscene.AddListener(BeginLotusCutscene);
        }

        private void OnDisable()
        {
            cutsceneEvent.startLotusCutscene.RemoveListener(BeginLotusCutscene);
        }

        // Start is called before the first frame update
        void Start()
        {
            // Set cameras
            mainCam = Camera.main;

            if (gameCam)
            {
                mainCineVirt = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
                lotusCam = GameObject.Find("Lotus Cam").GetComponent<CinemachineVirtualCamera>();
            }

            cameraEvent.SetMainCamera(mainCam);


            // Create bounds
            cameraEvent.SetCamHeight(mainCam.orthographicSize);
            cameraEvent.SetCamWidth(mainCam.orthographicSize * mainCam.aspect);

            // Set default viewports
            // Set the viewports of the Main Camera and the Cinemachine camera
            cameraEvent.SetMainViewport(CalculateViewportRectangleMain());
            cameraEvent.SetCMViewport(CalculateViewportRectangleCM());
            cameraEvent.SetCurrentCMViewport(CalculateViewportRectangleCM());
            cameraEvent.SetPreviousCMViewport(cameraEvent.GetCurrentCMViewport());

            if (camDirector == null)
            {
                playCutscene = false;
            }
            else
            {
                // Check if data has been loaded before setting to true - player might have already seen the cutscene
                if (!loadedData)
                {
                    playCutscene = true;
                }
            }

            // If play cutscene is disabled, disable the cam director
            if (!playCutscene)
            {
                camDirector.enabled = false;
            }
        }

        void Update()
        {
            // Set the viewports of the Main Camera and the Cinemachine camera
            cameraEvent.SetMainViewport(CalculateViewportRectangleMain());
            cameraEvent.SetCMViewport(CalculateViewportRectangleCM());
            cameraEvent.SetCurrentCMViewport(CalculateViewportRectangleCM());

            if (Mathf.Abs(cameraEvent.GetCurrentCMViewport().x - cameraEvent.GetPreviousCMViewport().x) > 0.001
                || Mathf.Abs(cameraEvent.GetCurrentCMViewport().y - cameraEvent.GetPreviousCMViewport().y) > 0.001)
            {
                // Calculate width offset
                // Negative values - Cinemachine is to the left of the main Camera
                // Positive values - Cinemachine is to the right of the main Camera
                float widthOffset = cameraEvent.GetCMViewport().x - cameraEvent.GetMainViewport().x;
                cameraEvent.SetLeftBound(cameraEvent.GetCMViewport().xMin - widthOffset);
                cameraEvent.SetRightBound(cameraEvent.GetCMViewport().xMax - widthOffset);
                cameraEvent.SetViewportWidthOffset(widthOffset);

                // Calculate height offset
                // Negative values - Cinemachine is below the main Camera
                // Positive values - Cinemachine is above the main Camera
                float heightOffset = cameraEvent.GetCMViewport().y - cameraEvent.GetMainViewport().y;
                cameraEvent.SetBottomBound(cameraEvent.GetCMViewport().yMin - heightOffset);
                cameraEvent.SetTopBound(cameraEvent.GetCMViewport().yMax - heightOffset);
                cameraEvent.SetViewportHeightOffset(heightOffset);

                // Set Camera to moving
                cameraEvent.SetCameraMoving(true);

                // Trigger move events
                cameraEvent.Move();
            }
            else
            {
                // Sset Camera to not moving
                cameraEvent.SetCameraMoving(false);
            }

            // Update the previous Cinemachine viewport
            cameraEvent.SetPreviousCMViewport(cameraEvent.GetCurrentCMViewport());
        }

        /// <summary>
        /// Calculate the viewport rectangle of the Cinemachine camera
        /// </summary>
        /// <returns>The viewport rectangle of the Cinemachine camera</returns>
        private Rect CalculateViewportRectangleCM()
        {
            float orthographicSize = mainCineVirt.m_Lens.OrthographicSize;
            float aspectRatio = mainCineVirt.m_Lens.Aspect;

            float viewportHeight = orthographicSize * 2;
            float viewportWidth = viewportHeight * aspectRatio;

            Vector2 cameraPosition = mainCineVirt.transform.position;

            Rect viewportRect = new Rect(
                cameraPosition.x - viewportWidth / 2,
                cameraPosition.y - viewportHeight / 2,
                viewportWidth,
                viewportHeight);

            return viewportRect;
        }

        /// <summary>
        /// Calculate the viewport rectangle of the Main Camera
        /// </summary>
        /// <returns>The viewport rectangle of the Main Camera</returns>
        private Rect CalculateViewportRectangleMain()
        {
            float orthographicSize = mainCam.orthographicSize;
            float aspectRatio = mainCam.aspect;

            float camHeight = orthographicSize * 2;
            float camWidth = camHeight * aspectRatio;

            Rect viewportRect = new Rect(
                mainCam.transform.position.x - camWidth / 2,
                mainCam.transform.position.y - camHeight / 2,
                camWidth,
                camHeight);

            return viewportRect;
        }

        /// <summary>
        /// Begin the lotus cutscene
        /// </summary>
        public void BeginLotusCutscene()
        {
            // If playing the cutscene and using the cutscene, enable he lotus cam
            if (playCutscene && usingLotusCutscene)
            {
                // Set playing cutscene to true
                cutsceneEvent.SetPlayingCutscene(true);

                // Enable the lotus cam
                lotusCam.enabled = true;

                // Play the cutscene
                camDirector.Play();
                StartCoroutine(PlayLotusCutscene());
            }
        }

        /// <summary>
        /// Play the lotus cutscene
        /// </summary>
        /// <returns></returns>
        public IEnumerator PlayLotusCutscene()
        {
            // Check if a cutscene is supposed to be played, if so, then play it
            while (playCutscene)
            {
                yield return null;

                if (camDirector.state != PlayState.Playing)
                {
                    if (!deactivateLotusCam)
                    {
                        // De-activate lotus cam
                        lotusCam.enabled = false;
                    }

                    // Deactivate the camera only once
                    deactivateLotusCam = true;

                    // Set play cutscene to false so it does not replay
                    playCutscene = false;

                    // Allow the player to pause after the cutscene
                    pauseEvent.SetCanPause(true);

                    // Allow the player to open the journal after the cutscene
                    journalEvent.SetCanOpen(true);

                    // Set playing cutscene to false
                    cutsceneEvent.SetPlayingCutscene(false);

                }
                else
                {
                    // Do not allow the player to pause during the cutscene
                    pauseEvent.SetCanPause(false);

                    // Do not allow the player to open the journal during the cutscene
                    journalEvent.SetCanOpen(false);
                }
            }

            // If the cutscene is no longer playing, end it
            if (!playCutscene)
            {
                cutsceneEvent.EndLotusCutscene();

                // Enable the HUD
                disableEvent.EnableHUD();
            }

            yield break;
        }

        public void LoadData(GameData data)
        {
            // Get the currently active scene
            Scene scene = SceneManager.GetActiveScene();

            // Check if that scene name exists in the dictionary for good measure
            if (data.levelData.ContainsKey(scene.name))
            {
                // If it does exist, load the cutscene's play data using the data for this scene
                bool playCutsceneForThisScene = data.levelData[scene.name].playCutscene;
                playCutscene = playCutsceneForThisScene;
            }
            else
            {
                // If it doesn't exist, let ourselves know that we need to add it to our game data
                Debug.LogError("Failed to get data for scene with name: " + scene.name + ". It may need to be added to the GameData constructor");
            }

            loadedData = true;
        }

        public void SaveData(GameData data)
        {
            // Save the cutscene's play data
            Scene scene = SceneManager.GetActiveScene();
            data.levelData[scene.name].playCutscene = playCutscene;
        }
    }
}
