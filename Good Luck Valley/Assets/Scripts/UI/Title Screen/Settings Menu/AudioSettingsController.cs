namespace GoodLuckValley.UI.TitleScreen.Settings
{
    public class AudioSettingsController : SettingsController
    {
        public void BackToSettings() => controller.SetState(controller.SETTINGS);
        public void ResetSettings() { }
    }
}