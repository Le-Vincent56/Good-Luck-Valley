using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DevTools : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private MushroomManager mushMan;
    [SerializeField] private PlayerMovement playerMove;
    private MenusManager menusMan;
    private Text devText;
    private Text noClipText;
    private Text instantThrowText;
    private Text infiniteShroomText;
    private Text shroomDurationText;
    #endregion

    #region FIELDS
    [SerializeField] static bool devToolsEnabled;
    [SerializeField] static bool noClip;
    [SerializeField] static bool instantThrow;
    [SerializeField] static bool infiniteShrooms;
    [SerializeField] static bool disableShroomDuration;
    static bool accessibilityToolsOn;
    private BoxCollider2D playerCollider;
    private CapsuleCollider2D capsuleCollider;
    #endregion

    #region PROPERTIES
    public bool NoClip { get { return noClip; } set {  noClip = value; } }
    public bool DevToolsEnabled { get { return devToolsEnabled; } set { devToolsEnabled = value; } }
    public bool AccessibilityToolsOn { get { return accessibilityToolsOn; } set {  accessibilityToolsOn = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (menusMan = GameObject.Find("MenusManager").GetComponent<MenusManager>())
        {
            if (menusMan.CurrentScene != 4)
            {
                mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
                playerMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
                playerCollider = playerMove.GetComponentInParent<BoxCollider2D>();
                capsuleCollider = playerMove.GetComponentInParent<CapsuleCollider2D>();
                devText = GameObject.Find("DevText").GetComponent<Text>();
                noClipText = GameObject.Find("NoClipText").GetComponent<Text>();
                instantThrowText = GameObject.Find("InstantShroomText").GetComponent<Text>();
                infiniteShroomText = GameObject.Find("InfiniteShroomText").GetComponent<Text>();
                shroomDurationText = GameObject.Find("ShroomDurationText").GetComponent<Text>();

                if (noClip)
                {
                    OnActivateNoClip();
                }

                if (instantThrow)
                {
                    OnActivateInstantThrow();
                }

                if (infiniteShrooms)
                {
                    OnEnableInfiniteShrooms();
                }

                if (disableShroomDuration)
                {
                    OnDisableShroomDuration();
                }
            }

            // Checks if dev tools are enabled
            if (devToolsEnabled)
            {
                // Default dev tools values
                noClip = false;
                instantThrow = false;
                infiniteShrooms = true;
                mushMan.ThrowUnlocked = true;
                disableShroomDuration = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (menusMan.CurrentScene != 4)
        {
            // No Clip text Change
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
    }

    #region INPUT HANDLERS
    public void OnActivateNoClip()
    {
        // Check if devTools is enabled
        if (devToolsEnabled || accessibilityToolsOn)
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
        if (devToolsEnabled || accessibilityToolsOn)
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
        if (devToolsEnabled || accessibilityToolsOn)
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
       if (devToolsEnabled || accessibilityToolsOn)
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
    #endregion
}