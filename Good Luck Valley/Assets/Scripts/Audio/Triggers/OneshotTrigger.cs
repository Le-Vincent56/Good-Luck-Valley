using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    public class OneshotTrigger : EnterExitTrigger
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event oneshotEvent;

        public override void OnEnter(PlayerController controller) => oneshotEvent.Post(gameObject);

        public override void OnExit(PlayerController controller) { }
    }
}
