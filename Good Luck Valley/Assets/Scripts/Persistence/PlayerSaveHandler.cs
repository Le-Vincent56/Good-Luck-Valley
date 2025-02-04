using GoodLuckValley.Scenes;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    public class PlayerSaveHandler : MonoBehaviour, IBind<PlayerData>
    {
        [SerializeField] private PlayerData playerData;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        // Update is called once per frame
        private void Update()
        {
            // Update save data
            UpdateSaveData();
        }

        /// <summary>
        /// Update the Player save data
        /// </summary>
        public void UpdateSaveData()
        {
            // Save transform data
            playerData.Position = transform.position;
            playerData.LevelIndex = SceneLoader.Instance.Manager.CurrentIndex;
        }

        /// <summary>
        /// Bind Player data for persistence
        /// </summary>
        public void Bind(PlayerData playerData)
        {
            // Bind the data
            this.playerData = playerData;
            this.playerData.ID = ID;

            // Exit case - if debugging
            if (SaveLoadSystem.Instance.Debug) return;

            // Set the player position
            transform.position = playerData.Position;
        }
    }
}
