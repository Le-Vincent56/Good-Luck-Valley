using UnityEngine;

namespace GoodLuckValley.UI.Menus.Pause
{
    public class PauseMenuSubcontroller : MonoBehaviour, IMenuController
    {
        private PauseMenuController pauseMenuController;

        private void Awake()
        {
            // Get components
            pauseMenuController = GetComponentInParent<PauseMenuController>();
        }

        /// <summary>
        /// Leave the Pause menu
        /// </summary>
        public void Back()
        {
            // Set the unpaused state
            pauseMenuController.SetState(pauseMenuController.UNPAUSED);
        }
    }
}
