using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Audio.Triggers
{
    public class RTPCDistanceTrigger : FloatDistanceBasedTrigger
    {
        [Header("WWise RTPCs")]
        [SerializeField] private AK.Wwise.RTPC rtpc;

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            // Calculate through the parent class
            base.OnTriggerStay2D(collision);

            // Set the global value for the RTPC
            rtpc.SetGlobalValue(currentDistanceValue);
        }
    }
}
