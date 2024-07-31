using GoodLuckValley.Persistence;
using UnityEngine;

namespace GoodLuckValley.UI.Settings.Video
{
    public class VideoSaveHandler : MonoBehaviour, IBind<VideoData>
    {
        [SerializeField] private VideoData data;
        [SerializeField] private BrightnessSlider brightnessSlider;
        [field: SerializeField] public SerializableGuid ID { get; set; } = new SerializableGuid(1279892122, 1270737158, 308844696, 3286357241);

        public void SaveData()
        {
            // Save brightness data
            data.brightness = brightnessSlider.GetBrightness();

            SaveLoadSystem.Instance.SaveSettings();
        }

        public void Bind(VideoData data, bool applyData = true)
        {
            this.data = data;
            this.data.ID = data.ID;

            // Set slider data
            brightnessSlider.LoadBrightness(data.brightness);
        }
    }
}