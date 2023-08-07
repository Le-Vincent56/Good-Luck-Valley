using UnityEngine;
using HiveMind.Audio;
using HiveMind.Movement;
using HiveMind.Events;
using HiveMind.SaveData;

namespace HiveMind.Menus
{
    public class Settings : MonoBehaviour, ISettingsData
    {
        #region REFERENCES
        [SerializeField] private SettingsScriptableObj settingsEvent;
        private MenusManager menusMan;
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
        public float Brightness { get { return brightness; } set { brightness = value; } }
        public bool SubtitlesEnabled { get { return subtitlesEnabled; } set { subtitlesEnabled = value; } }
        public int ResOption { get { return resOption; } set { resOption = value; } }
        public bool IsFullscreen { get { return isFullscreen; } set { isFullscreen = value; } }
        public Vector2 Resolution { get { return resolution; } set { resolution = value; } }
        public float MasterVolume { get { return masterVolume; } set { masterVolume = value; } }
        public float MusicVolume { get { return musicVolume; } set { musicVolume = value; } }
        public float SFXVolume { get { return sfxVolume; } set { sfxVolume = value; } }
        public float AmbientVolume { get { return ambientVolume; } set { ambientVolume = value; } }
        public float VoicesVolume { get { return voicesVolume; } set { voicesVolume = value; } }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            menusMan = GameObject.Find("MenusManager").GetComponent<MenusManager>();
            brightnessSquare = GameObject.Find("Fade");
            if (menusMan.CurrentScene > 5)
            {
                playerMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
                playerCollider = playerMove.GetComponentInParent<BoxCollider2D>();
                capsuleCollider = playerMove.GetComponentInParent<CapsuleCollider2D>();
                UpdateSettings();
            }
        }

        void Update()
        {
            // Update settings
            noClipOn = settingsEvent.GetNoClipActive();
            instantThrowOn = settingsEvent.GetInstantThrowActive();
            infiniteShroomsOn = settingsEvent.GetInfiniteShroomsActive();
            shroomDurationOn = settingsEvent.GetShroomTimersActive();
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
            settingsEvent.SetThrowMultiplier(30);
            settingsEvent.UpdateShroomSettings();
        }

        private void DeactivateInstantShroom()
        {
            settingsEvent.SetThrowMultiplier(8);
            settingsEvent.UpdateShroomSettings();
        }
        #endregion

        #region INFINITE SHROOMS
        private void ActivateInfiniteShrooms()
        {
            // If enabled, set shroom limit to max value, 'infinite'
            settingsEvent.SetShroomLimit(int.MaxValue);
            settingsEvent.UpdateShroomSettings();
        }

        private void DeactivateInfiniteShrooms()
        {
            // If enabled, set shroom limit to max value, 'infinite'
            settingsEvent.SetShroomLimit(3);
            settingsEvent.UpdateShroomSettings();
        }
        #endregion

        #region SHROOM TIMERS
        private void ActivateShroomTimers()
        {
            settingsEvent.SetShroomTimersActive(true);
            settingsEvent.UpdateShroomSettings();
        }

        private void DeactivateShroomTimers()
        {
            settingsEvent.SetShroomTimersActive(false);
            settingsEvent.UpdateShroomSettings();
        }
        #endregion

        #region THROW LINE
        private void EnableThrowLine()
        {
            settingsEvent.SetThrowLineActive(true);
            settingsEvent.UpdateShroomSettings();
        }

        private void DisableThrowLine()
        {
            settingsEvent.SetThrowLineActive(false);
            settingsEvent.UpdateShroomSettings();
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
            if (menusMan != null && menusMan.CurrentScene > 5)
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
}
