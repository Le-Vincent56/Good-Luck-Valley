using GoodLuckValley.Architecture.Singletons;
using GoodLuckValley.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
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
        [SerializeField] private GameData selectedData;
        [SerializeField] private Dictionary<string, GameData> saves;
        private IDataService dataService;

        public GameData GameData { get => selectedData; }
        public Dictionary<string, GameData> Saves { get { return saves; } }

        protected override void Awake()
        {
            // Call the parent Awake to set up the Singleton
            base.Awake();

            // Create a File Data Service using a JSON Serializer
            dataService = new FileDataService(new JsonSerializer());
        }

        private void OnEnable()
        {
            SceneLoader.Instance.Cleanup += Cleanup;
            SceneLoader.Instance.Manager.OnSceneGroupLoaded += OnSceneGroupLoaded;
        }

        /// <summary>
        /// Clean up by unsubscribing from events from the Scene Loader
        /// </summary>
        private void Cleanup()
        {
            SceneLoader.Instance.Cleanup -= Cleanup;
            SceneLoader.Instance.Manager.OnSceneGroupLoaded -= OnSceneGroupLoaded;
        }

        private void OnSceneGroupLoaded(int index)
        {
            //Bind<PlayerSaveHandler, PlayerData>(selectedData.PlayerData);
        }

        /// <summary>
        /// Bind a specific type of entity
        /// </summary>
        private void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            // Get the first or default entity of the given type
            T entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();

            // Exit case - no entity was found
            if (entity == null) return;

            // Check if the data is null
            if (data == null) 
                // Create a new instance of the data and set the ID
                data = new TData { ID = entity.ID };

            // Bind the data to the entity
            entity.Bind(data);
        }

        /// <summary>
        /// Bind a list of entities of a given type
        /// </summary>
        private void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            // Get the list of entities of the given type
            T[] entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            // Iterate through each entity
            foreach (T entity in entities)
            {
                // Get the data that matches the entity by comapring their IDs
                TData data = datas.FirstOrDefault(d => d.ID == entity.ID);

                // Check if the data is null
                if (data == null)
                {
                    // If so, create a new instance of the data and set the ID to the
                    // entity's ID
                    data = new TData { ID = entity.ID };

                    // Add the data to the list of datas to be tracked
                    datas.Add(data);
                }

                // Bind the data to the entity
                entity.Bind(data);
            }
        }

        /// <summary>
        /// Create a New Game
        /// </summary>
        public async void NewGame(bool loadGame = false)
        {
            // Create a new Game Data
            selectedData = new GameData
            {
                Slot = 1,
                Name = "New Game",
                SceneGroupIndex = 0,
                LastUpdated = DateTime.Now.ToBinary(),
                PlayerData = new PlayerData()
            };

            SaveGame();

            Bind<PlayerSaveHandler, PlayerData>(selectedData.PlayerData);

            // Exit case - not loading the game
            if (!loadGame) return;

            // Load the Scene Group
            await SceneLoader.Instance.LoadSceneGroup(selectedData.SceneGroupIndex);
        }

        /// <summary>
        /// Save the current state of the Game Data
        /// </summary>
        public void SaveGame()
        {
            // Set when the data was last updated
            selectedData.LastUpdated = DateTime.Now.ToBinary();

            // Save the selected data
            dataService.Save(selectedData);

            // Refresh the save data
            RefreshSaveData();
        }

        /// <summary>
        /// Load a persisted Game Data file
        /// </summary>
        public void LoadGame(string gameName)
        {
            // Set the Game Data by loading in the data from a file
            selectedData = dataService.Load(gameName);

            // Load the scene group at the scene group index
            SceneLoader.Instance.ChangeSceneGroupSystem(selectedData.SceneGroupIndex);
        }

        /// <summary>
        /// Select a persisted Game Data file
        /// </summary>
        public void SelectGame(string saveName) => selectedData = dataService.Load(saveName);

        /// <summary>
        /// Load the most recently updated Game Data file
        /// </summary>
        public void ContinueGame()
        {
            // Load the most recently updated save
            LoadGame(GetMostRecentlyUpdatedSave());
        }

        /// <summary>
        /// Reload the currently loaded Game Data
        /// </summary>
        public void ReloadGame() => LoadGame(selectedData.Name);

        /// <summary>
        /// Delete a persisted Game Data file
        /// </summary>
        public void DeleteGame(string gameName) => dataService.Delete(gameName);

        /// <summary>
        /// List all of the saved files by name
        /// </summary>
        public IEnumerable<string> ListSaves() => dataService.ListSaves();

        /// <summary>
        /// Get the number of current saves
        /// </summary>
        public int GetSaveCount() => ListSaves().Count();

        /// <summary>
        /// Refresh the save data Dictionary
        /// </summary>
        public void RefreshSaveData()
        {
            // Initialize the Saves dictionary
            saves = new Dictionary<string, GameData>();

            // Get the list of saved data
            IEnumerable<string> savesEnum = ListSaves();

            // Iterate through each savedata
            foreach (string save in savesEnum)
            {
                // Set the Dictionary
                saves[save] = dataService.Load(save);
            }
        }

        /// <summary>
        /// Get the most recently updated Save
        /// </summary>
        /// <returns>The name of the most recently updated Save</returns>
        public string GetMostRecentlyUpdatedSave()
        {
            string mostRecentSaveName = null;

            // Loop through every save
            foreach (KeyValuePair<string, GameData> kvp in saves)
            {
                // Set data
                string saveName = kvp.Key;
                GameData gameData = kvp.Value;

                // If there's no recent profile set, set one
                if (mostRecentSaveName == null)
                {
                    mostRecentSaveName = saveName;
                }
                else
                {
                    // Compare the most recent and the selected data
                    DateTime mostRecent = DateTime.FromBinary(saves[mostRecentSaveName].LastUpdated);
                    DateTime newest = DateTime.FromBinary(selectedData.LastUpdated);

                    // If newer, set this name as the most recent save
                    if (newest > mostRecent)
                    {
                        mostRecentSaveName = saveName;
                    }
                }
            }

            return mostRecentSaveName;
        }
    }
}
