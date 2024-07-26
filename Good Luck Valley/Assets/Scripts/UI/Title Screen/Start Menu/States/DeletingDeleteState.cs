using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Start.States
{
    public class DeletingDeleteState : DeleteState
    {
        public DeletingDeleteState(DeleteOverlayController controller, Animator animator, GameObject backgroundObj, GameObject animatedObj) 
            : base(controller, animator, backgroundObj, animatedObj)
        {
        }

        public override async void OnEnter()
        {
            await ShowAnimation(fadeTime);

            animator.CrossFade(DeletingHash, crossFadeDuration);
        }

        public override async void OnExit()
        {
            await Hide(fadeTime);
        }
    }
}