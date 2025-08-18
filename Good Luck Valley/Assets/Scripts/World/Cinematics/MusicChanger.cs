using GoodLuckValley.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Cinematics
{
    public class MusicChanger : MonoBehaviour
    {
        [Header("Wwise States")]
        [SerializeField] private List<AK.Wwise.State> statesToSet;
        [SerializeField] private bool restartOnChange = true;

        /// <summary>
        /// Change the music
        /// </summary>
        public void ChangeMusic()
        {
            // Set each state
            MusicManager.Instance.SetStates(statesToSet);

            // Restart the music if required, otherwise play it normally
            if (restartOnChange) MusicManager.Instance.Restart();
            else MusicManager.Instance.Play();
        }
    }
}
