using GoodLuckValley.Patterns;
using GoodLuckValley.Player;
using GoodLuckValley.Player.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Persistence
{
    [Serializable] public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        public PlayerSaveData playerSaveData;
    }

    public interface ISaveable
    {
        SerializableGuid ID { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid ID { get; set; }
        void Bind(TData data);
    }

    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
    {
        #region FIELDS
        [SerializeField] public GameData gameData;
        IDataService dataService;
        #endregion
        
        protected override void Awake()
        {
            // Initialize the Persistent Singleton
            base.Awake();

            // Initialize the data service
            dataService = new FileDataService(new JsonSerializer());
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Guard clause to not load data in
            if (scene.name == "Menu") return;

            // Bind player data
            Bind<PlayerController, PlayerSaveData>(gameData.playerSaveData);
        }

        private void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            // Find the entity of the given type
            T entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();

            // Check if an entity is found
            if (entity != null)
            {
                // Check if there's any data
                if (data == null)
                {
                    // if not, create new data with an identical ID
                    data = new TData { ID = entity.ID };
                }

                // Bind the data
                entity.Bind(data);
            }
        }

        private void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            // Find all entities of a certain type
            T[] entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            // Loop through each entity
            foreach(T entity in entities)
            {
                // Find a data that matches the entity's id
                TData data = datas.FirstOrDefault(d => d.ID == entity.ID);

                // If there's no data that matches the entity's id,
                // create one and add it to the list
                if(data == null)
                {
                    data = new TData { ID = entity.ID };
                    datas.Add(data);
                }

                // Bind the entity with its data
                entity.Bind(data);
            }
        }

        /// <summary>
        /// Create a new game
        /// </summary>
        public void NewGame()
        {
            // Create a base GameData object
            gameData = new GameData
            {
                Name = "New Game",
                CurrentLevelName = "SampleScene",
            };
            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        /// <summary>
        /// Save the game
        /// </summary>
        public void SaveGame()
        {
            dataService.Save(gameData);
        }

        /// <summary>
        /// Load a game
        /// </summary>
        /// <param name="savename">The name of the GameData to load</param>
        public void LoadGame(string savename)
        {
            // Load the game data
            gameData = dataService.Load(savename);

            // If no Current Level Name is given, default to a given scene
            if(String.IsNullOrWhiteSpace(gameData.CurrentLevelName))
            {
                gameData.CurrentLevelName = "SampleScene";
            }

            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        /// <summary>
        /// Reload the current game
        /// </summary>
        public void ReloadGame()
        {
            LoadGame(gameData.Name);
        }

        /// <summary>
        /// Delete a game
        /// </summary>
        /// <param name="saveName">The name of the GameData to delete</param>
        public void DeleteGame(string saveName)
        {
            dataService.Delete(saveName);
        }
    }
}
