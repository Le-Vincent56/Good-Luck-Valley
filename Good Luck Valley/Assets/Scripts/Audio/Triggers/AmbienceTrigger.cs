using GoodLuckValley.Audio.Ambience;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.World.Triggers;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Audio.Triggers
{
    public class AmbienceTrigger : BaseTrigger
    {
        [Header("Fields")]
        [SerializeField] private List<AK.Wwise.Switch> inSwitches = new List<AK.Wwise.Switch>();
        [SerializeField] private bool changeOnExit;
        [SerializeField] private List<AK.Wwise.Switch> outSwitches = new List<AK.Wwise.Switch>();
        [SerializeField] private bool startAmbience;

        private bool inTrigger = true;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - there's no Player Controller on the collider
            if (!collision.TryGetComponent(out PlayerController controller)) return;
             
            // Iterate through each In Switch
            foreach(AK.Wwise.Switch @switch in inSwitches)
            {
                // Set the switch value
                @switch.SetValue(AmbienceManager.Instance.gameObject);
            }

            // Check if starting ambience
            if (startAmbience) 
                // Start ambience
                AmbienceManager.Instance.StartAmbience();

            // Set to inside trigger
            inTrigger = true;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // Exit case - already in the Trigger
            if (inTrigger) return;

            // Exit case - there's no Player Controller on the collider
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Iterate through each In Switch
            foreach (AK.Wwise.Switch @switch in inSwitches)
            {
                // Set the switch value
                @switch.SetValue(AmbienceManager.Instance.gameObject);
            }

            // Set to inside trigger
            inTrigger = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - not changing switches on exit
            if(!changeOnExit) return;

            // Exit case - there's no Player Controller on the collider
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Iterate through each Out switch
            foreach (AK.Wwise.Switch @switch in outSwitches)
            {
                // Set the switch value
                @switch.SetValue(AmbienceManager.Instance.gameObject);
            }

            // Set to outside trigger
            inTrigger = false;
        }
    }
}
