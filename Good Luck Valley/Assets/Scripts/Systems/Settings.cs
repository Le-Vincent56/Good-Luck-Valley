using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Settings : MonoBehaviour, ISettingsData
{
    #region REFERENCES
    private MenusManager menusMan;
    [SerializeField] private MushroomManager mushMan;
    [SerializeField] private PlayerMovement playerMove;
    private GameObject brightnessSquare;
    #endregion

    #region FIELDS
    // Accessibility Settings
    private static bool throwIndicatorShown;
    private static bool noClipOn;
    private static bool instantThrowOn;
    private static bool infiniteShroomsOn;
    private static bool shroomDurationOn;

    // Display settings
    private static float brightness;
    private static bool subtitlesEnabled;
    private static int resOption;
    private static Vector2 resolution;
    private static bool isFullscreen;

    // Audio settings
    private static float masterVolume = 1.0f;
    private static float musicVolume = 1.0f;
    private static float sfxVolume = 1.0f;
    private static float ambientVolume = 1.0f;
    private static float voicesVolume;

    private BoxCollider2D playerCollider;
    private CapsuleCollider2D capsuleCollider;
    #endregion

    #region PROPERTIES
    public bool ThrowIndicatorShown { get { return throwIndicatorShown; } set { throwIndicatorShown = value; } }
    public bool NoClipOn { get { return noClipOn; } set { noClipOn = value; } }
    public bool InstantThrowOn { get { return instantThrowOn; } set { instantThrowOn = value; } }
    public bool InfiniteShroomsOn { get { return infiniteShroomsOn; } set { infiniteShroomsOn = value; } }
    public bool ShroomDurationOn { get { return shroomDurationOn; } set { shroomDurationOn = value; } }
    public float Brightness { get { return brightness; } set {  brightness = value; } }
    public bool SubtitlesEnabled { get {  return subtitlesEnabled; } set {  subtitlesEnabled = value; } }
    public int ResOption { get { return resOption; } set { resOption = value; } }
    public bool IsFullscreen { get { return isFullscreen; } set { isFullscreen = value; } }
    public Vector2 Resolution {  get { return resolution; } set {  resolution = value; } }
    public float MasterVolume { get { return masterVolume; } set {  masterVolume = value; } }
    public float MusicVolume { get { return musicVolume; } set {  musicVolume = value; } }
    public float SFXVolume { get { return sfxVolume; } set { sfxVolume = value; } }
    public float AmbientVolume { get { return ambientVolume; } set {  ambientVolume = value; } }
    public float VoicesVolume { get { return voicesVolume; } set { voicesVolume = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        menusMan = GameObject.Find("MenusManager").GetComponent<MenusManager>();
        brightnessSquare = GameObject.Find("Fade");
        if (menusMan.CurrentScene > 5)
        {
            mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
            playerMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
            playerCollider = playerMove.GetComponentInParent<BoxCollider2D>();
            capsuleCollider = playerMove.GetComponentInParent<CapsuleCollider2D>();
            UpdateSettings();
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
    #endregion

    #region THROW LINE
    private void EnableThrowLine()
    {
        mushMan.ThrowLineOn = true;
    }

    private void DisableThrowLine()
    {
        mushMan.ThrowLineOn = false;
    }
    #endregion

    // DATA HANDLING
    #region DATA HANDLING
    public void LoadData(SettingsData data)
    {

        // Load accessibility settings
        #region ACCESSIBILITY
        throwIndicatorShown = data.throwIndicatorShown;
        infiniteShroomsOn = data.infiniteShroomsOn;
        shroomDurationOn = data.shroomDurationOn;
        instantThrowOn = data.instantThrowOn;
        noClipOn = data.noClipOn;
        #endregion

        // Load display settings
        #region DISPLAY
        brightness = data.brightness;
        if (data.resolution.x != 12 && data.resolution.y != 34)
        {
            Screen.SetResolution((int)data.resolution.x, (int)data.resolution.y, data.isFullscreen);
        }
        else
        {
            Screen.SetResolution(1920, 1080, true);
        }
        Screen.fullScreen = data.isFullscreen;
        subtitlesEnabled = data.subtitlesEnabled;
        isFullscreen = data.isFullscreen;
        resolution = data.resolution;
        resOption = data.resOption;
        #endregion

        // Load audio settings
        #region AUDIO
        musicVolume = data.musicVolume;
        masterVolume = data.masterVolume;
        ambientVolume = data.ambientVolume;
        SFXVolume = data.SFXVolume;
        voicesVolume = data.voicesVolume;
        #endregion

        UpdateSettings();
    }

    public void SaveData(SettingsData data)
    {

        // Save accessibility settings values
        #region ACCESSIBILITY
        data.throwIndicatorShown = throwIndicatorShown;
        data.infiniteShroomsOn = infiniteShroomsOn;
        data.shroomDurationOn = shroomDurationOn;
        data.instantThrowOn = instantThrowOn;
        data.noClipOn = noClipOn;
        #endregion

        // Save display settings values
        #region DISPLAY
        data.brightness = brightness;
        data.subtitlesEnabled = subtitlesEnabled;
        data.isFullscreen = isFullscreen;
        data.resolution = resolution;
        data.resOption = resOption;
        #endregion

        // Save audio settings values
        #region AUDIO
        data.masterVolume = masterVolume;
        data.ambientVolume = ambientVolume;
        data.voicesVolume = voicesVolume;
        data.SFXVolume = SFXVolume;
        data.musicVolume = musicVolume;
        #endregion

        UpdateSettings();
    }
    #endregion

    #region UPDATING SETTINGS
    public void UpdateSettings()
    {
        if (menusMan!= null && menusMan.CurrentScene > 5)
        {
            #region ACCESSIBILITY SETTINGS
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
            #endregion

            #region DISPLAY SETTINGS
            float transparencyValue = 1 - (brightness / 100);
            brightnessSquare.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, transparencyValue);
            #endregion
        }

        #region SOUND SETTINGS
        if (AudioManager.Instance)
        {
            AudioManager.Instance.SetMasterVolume(masterVolume / 100);
            AudioManager.Instance.SetMusicVolume(musicVolume / 100);
            AudioManager.Instance.SetAmbienceVolume(ambientVolume / 100);
            AudioManager.Instance.SetSFXVolume(SFXVolume / 100);
        }
        #endregion
    }
    #endregion
}
