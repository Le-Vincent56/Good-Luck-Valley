using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour, IData
{
    #region REFERENCES
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    [SerializeField] private PauseScriptableObj pauseEvent;
    [SerializeField] private JournalScriptableObj journalEvent;
    private CinemachineVirtualCamera lotusCam;
    [SerializeField] private PlayableDirector camDirector; // Initialized in Inspector
    #endregion

    #region FIELDS
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
        lotusCam = GameObject.Find("Lotus Cam").GetComponent<CinemachineVirtualCamera>();

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

    public void BeginLotusCutscene()
    {
        // If playing the cutscene and using the cutscene, enable he lotus cam
        if (playCutscene && usingLotusCutscene)
        {
            lotusCam.enabled = true;

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
        while(playCutscene)
        {
            yield return null;

            if(camDirector.state != PlayState.Playing)
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

            } else
            {
                // Do not allow the player to pause during the cutscene
                pauseEvent.SetCanPause(false);

                // Do not allow the player to open the journal during the cutscene
                journalEvent.SetCanOpen(false);
            }
        }

        if(!playCutscene)
        {
            cutsceneEvent.EndLotusCutscene();
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
