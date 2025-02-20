using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Persistence;
using GoodLuckValley.Scenes;
using UnityEngine;

namespace GoodLuckValley.Player.Persistence
{
    public class PlayerSaveHandler : MonoBehaviour, IBind<PlayerData>
    {
        private SaveLoadSystem saveLoadSystem;
        private SceneLoader sceneLoader;

        [SerializeField] private PlayerData playerData;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        private void Awake()
        {
            // Get services
            saveLoadSystem = ServiceLocator.Global.Get<SaveLoadSystem>();
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
        }

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
            playerData.LevelIndex = sceneLoader.Manager.CurrentIndex;
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
            if (saveLoadSystem.Debug) return;

            // Set the player position
            transform.position = playerData.Position;
        }
    }
}
