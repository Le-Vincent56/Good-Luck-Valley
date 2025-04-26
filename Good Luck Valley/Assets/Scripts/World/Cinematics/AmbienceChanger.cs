using GoodLuckValley.Audio.Ambience;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Cinematics
{
    public class AmbienceChanger : MonoBehaviour
    {
        [Header("Fields")]
        [SerializeField] private List<AK.Wwise.Switch> inSwitches = new List<AK.Wwise.Switch>();

        /// <summary>
        /// Change the ambience
        /// </summary>
        public void ChangeAmbience()
        {
            // Iterate through each In Switch
            foreach (AK.Wwise.Switch @switch in inSwitches)
            {
                // Set the switch value
                @switch.SetValue(AmbienceManager.Instance.gameObject);
            }

            // Start the ambience if it hasn't started already
            AmbienceManager.Instance.StartAmbience();
        }
    }
}
