using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CameraManager : MonoBehaviour, IData
{
    #region REFERENCES
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    [SerializeField] private PauseScriptableObj pauseEvent;
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
    #endregion

    private void OnEnable()
    {
        cutsceneEvent.startLotusCutscene.AddListener(StartLotusCutscene);
    }

    private void OnDisable()
    {
        cutsceneEvent.startLotusCutscene.RemoveListener(StartLotusCutscene);
    }

    // Start is called before the first frame update
    void Start()
    {
        lotusCam = GameObject.Find("Lotus Cam").GetComponent<CinemachineVirtualCamera>();

        if (camDirector == null)
        {
            playCutscene = false;
        } else
        {
            // Check if data has been loaded before setting to true - player might have already seen the cutscene
            if(!loadedData)
            {
                playCutscene = true;
            }
        }

        // If play cutscene is disabled, disable the cam director
        if(!playCutscene)
        {
            camDirector.enabled = false;
        }

        // If playing the cutscene and using the cutscene, enable he lotus cam
        if(playCutscene && usingLotusCutscene)
        {
            lotusCam.enabled = true;

            // Start the cutscene coroutine
            cutsceneEvent.StartLotusCutscene();
        }
    }

    public void StartLotusCutscene()
    {
        StartCoroutine(PlayLotusCutscene());
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
                cutsceneEvent.EndLotusCutscene();

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

            } else
            {
                // Do not allow the player to pause during the cutscene
                pauseEvent.SetCanPause(false);
            }
        }
    }

    public void LoadData(GameData data)
    {
        playCutscene = data.playCutscene;
        loadedData = true;
    }

    public void SaveData(GameData data)
    {
        data.playCutscene = playCutscene;
    }
}
