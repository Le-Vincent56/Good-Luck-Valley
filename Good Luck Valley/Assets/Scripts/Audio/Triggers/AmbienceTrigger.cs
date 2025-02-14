using GoodLuckValley.Audio.Ambience;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Audio.Triggers
{
    public class AmbienceTrigger : BaseTrigger
    {
        [Header("Fields")]
        [SerializeField] private AK.Wwise.Switch forestSwitch;
        [SerializeField] private bool startAmbience;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - there's no Player Controller on the collider
            if (!collision.TryGetComponent(out PlayerController controller)) return;
               
            // Set the switch value
            forestSwitch.SetValue(AmbienceManager.Instance.gameObject);

            // Check if starting ambience
            if (startAmbience) 
                // Start ambience
                AmbienceManager.Instance.StartAmbience();
        }
    }
}
