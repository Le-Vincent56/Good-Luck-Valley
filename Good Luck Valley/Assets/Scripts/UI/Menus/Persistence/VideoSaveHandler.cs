using GoodLuckValley.Persistence;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Persistence
{
    public class VideoSaveHandler : MonoBehaviour, IBind<VideoData>
    {
        [SerializeField] private VideoData data;
        [field: SerializeField] public SerializableGuid ID { get; set; } = new SerializableGuid(1279892122, 1270737158, 308844696, 3286357241);

        /// <summary>
        /// Save the Video settings data
        /// </summary>
        public void SaveData()
        {
            // TODO: Set data

            // Save the settings
            SaveLoadSystem.Instance.SaveSettings();
        }

        /// <summary>
        /// Bind the Video settings data
        /// </summary>
        public void Bind(VideoData data)
        {
            this.data = data;
            this.data.ID = ID;

            // TODO: Set data
        }
    }
}
