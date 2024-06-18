using GoodLuckValley.Audio.Sound;
using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.UI
{
    public class EndGameScreen : FadePanel
    {
        #region EVENTS
        [Header("Events")]
        [SerializeField] private GameEvent onSoftPause;

        [Header("Fields")]
        [SerializeField] private SoundData buttonSFX;
        #endregion

        public void Start()
        {
            // Hide the UI
            HideUIHard();
        }

        /// <summary>
        /// Return to the Main Menu
        /// </summary>
        public void ReturnToMenu()
        {
            SceneManager.LoadScene("Main Menu");

            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(buttonSFX)
                .WithRandomPitch()
                .Play();
        }

        /// <summary>
        /// Enter the end screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void EnterEndScreen(Component sender, object data)
        {
            // Soft pause
            onSoftPause.Raise(this, null);

            // Show the UI
            ShowUI();
        }
    }
}