using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Start.States
{
    public class DeletePopupState : DeleteState
    {
        private readonly ConfirmationMenu popup;

        public DeletePopupState(DeleteOverlay controller, Animator animator, CanvasGroup canvasGroup, ConfirmationMenu popup, Image contrastOverlay) 
            : base(controller, animator, canvasGroup, contrastOverlay)
        {
            this.popup = popup;
        }

        public override void OnEnter()
        {
            // Fade in the overlay
            FadeGroup(1f, fadeDuration, () =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });

            FadeOverlay(overlayOpacity, fadeDuration);

            // Activate the menu
            popup.ActivateMenu("Deleting a save is permanent. Are you sure?",
                () =>
                {
                    // Delete the selected data
                    controller.DeleteData();

                    // Set the state to deleting
                    controller.SetState(controller.DELETING);
                },
                () =>
                {
                    // Set the idle state
                    controller.SetState(controller.IDLE);
                }
            );
        }
    }
}
