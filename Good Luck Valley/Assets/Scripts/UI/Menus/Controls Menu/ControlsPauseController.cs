using GoodLuckValley.UI.Menus.Pause;

namespace GoodLuckValley.UI.Menus.Controls
{
    public class ControlsPauseController : ControlsController, IMenuController
    {
        private PauseMenuController pauseMenuController;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Get components
            pauseMenuController = GetComponentInParent<PauseMenuController>();
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
            pauseMenuController.SetState(pauseMenuController.SETTINGS);
        }
    }
}
