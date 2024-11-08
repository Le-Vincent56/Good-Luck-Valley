using GoodLuckValley.Journal.Persistence;
using GoodLuckValley.Patterns.Singletons;
using GoodLuckValley.Player.Control;
using GoodLuckValley.UI.Settings.Audio;
using GoodLuckValley.UI.Settings.Video;
using GoodLuckValley.UI.Settings.Controls;
using GoodLuckValley.World.Interactables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
        void Bind(TData data, bool applyData = true);
    }

    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
    {
        #region FIELDS
        [SerializeField] public GameData selectedData;
        [SerializeField] public SettingsData settingsData;
        [SerializeField] private InputActionAsset inputActions;
        Dictionary<string, GameData> saves;
        FileDataService dataService;
        FileSettingsService settingsService;
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
            settingsService = new FileSettingsService(new JsonSerializer());

            // Initialize the Saves dictionary
            saves = new Dictionary<string, GameData>();
            IEnumerable<string> savesEnum = ListSaves();
            foreach(string save in savesEnum)
            {
                saves[save] = dataService.Load(save);
            }

            // Try to get a settings file
            settingsData = settingsService.Load("PlayerSettings");

            // If none found, create a new settings file
            if (settingsData == null)
                NewSettings();
        }

        /// <summary>
        /// Bind data
        /// </summary>
        public void BindData(bool applyData = true)
        {
            Bind<PlayerSaveHandler, PlayerSaveData>(selectedData.playerSaveData, applyData);
            Bind<JournalSaveHandler, JournalSaveData>(selectedData.journalSaveData, applyData);
            Bind<GlobalDataSaveHandler, GlobalData>(selectedData.globalData, applyData);
            Bind<Collectible, CollectibleSaveData>(selectedData.collectibleSaveDatas, applyData);
            Bind<TutorialSaveHandler, TutorialData>(selectedData.tutorialData, applyData);
        }

        public void BindSettings(bool applyData = true)
        {
            Bind<AudioSaveHandler, AudioData>(settingsData.Audio, applyData);
            Bind<VideoSaveHandler, VideoData>(settingsData.Video, applyData);
            Bind<ControlsSaveHandler, ControlsData>(settingsData.Controls, applyData);
        }

        private void Bind<T, TData>(TData data, bool applyData = true) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
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
                entity.Bind(data, applyData);
            }
        }

        private void Bind<T, TData>(List<TData> datas, bool applyData = true) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
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
                entity.Bind(data, applyData);
            }
        }

        public void NewSettings()
        {
            settingsData = new SettingsData
            {

                Name = "PlayerSettings",
                Audio = new AudioData(),
                Video = new VideoData(),
                Controls = new ControlsData(inputActions)
            };

            SaveSettings();
        }

        /// <summary>
        /// Create a new game
        /// </summary>
        public void NewGame(int slot)
        {
            // Create a base GameData object
            selectedData = new GameData
            {
                Slot = slot,
                Name = $"Slot {Mathf.Clamp(GetSaveCount(), 1, 4)}",
                CurrentLevelName = "Level 1",
                playerSaveData = new PlayerSaveData(),
                journalSaveData = new JournalSaveData(),
                globalData = new GlobalData(),
                tutorialData = new TutorialData(),
                collectibleSaveDatas = new List<CollectibleSaveData>()
            };

            // Save the game
            SaveGame(true);

            //Debug();
        }

        /// <summary>
        /// Save the game
        /// </summary>
        public void SaveGame(bool fromNewGame = false)
        {
            // Set the current level name if not starting a new game (will put the main menu scene
            // if saving from NewGame())
            if (!fromNewGame)
                // Set the current level name
                selectedData.CurrentLevelName = SceneManager.GetActiveScene().name;

            // Set when the data was last updated
            selectedData.LastUpdated = DateTime.Now.ToBinary();

            // Save the data
            dataService.Save(selectedData);

            // Refresh save data
            RefreshSaveData();
        }

        public void SaveSettings() => settingsService.Save(settingsData);

        /// <summary>
        /// Load a game
        /// </summary>
        /// <param name="savename">The name of the GameData to load</param>
        public void LoadGame(string saveName)
        {
            // Load the game data
            selectedData = dataService.Load(saveName);

            // If no Current Level Name is given, default to a given scene
            if(String.IsNullOrWhiteSpace(selectedData.CurrentLevelName))
            {
                selectedData.CurrentLevelName = "Level 1";
            }
        }


        /// <summary>
        /// Select a game
        /// </summary>
        /// <param name="saveName">The name of the GameData to select</param>
        public void SelectGame(string saveName)
        {
            selectedData = dataService.Load(saveName);
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
            // Delete the save file
            dataService.Delete(saveName);

            // Remove the save from the dictionary
            saves.Remove(saveName);
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

        /// <summary>
        /// Refresh the save data
        /// </summary>
        public void RefreshSaveData()
        {
            // Initialize the Saves dictionary
            saves = new Dictionary<string, GameData>();
            IEnumerable<string> savesEnum = ListSaves();
            foreach (string save in savesEnum)
            {
                saves[save] = dataService.Load(save);
            }
        }

        public void Debug() => UnityEngine.Debug.Log(selectedData);
    }
}
