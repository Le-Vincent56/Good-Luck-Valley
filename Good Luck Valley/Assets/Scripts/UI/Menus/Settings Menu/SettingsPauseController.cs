using GoodLuckValley.UI.Menus.Pause;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Settings
{
    public class SettingsPauseController : MonoBehaviour, IMenuController
    {
        private PauseMenuController pauseMenuController;

        private void Awake()
        {
            pauseMenuController = GetComponentInParent<PauseMenuController>();
        }

        /// <summary>
        /// Leave the Settings menu
        /// </summary>
        public void Back() => pauseMenuController.SetState(pauseMenuController.PAUSED);
    }
}
