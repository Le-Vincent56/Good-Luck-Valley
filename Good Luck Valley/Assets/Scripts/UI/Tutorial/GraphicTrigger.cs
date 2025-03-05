using UnityEngine;
using GoodLuckValley.World.Triggers;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;

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

        /// <summary>
        /// Flag the Graphic Fader as raised
        /// </summary>
        public void Raise() => raised = true;

        public override void OnEnter(PlayerController controller)
        {
            // Raise the event to fade in the Graphic
            EventBus<FadeTutorialCanvas>.Raise(new FadeTutorialCanvas()
            {
                ID = id,
                FadeIn = true,
                Trigger = this,
            });
        }

        public override void OnExit(PlayerController controller)
        {
            // Raise the event to fade out the Graphic
            EventBus<FadeTutorialCanvas>.Raise(new FadeTutorialCanvas()
            {
                ID = id,
                FadeIn = false,
                Trigger = this,
            });

            // Flag the event as raised
            raised = false;
        }
    }
}
