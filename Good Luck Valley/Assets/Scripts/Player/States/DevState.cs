using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class DevState : BaseState
    {
        private DevTools devTools;

        public DevState(PlayerController player, DevTools devTools, Animator animator) : base(player, animator)
        {
            this.devTools = devTools;
        }

        public override void OnEnter()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);

            // Don't allow the player to peek
            player.SetCanPeek(false);
        }

        public override void FixedUpdate()
        {
            if(devTools.NoClip)
            {
                devTools.HandleNoClip();
            }
        }
    }
}