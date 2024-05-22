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
        [SerializeField] public GameData selectedData;
        Dictionary<string, GameData> saves;
        IDataService dataService;
        #endregion

        #region PROPERTIES
        public Dictionary<string, GameData> Saves { get { return saves; } }
        #endregion

        protected override void Awake()
        {
            // Initialize the Persistent Singleton
            base.Awake();

            // Initialize the data service
            dataService = new FileDataService(new JsonSerializer());

            // Initialize the Saves dictionary
            saves = new Dictionary<string, GameData>();
            IEnumerable<string> savesEnum = ListSaves();
            foreach(string save in savesEnum)
            {
                saves[save] = dataService.Load(save);
            }
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Guard clause to not load data in
            if (scene.name == "Menu") return;

            // Bind player data
            Bind<PlayerController, PlayerSaveData>(selectedData.playerSaveData);
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
            selectedData = new GameData
            {
                Name = $"Save {Mathf.Clamp(GetSaveCount(), 1, 4)}",
                CurrentLevelName = "SampleScene",
                playerSaveData = new PlayerSaveData()
            };
            SceneManager.LoadScene(selectedData.CurrentLevelName);
        }

        /// <summary>
        /// Save the game
        /// </summary>
        public void SaveGame()
        {
            // Set when the data was last updated
            selectedData.LastUpdated = DateTime.Now.ToBinary();

            // Save the data
            dataService.Save(selectedData);
        }

        /// <summary>
        /// Load a game
        /// </summary>
        /// <param name="savename">The name of the GameData to load</param>
        public void LoadGame(string savename)
        {
            // Load the game data
            selectedData = dataService.Load(savename);

            // If no Current Level Name is given, default to a given scene
            if(String.IsNullOrWhiteSpace(selectedData.CurrentLevelName))
            {
                selectedData.CurrentLevelName = "SampleScene";
            }

            SceneManager.LoadScene(selectedData.CurrentLevelName);
        }

        /// <summary>
        /// Continue the most recent game
        /// </summary>
        public void ContinueGame()
        {
            // Load the most recently updated save
            LoadGame(GetMostRecentlyUpdatedSave());
        }

        /// <summary>
        /// Reload the current game
        /// </summary>
        public void ReloadGame()
        {
            LoadGame(selectedData.Name);
        }

        /// <summary>
        /// Delete a game
        /// </summary>
        /// <param name="saveName">The name of the GameData to delete</param>
        public void DeleteGame(string saveName)
        {
            dataService.Delete(saveName);
        }

        /// <summary>
        /// Get a IEnumerable<string> of save file names
        /// </summary>
        /// <returns>An IEnumerable<string> of save file names</returns>
        public IEnumerable<string> ListSaves()
        {
            return dataService.ListSaves();
        }

        /// <summary>
        /// Get the number of current saves
        /// </summary>
        /// <returns>The number of current saves</returns>
        public int GetSaveCount()
        {
            return dataService.ListSaves().Count();
        }

        /// <summary>
        /// Get the most recently updated Save
        /// </summary>
        /// <returns>The name of the most recently updated Save</returns>
        public string GetMostRecentlyUpdatedSave()
        {
            string mostRecentSaveName = null;

            // Loop through every save
            foreach(KeyValuePair<string, GameData> kvp in saves)
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
                    if(newest > mostRecent)
                    {
                        mostRecentSaveName = saveName;
                    }
                }
            }

            return mostRecentSaveName;
        }
    }
}
