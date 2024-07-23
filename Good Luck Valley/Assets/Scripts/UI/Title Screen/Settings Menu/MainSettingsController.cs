using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings
{
    public class MainSettingsController : SettingsController
    {
        public void EnterAudio() => controller.SetState(controller.AUDIO);
        public void EnterVideo() => controller.SetState(controller.VIDEO);
        public void EnterControls() => controller.SetState(controller.CONTROLS);
        public void ReturnToMain() => controller.SetState(controller.MAIN);
    }
}