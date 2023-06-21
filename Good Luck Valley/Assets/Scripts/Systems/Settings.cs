using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    #region REFERENCES
    private MenusManager menusMan;
    [SerializeField] private MushroomManager mushMan;
    [SerializeField] private PlayerMovement playerMove;
    #endregion

    #region FIELDS
    private static bool throwIndicatorShown = false;
    private static bool noClipOn = false;
    private static bool instantThrowOn = false;
    private static bool infiniteShroomsOn = false;
    private static bool shroomDurationOn = true;
    private static bool updateSettings;
    private static float brightness;
    private static bool subtitlesEnabled;
    private static int resOption;
    private static bool isFullscreen;
    private BoxCollider2D playerCollider;
    private CapsuleCollider2D capsuleCollider;
    #endregion

    #region PROPERTIES
    public bool ThrowIndicatorShown { get { return throwIndicatorShown; } set { throwIndicatorShown = value; } }
    public bool NoClipOn { get { return noClipOn; } set { noClipOn = value; } }
    public bool InstantThrowOn { get { return instantThrowOn; } set { instantThrowOn = value; } }
    public bool InfiniteShroomsOn { get { return infiniteShroomsOn; } set { infiniteShroomsOn = value; } }
    public bool ShroomDurationOn { get { return shroomDurationOn; } set { shroomDurationOn = value; } }
    public bool UpdateSettings { get {  return updateSettings; } set {  updateSettings = value; } }
    public float Brightness { get { return brightness; } set {  brightness = value; } }
    public bool SubtitlesEnabled { get {  return subtitlesEnabled; } set {  subtitlesEnabled = value; } }
    public int ResOption { get { return resOption; } set { resOption = value; } }
    public bool IsFullscreen { get { return isFullscreen; } set { isFullscreen = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        menusMan = GameObject.Find("MenusManager").GetComponent<MenusManager>();
        if (menusMan.CurrentScene > 5)
        {
            mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
            playerMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
            playerCollider = playerMove.GetComponentInParent<BoxCollider2D>();
            capsuleCollider = playerMove.GetComponentInParent<CapsuleCollider2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (menusMan.CurrentScene > 5)
        {
            if (updateSettings)
            {
                if (noClipOn)
                {
                    ActivateNoClip();
                }
                else
                {
                    DeactivateNoClip();
                }

                if (instantThrowOn)
                {
                    ActivateInstantShroom();
                }
                else
                {
                    DeactivateInstantShroom();
                }

                if (infiniteShroomsOn)
                {
                    ActivateInfiniteShrooms();
                }
                else
                {
                    DeactivateInfiniteShrooms();
                }

                if (shroomDurationOn)
                {
                    ActivateShroomTimers();
                }
                else
                {
                    DeactivateShroomTimers();
                }

                if (throwIndicatorShown)
                {
                    EnableThrowLine();
                }
                else
                {
                    DisableThrowLine();
                }

                updateSettings = false;
            }
        }
    }

    #region NO CLIP
    private void ActivateNoClip()
    {
        // Switch collider's isTrigger bool
        playerCollider.isTrigger = true;
        capsuleCollider.isTrigger = true;
        playerMove.RB.bodyType = RigidbodyType2D.Static;
    }
    private void DeactivateNoClip()
    {
        // Switch collider's isTrigger bool
        playerCollider.isTrigger = false;
        capsuleCollider.isTrigger = false;
        playerMove.RB.bodyType = RigidbodyType2D.Dynamic;
    }
    #endregion

    #region INSTANT SHROOM THROW
    private void ActivateInstantShroom()
    {
        mushMan.ThrowMultiplier = 30;
    }

    private void DeactivateInstantShroom()
    {
        mushMan.ThrowMultiplier = 8;
    }
    #endregion

    #region INFINITE SHROOMS
    private void ActivateInfiniteShrooms()
    {
        // If enabled, set shroom limit to max value, 'infinite'
        mushMan.MushroomLimit = int.MaxValue;
    }

    private void DeactivateInfiniteShrooms()
    {
        // If enabled, set shroom limit to max value, 'infinite'
        mushMan.MushroomLimit = 3;
    }
    #endregion

    #region SHROOM TIMERS
    private void ActivateShroomTimers()
    {
        mushMan.EnableShroomTimers = true;
    }

    private void DeactivateShroomTimers()
    {
        mushMan.EnableShroomTimers = false;
    }

    private void EnableThrowLine()
    {
        mushMan.ThrowLineOn = true;
    }

    private void DisableThrowLine()
    {
        mushMan.ThrowLineOn = false;
    }
    #endregion 
}
