using GoodLuckValley.UI.Menus.Main;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Controls
{
    public class ControlsMainController : ControlsController, IMenuController
    {
        [Header("References")]
        [SerializeField] private MainMenuController mainMenuController;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Get components
            mainMenuController = GetComponentInParent<MainMenuController>();
        }

        /// <summary>
        /// Leave the Controls Menu
        /// </summary>
        public void Back()
        {
            bool canGoBack = true;

            // Iterate through each Rebind Button
            foreach(RebindButton button in rebindingButtons)
            {
                // Break case - if an invalid rebind is found
                if(!button.ValidRebind)
                {
                    canGoBack = false;
                    break;
                }
            }

            // Exit case - if cannot go back
            if (!canGoBack)
            {
                // Pop the warning text
                warningText.Pop();

                return;
            }

            // Hide the warning text
            warningText.Hide();

            // Save data
            saveHandler.SaveData();

            // Set the settings state
            mainMenuController.SetState(mainMenuController.SETTINGS);
        }
    }
}
