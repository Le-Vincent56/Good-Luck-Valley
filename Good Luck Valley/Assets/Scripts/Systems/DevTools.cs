using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevTools : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private MushroomManager mushMan;
    [SerializeField] private PlayerMovement playerMove;
    private Text devText;
    private Text noClipText;
    private Text instantThrowText;
    private Text infiniteShroomText;
    private Text shroomDurationText;
    private Settings settings;
    private GameObject textHolder;
    #endregion

    #region FIELDS
    [SerializeField] bool devToolsEnabled;
    [SerializeField] bool noClip;
    [SerializeField] bool instantThrow;
    [SerializeField] bool infiniteShrooms;
    [SerializeField] bool disableShroomDuration;
    private BoxCollider2D playerCollider;
    private CapsuleCollider2D capsuleCollider;
    #endregion

    #region PROPERTIES
    public bool NoClip { get { return noClip; } set {  noClip = value; } }
    public bool InstantThrow { get { return instantThrow; } set { instantThrow = value; } }
    public bool InfiniteShrooms { get { return infiniteShrooms; } set { infiniteShrooms = value; } }
    public bool DisableShroomDuration { get { return disableShroomDuration; } set { disableShroomDuration = value; } }
    public bool DevToolsEnabled { get { return devToolsEnabled; } set { devToolsEnabled = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        playerMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
        playerCollider = playerMove.GetComponentInParent<BoxCollider2D>();
        capsuleCollider = playerMove.GetComponentInParent<CapsuleCollider2D>();
        settings = GameObject.Find("MenusManager").GetComponent<Settings>();

        // Get the canvas contianing the text boxes
        GameObject canvas = transform.GetComponentInChildren<Canvas>().gameObject;

        // Get the empty object holding the text boxes
        textHolder = canvas.transform.GetChild(0).gameObject;

        // Get all the textboxes
        devText = textHolder.transform.GetChild(0).GetComponent<Text>();
        noClipText = textHolder.transform.GetChild(1).GetComponentInChildren<Text>();
        instantThrowText = textHolder.transform.GetChild(2).GetComponentInChildren<Text>();
        infiniteShroomText = textHolder.transform.GetChild(3).GetComponentInChildren<Text>();
        shroomDurationText = textHolder.transform.GetChild(4).GetComponentInChildren<Text>();

        // Checks if dev tools are enabled
        if (devToolsEnabled)
        {
            // Default dev tools values
            noClip = false;
            instantThrow = false;
            infiniteShrooms = true;
            mushMan.ThrowUnlocked = true;
            disableShroomDuration = true;

            settings.NoClipOn = false;
            settings.InfiniteShroomsOn = true;
            settings.InstantThrowOn = false;
            settings.ShroomDurationOn = false;
            settings.ThrowIndicatorShown = true;
        }
        else
        {
            noClip = settings.NoClipOn;
            instantThrow = settings.InstantThrowOn;
            infiniteShrooms = settings.InfiniteShroomsOn;
            mushMan.ThrowUnlocked = false;
            disableShroomDuration = !settings.ShroomDurationOn;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (devToolsEnabled)
        {
            // Dev Tools text change
            devText.text = "Dev Tools Enabled";
            mushMan.ThrowUnlocked = true;

            // No Clip text change
            if (noClip == true)
            {
                noClipText.text = "Press F1 for no-clip: Enabled";
            }
            else if (noClip == false)
            {
                noClipText.text = "Press F1 for no-clip: Disabled";
            }

            // Instant Throw text change
            if (instantThrow)
            {
                instantThrowText.text = "Press F2 for 'instant' shroom throw: Enabled";
            }
            else if (instantThrow == false)
            {
                instantThrowText.text = "Press F2 for 'instant' shroom throw: Disabled";
            }

            // Infinite shrooms text change
            if (infiniteShrooms)
            {
                infiniteShroomText.text = "Press F3 for infinite shrooms: Enabled";
            }
            else if (infiniteShrooms == false)
            {
                infiniteShroomText.text = "Press F3 for infinite shrooms: Disabled";
            }

            // Disable shroom timer text change
            if (disableShroomDuration)
            {
                shroomDurationText.text = "Press F4 to disable/enable shroom timers: Timers Disabled";
            }
            else if (disableShroomDuration == false)
            {
                shroomDurationText.text = "Press F4 to disable/enable shroom timers: Timers Enabled";
            }
        } 
        else
        {
            // Dev Tools text change
            devText.text = "";
            shroomDurationText.text = "";
            infiniteShroomText.text = "";
            instantThrowText.text = "";
            noClipText.text = "";
        }
    }

    #region INPUT HANDLERS
    public void OnActivateNoClip()
    {
        // Check if devTools is enabled
        if (devToolsEnabled)
        {
            // Switch noClip on/off
            noClip = !noClip;

            // Switch collider's isTrigger bool
            playerCollider.isTrigger = !playerCollider.isTrigger;
            capsuleCollider.isTrigger = !capsuleCollider.isTrigger;

            // Check if the rigid body is dynamic type, if it is then set it to static
            if (playerMove.RB.bodyType == RigidbodyType2D.Dynamic)
            {
                playerMove.RB.bodyType = RigidbodyType2D.Static;
            }
            // Otherwise set it to dynamic
            else
            {
                playerMove.RB.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    public void OnActivateInstantThrow()
    {
        // Check if the dev tools are enabled
        if (devToolsEnabled)
        {
            // Switch instant throw on/off
            instantThrow = !instantThrow;

            // Check if it was set to enabled or disabled
            if (instantThrow)
            {
                // If enabled, set throw multiplier to 30 for fast/instant shroom
                mushMan.ThrowMultiplier = 30;
            }
            else if (instantThrow == false)
            {
                // If disabled, set throw multiplier to 8, original value
                mushMan.ThrowMultiplier = 8;
            }
        }
    }

    public void OnEnableInfiniteShrooms()
    {
        // Check if the dev tools are enabled
        if (devToolsEnabled)
        {
            // Switch infinite shrooms on/off
            infiniteShrooms = !infiniteShrooms;

            // Check if it was set to enabled or disabled
            if (infiniteShrooms)
            {
                // If enabled, set shroom limit to max value, 'infinite'
                mushMan.MushroomLimit = int.MaxValue;
            }   
            else if (infiniteShrooms == false)
            {
                // If disabled, set shroom limit to 3, original value
                mushMan.MushroomLimit = 3;
            }
        } 
    }

    public void OnDisableShroomDuration()
    {
        // Check if dev tools is enabled
        if (devToolsEnabled)
        {
            // Switches shroom duration on/off
            disableShroomDuration = !disableShroomDuration;

            // Checks if shroom durations are enabled or disabled
            if (disableShroomDuration)
            {
                // If disabled, turns off shroom timers
                mushMan.EnableShroomTimers = false;
            }
            else if (disableShroomDuration == false)
            {
                // If enabled, turns on shroom timers
                mushMan.EnableShroomTimers = true;
            }
        }
    }

    public void OnCutToLevel(int level)
    {
        if (devToolsEnabled)
        {
            SceneManager.LoadScene("Level " + level);
        }
    }
    #endregion
}