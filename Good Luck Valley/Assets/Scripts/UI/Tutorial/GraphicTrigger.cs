using UnityEngine;
using GoodLuckValley.World.Triggers;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Architecture.EventBus;
using GoodLuckValley.UI.Events;

namespace GoodLuckValley.UI.Triggers
{
    public class GraphicTrigger : EnterExitTrigger
    {
        [SerializeField] private int id;
        [SerializeField] private bool raised;

        private void OnTriggerStay2D(Collider2D collision)
        {
            // Exit case - if the event was already raised
            if (raised) return;

            // Exit case - the collision object is not a PlayerController
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Handle the enter function
            OnEnter(controller);
        }

        public override void OnEnter(PlayerController controller)
        {
            // Raise the event to fade in the Graphic
            EventBus<FadeGraphic>.Raise(new FadeGraphic()
            {
                ID = id,
                FadeIn = true
            });

            // Flag the event as raised
            raised = true;
        }

        public override void OnExit(PlayerController controller)
        {
            // Raise the event to fade out the Graphic
            EventBus<FadeGraphic>.Raise(new FadeGraphic()
            {
                ID = id,
                FadeIn = false
            });

            // Flag the event as raised
            raised = false;
        }
    }
}
