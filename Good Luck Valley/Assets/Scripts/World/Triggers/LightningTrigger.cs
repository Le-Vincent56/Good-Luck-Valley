using GoodLuckValley.Events;
using GoodLuckValley.Events.World;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class LightningTrigger : EnterExitTrigger
    {
        [Header("Fields")]
        [SerializeField] private float intensity;
        [SerializeField] private float duration;

        public override void OnEnter(PlayerController controller)
        {
            EventBus<StrikeLightning>.Raise(new StrikeLightning()
            {
                Intensity = intensity,
                Duration = duration
            });
        }

        public override void OnExit(PlayerController controller) { }
    }
}
