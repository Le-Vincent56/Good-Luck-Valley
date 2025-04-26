using GoodLuckValley.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Cinematics
{
    public class MusicChanger : MonoBehaviour
    {
        [Header("Wwise States")]
        [SerializeField] private List<AK.Wwise.State> statesToSet;

        /// <summary>
        /// Change the music
        /// </summary>
        public void ChangeMusic()
        {
            // Set each state
            MusicManager.Instance.SetStates(statesToSet);

            // Play the music
            MusicManager.Instance.Play();
        }
    }
}
