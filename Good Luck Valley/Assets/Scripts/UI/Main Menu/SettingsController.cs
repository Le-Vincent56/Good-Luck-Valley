using GoodLuckValley.Audio.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class SettingsController : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private MenuController menuController;
        [SerializeField] private SoundData buttonSFX;
        #endregion

        /// <summary>
        /// Go back to the Main Menu
        /// </summary>
        public void BackButton()
        {
            // Go back to the Main Menu
            menuController.SetState(1);

            PlayButtonSFX();
        }

        public void PlayButtonSFX()
        {
            // Play the button sound effect
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(buttonSFX)
                .WithRandomPitch()
                .Play();
        }
    }
}