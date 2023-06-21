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
    #endregion

    #region FIELDS
    [SerializeField] private bool playCutscene = true;
    [SerializeField] private bool usingLotusCutscene = true;
    private bool deactivateLotusCam = false;
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

        if (camDirector == null)
        {
            playCutscene = false;
        } else
        {
            playCutscene = true;
        }

        if(playCutscene && usingLotusCutscene)
        {
            lotusCam.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the cutscene is being played
        if(playCutscene)
        {
            // Check if the director is playing the cutscene
            if(camDirector.state == PlayState.Playing)
            {
                // If playing, lock player movement
                playerMovement.IsLocked = true;

                // Lock mushroom throw
                if(mushroomManager.ThrowUnlocked)
                {
                    mushroomManager.ThrowLocked = true;
                }
            } else
            {
                // If not playing, unlock player movement
                playerMovement.IsLocked = false;

                // Unlock mushroom throw
                if(mushroomManager.ThrowUnlocked)
                {
                    mushroomManager.ThrowLocked = false;
                }

                if(!deactivateLotusCam)
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
    }

    public void SaveData(GameData data)
    {
        data.playCutscene = playCutscene;
    }
}
