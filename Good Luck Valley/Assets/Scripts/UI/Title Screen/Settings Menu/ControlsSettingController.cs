namespace GoodLuckValley.UI.TitleScreen.Settings
{
    public class ControlsSettingController : SettingsController
    {
        public void BackToSettings() => controller.SetState(controller.SETTINGS);
        public void ResetSettings() { }
    }
}