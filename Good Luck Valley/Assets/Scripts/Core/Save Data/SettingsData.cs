using UnityEngine;

namespace HiveMind.SaveData
{
    [System.Serializable]
    public class SettingsData
    {
        #region FIELDS
        // Accessibility
        public bool throwIndicatorShown;
        public bool noClipOn;
        public bool instantThrowOn;
        public bool infiniteShroomsOn;
        public bool shroomDurationOn;

        // Display
        public float brightness;
        public bool subtitlesEnabled;
        public Vector2 resolution;
        public bool isFullscreen;
        public int resOption;

        // Audio
        public float masterVolume;
        public float musicVolume;
        public float SFXVolume;
        public float ambientVolume;
        public float voicesVolume;

        // Controls
        #endregion

        // Constructor will have default values for when the game starts when there's no data to load
        public SettingsData()
        {
            throwIndicatorShown = true;
            noClipOn = false;
            instantThrowOn = false;
            infiniteShroomsOn = false;
            shroomDurationOn = true;

            // Display
            brightness = 95f;
            subtitlesEnabled = false;
            resolution = new Vector2(1920, 1080);
            isFullscreen = true;
            resOption = 1;

            // Audio
            masterVolume = 40;
            musicVolume = 30;
            SFXVolume = 30;
            ambientVolume = 30;
            voicesVolume = 40;
        }
    }
}
