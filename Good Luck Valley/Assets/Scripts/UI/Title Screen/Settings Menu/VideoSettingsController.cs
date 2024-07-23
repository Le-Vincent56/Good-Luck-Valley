namespace GoodLuckValley.UI.TitleScreen.Settings
{
    public class VideoSettingsController : SettingsController
    {
        public void BackToSettings() => controller.SetState(controller.SETTINGS);
        public void ResetSettings() { }
    }
}