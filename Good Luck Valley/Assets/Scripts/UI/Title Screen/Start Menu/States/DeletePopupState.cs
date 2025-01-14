using UnityEngine;

namespace GoodLuckValley.UI.MainMenu.StartMenu.States
{
    public class DeletePopupState : DeleteState
    {
        private readonly ConfirmationMenu popup;

        public DeletePopupState(DeleteOverlay controller, Animator animator, CanvasGroup canvasGroup, ConfirmationMenu popup) : base(controller, animator, canvasGroup)
        {
            this.popup = popup;
        }

        public override void OnEnter()
        {
            // Fade in the overlay
            Fade(1f, fadeDuration, () =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });

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
