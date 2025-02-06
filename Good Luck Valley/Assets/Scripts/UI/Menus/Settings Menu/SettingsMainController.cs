using GoodLuckValley.UI.Menus.Main;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Settings
{
    public class SettingsMainController : MonoBehaviour, IMenuController
    {
        private MainMenuController mainMenuController;

        private void Awake()
        {
            // Get components
            mainMenuController = GetComponentInParent<MainMenuController>();
        }

        /// <summary>
        /// Leave the Settings menu
        /// </summary>
        public void Back() => mainMenuController.SetState(mainMenuController.INITIAL);
    }
}
