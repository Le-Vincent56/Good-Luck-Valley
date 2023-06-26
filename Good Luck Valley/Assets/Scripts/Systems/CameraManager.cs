using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CameraManager : MonoBehaviour, IData
{
    #region REFERENCES
    private CinemachineVirtualCamera lotusCam;
    [SerializeField] private PlayableDirector camDirector; // Initialized in Inspector
    private PlayerMovement playerMovement;
    private MushroomManager mushroomManager;
    private PauseMenu pauseMenu;
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

    // Start is called before the first frame update
    void Start()
    {
        lotusCam = GameObject.Find("LotusCam").GetComponent<CinemachineVirtualCamera>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        mushroomManager = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();

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

            // Check if the director is playing the cutscene
            if (camDirector.state == PlayState.Playing)
            {
                // Disable the player to pause
                pauseMenu.CanPause = false;

                // If playing, lock player movement
                playerMovement.IsLocked = true;

                // Lock mushroom throw
                if (mushroomManager.ThrowUnlocked)
                {
                    mushroomManager.ThrowLocked = true;
                }
            }
            else
            {
                // Enable the player to pause
                pauseMenu.CanPause = true;

                // If not playing, unlock player movement
                playerMovement.IsLocked = false;

                // Unlock mushroom throw
                if (mushroomManager.ThrowUnlocked)
                {
                    mushroomManager.ThrowLocked = false;
                }

                if (!deactivateLotusCam)
                {
                    // De-activate lotus cam
                    lotusCam.enabled = false;
                }

                // Deactivate the camera only once
                deactivateLotusCam = true;

                // Set play cutscene to false so it does not replay
                playCutscene = false;
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
