using GoodLuckValley.Persistence;
using UnityEngine;

namespace GoodLuckValley.Player.Control
{
    public class PlayerSaveHandler : MonoBehaviour, IBind<PlayerSaveData>
    {
        [SerializeField] private PlayerSaveData data;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        // Update is called once per frame
        void Update()
        {
            // Update save data
            UpdateSaveData();
        }

        public void UpdateSaveData()
        {
            // Save transform data
            data.position = transform.position;
        }

        /// <summary>
        /// Force a save update
        /// </summary>
        public void ForceUpdate() => UpdateSaveData();

        /// <summary>
        /// Bind Player data for persistence
        /// </summary>
        /// <param name="data"></param>
        public void Bind(PlayerSaveData data, bool applyData = true)
        {
            this.data = data;
            this.data.ID = ID;

            if (applyData)
                transform.position = data.position;
        }
    }
}