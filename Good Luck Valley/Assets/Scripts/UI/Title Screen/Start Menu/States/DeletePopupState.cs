using GoodLuckValley.UI.Menus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.TitleScreen.Start.States
{
    public class DeletePopupState : DeleteState
    {
        private readonly ConfirmationPopupMenu popup;
        private readonly MenuCursor cursors;
        private readonly List<Button> buttonsToDisable;

        public DeletePopupState(DeleteOverlayController controller, Animator animator, GameObject backgroundObj, GameObject animatedObj, ConfirmationPopupMenu popup, MenuCursor cursors, List<Button> buttonsToDisable) 
            : base(controller, animator, backgroundObj, animatedObj)
        {
            this.popup = popup;
            this.cursors = cursors;
            this.buttonsToDisable = buttonsToDisable;
        }

        public override async void OnEnter()
        {
            await ShowBackground(fadeTime);

            // Show the cursors
            cursors.ShowCursors();

            // Disable buttons
            foreach(Button button in buttonsToDisable)
            {
                button.interactable = false;
            }

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

            // Fade in the popup
            await popup.Show();

            // Activate the cursors
            cursors.ActivateCursors();
        }

        public override void OnExit()
        {
            // Deactivate the cursors
            cursors.DeactivateCursors();

            // Re-enable buttons
            foreach(Button button in buttonsToDisable)
            {
                button.interactable = true;
            }
        }
    }
}