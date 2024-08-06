using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings
{
    public class MainSettingsController : SettingsController
    {
        private const int stateNum = 3;

        public void EnterAudio() => controller.SetState(controller.AUDIO);
        public void EnterVideo() => controller.SetState(controller.VIDEO);
        public void EnterControls() => controller.SetState(controller.CONTROLS);

        public void BackInput(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not int) return;

            // Cast and compare data
            if ((int)data == stateNum)
            {
                ReturnToMain();
            }
        }

        public void ReturnToMain() => controller.SetState(controller.MAIN);
    }
}