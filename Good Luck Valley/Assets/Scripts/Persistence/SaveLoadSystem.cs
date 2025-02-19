using GoodLuckValley.Architecture.Singletons;
using GoodLuckValley.Cameras.Persistence;
using GoodLuckValley.Interactables;
using GoodLuckValley.Player.Persistence;
using GoodLuckValley.Scenes;
using GoodLuckValley.UI.Journal.Persistence;
using GoodLuckValley.UI.Menus.Persistence;
using GoodLuckValley.World.Cinematics.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private bool debug;
        [SerializeField] private GameData selectedData;
        [SerializeField] private SettingsData settingsData;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Dictionary<string, GameData> saves;
        private FileGameDataService gameDataService;
        private FileSettingsDataService settingsDataService;

        public Action PrepareDataBind = delegate { };
        public Action<int> DataBinded = delegate { };
        public Action SettingsBinded = delegate { };
        public Action Release = delegate { };

        public bool Debug { get => debug; }
        public GameData GameData { get => selectedData; }
        public Dictionary<string, GameData> Saves { get { return saves; } }

        protected override void Awake()
        {
            // Call the parent Awake to set up the Singleton
            base.Awake();

            // Create a File Data Service using a JSON Serializer
            gameDataService = new FileGameDataService(new JsonSerializer());
            settingsDataService = new FileSettingsDataService(new JsonSerializer());

            // Initialize the Save Slots Dictionary
            RefreshSaveData();
            
            // Try to get the settings file
            settingsData = settingsDataService.Load("Settings");

            // Check if the settings data was not set
            if (settingsData == null)
                // Create a new settings file
                NewSettings();
        }

        private void OnEnable()
        {
            SceneLoader.Instance.Release += Cleanup;
            SceneLoader.Instance.Manager.OnSceneGroupLoaded += OnSceneGroupLoaded;
        }

        private void OnDestroy()
        {
            // Release all events
            Release.Invoke();
        }

        /// <summary>
        /// Clean up by unsubscribing from events from the Scene Loader
        /// </summary>
        private void Cleanup()
        {
            SceneLoader.Instance.Release -= Cleanup;
            SceneLoader.Instance.Manager.OnSceneGroupLoaded -= OnSceneGroupLoaded;
        }

        private void OnSceneGroupLoaded(int index)
        {
            // Bind Settings
            Bind<AudioSaveHandler, AudioData>(settingsData.Audio);
            Bind<VideoSaveHandler, VideoData>(settingsData.Video);
            Bind<ControlsSaveHandler, ControlsData>(settingsData.Controls);

            // Notify settings binded
            SettingsBinded.Invoke();

            // Exit case - no selected data
            if (selectedData == null) return;

            // Prepare for data binding
            PrepareDataBind.Invoke();

            // Bind Data
            Bind<PlayerSaveHandler, PlayerData>(selectedData.PlayerData);
            Bind<JournalSaveHandler, JournalData>(selectedData.JournalData);
            Bind<Collectible, CollectibleSaveData>(selectedData.CollectibleDatas);
            Bind<CameraSaveHandler, CameraData>(selectedData.CameraDatas);
            Bind<TimelineSaveHandler, TimelineData>(selectedData.TimelineDatas);

            // Notify data binded
            DataBinded.Invoke(index);
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
        /// Create a new settings file
        /// </summary>
        private void NewSettings()
        {
            settingsData = new SettingsData
            {
                Name = "Settings",
                Audio = new AudioData(),
                Video = new VideoData(),
                Controls = new ControlsData(inputActions)
            };

            SaveSettings();
        }

        /// <summary>
        /// Save the game settings
        /// </summary>
        public void SaveSettings() => settingsDataService.Save(settingsData); 

        /// <summary>
        /// Create a New Game
        /// </summary>
        public void NewGame(int slot, bool loadGame = false)
        {
            // Create a new Game Data
            selectedData = new GameData
            {
                Slot = slot,
                Name = $"New Game {slot}",
                LastUpdated = DateTime.Now.ToBinary(),
                PlayerData = new PlayerData(),
                CameraDatas = new List<CameraData>(),
                JournalData = new JournalData(),
                CollectibleDatas = new List<CollectibleSaveData>(),
                TimelineDatas = new List<TimelineData>()
            };

            SaveGame();

            // Exit case - not loading the game
            if (!loadGame) return;

            // Load the Scene Group
            SceneLoader.Instance.ChangeSceneGroupSystem(selectedData.PlayerData.LevelIndex);
        }

        /// <summary>
        /// Save the current state of the Game Data
        /// </summary>
        public void SaveGame()
        {
            // Set when the data was last updated
            selectedData.LastUpdated = DateTime.Now.ToBinary();

            // Save the selected data
            gameDataService.Save(selectedData);

            // Refresh the save data
            RefreshSaveData();
        }

        /// <summary>
        /// Load a persisted Game Data file
        /// </summary>
        public void LoadGame(string gameName)
        {
            // Set the Game Data by loading in the data from a file
            selectedData = gameDataService.Load(gameName);

            // Load the scene group at the scene group index
            SceneLoader.Instance.ChangeSceneGroupSystem(selectedData.PlayerData.LevelIndex);
        }

        /// <summary>
        /// Select a persisted Game Data file
        /// </summary>
        public void SelectGame(string saveName) => selectedData = gameDataService.Load(saveName);

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
        public void DeleteGame(string gameName) => gameDataService.Delete(gameName);

        /// <summary>
        /// List all of the saved files by name
        /// </summary>
        public IEnumerable<string> ListSaves() => gameDataService.ListSaves();

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
                saves[save] = gameDataService.Load(save);
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
