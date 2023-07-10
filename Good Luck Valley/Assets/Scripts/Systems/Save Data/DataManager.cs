using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class DataManager : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private GameData gameData;
    private SettingsData settingsData;
    [SerializeField] private List<IData> dataObjects;
    [SerializeField] private List<ISettingsData> settingsDataObjects;
    public FileDataHandler dataHandler;
    public SettingsDataHandler settingsHandler;
    public InputActionAsset actions;
    #endregion

    #region FIELDS
    [SerializeField] private string fileName = "PlayerSave.json";
    [SerializeField] private string settingsFileName = "SettingsSave.json";
    [SerializeField] private bool useEncryption = true;
    [SerializeField] private bool initializeDataIfNull = false; // Use if you don't want to go through the main menu to test data persistence
    [SerializeField] private bool useAutoSave = false;
    private string selectedProfileID = "";
    private string softProfileID = "";
    [SerializeField] private float autoSaveTimeSeconds = 60f;
    private Coroutine autoSaveCoroutine;
    #endregion

    #region PROPERTIES
    public static DataManager Instance { get; private set; }
    public string Level { get { return gameData.currentLevelName; } }
    public string SelectedProfileID { get { return selectedProfileID; } }
    public string SoftProfileID { get { return softProfileID; } }
    #endregion

    private void Awake()
    {
        // Check if there's already a DataManager
        if(Instance != null)
        {
            // If there is, destroy this one to retain singleton design
            Debug.LogError("Found more than one Data Manager in the scene. Destroying the newest one");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Don't destroy data management on load
        DontDestroyOnLoad(gameObject);

        // Create a FileDataHandler
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        // Create a SettingsDataHandler
        settingsHandler = new SettingsDataHandler(Application.persistentDataPath, settingsFileName);

        InitializeSelectedProfileID();
    }

    private void OnEnable()
    {
        // Subscribe to scene events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to scene events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Load the save data whenever a scene is loaded
    /// </summary>
    /// <param name="scene">The scene being loaded</param>
    /// <param name="mode">The LoadSceneMode</param>
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataObjects = FindAllDataObjects();

        settingsDataObjects = FindAllSettingsDataObjects();

        // Load the Game
        LoadGame();

        // If using autosave, start up the auto saving coroutine
        if (useAutoSave)
        {
            if (autoSaveCoroutine != null)
            {
                StopCoroutine(autoSaveCoroutine);
            }
            autoSaveCoroutine = StartCoroutine(AutoSave());
        }
    }

    /// <summary>
    /// Change the selected profile ID
    /// </summary>
    /// <param name="newProfileID">The profile ID to change to</param>
    public void ChangeSelectedProfileID(string newProfileID)
    {
        // Update the profile to use for saving and loading
        selectedProfileID = newProfileID;
    }

    /// <summary>
    /// Change the soft profile ID - allows the selection of save slots without committing fully
    /// </summary>
    /// <param name="newProfileID">The profile ID to change to</param>
    public void ChangeSelectedProfileIDSoft(string newProfileID)
    {
        // Update the profile to use for saving during game
        softProfileID = newProfileID;
    }

    /// <summary>
    /// Create a new game
    /// </summary>
    public void NewGame()
    {
        // Gets rebinds so that starting a new game doesn't mess up keybinds
        string rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            actions.LoadBindingOverridesFromJson(rebinds);
        }

        gameData = new GameData();

        if (settingsData == null)
        {
            settingsData = new SettingsData();
        }

        // If there is no profileID, make one
        if (selectedProfileID == null)
        {
            selectedProfileID = "0";
        }
    }

    /// <summary>
    /// Save data to a new or existing file
    /// </summary>
    public void SaveGame()
    {
        // If there's no data to save, log a warning and return
        if(gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }

        // Pass the data to other scripts so they can update it
        foreach(IData dataObj in dataObjects)
        {
            dataObj.SaveData(gameData);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // Save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileID);
    }

    public void SaveSettings()
    {
        if (settingsData == null)
        {
            Debug.LogWarning("No Settings data was found.");
        }

        foreach (ISettingsData settingsObj in settingsDataObjects)
        {
            settingsObj.SaveData(settingsData);
        }

        settingsHandler.Save(settingsData);
    }

    /// <summary>
    /// Load the game with previously saved data
    /// </summary>
    public void LoadGame()
    {
        // Load any saved data from a file using the data handler
        gameData = dataHandler.Load(selectedProfileID);

        settingsData = settingsHandler.Load();

        // Start a new game if the data is null and we're configured to initialize data for debugging purposes
        if(gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        if (settingsData == null)
        {
            Debug.LogWarning("Settings not found, creating new default profile");
            settingsData = new SettingsData();
            settingsHandler.Save(settingsData);
        }

        foreach (ISettingsData settingsObj in settingsDataObjects)
        {
            settingsObj.LoadData(settingsData);
        }

        // If no data can be loaded, log a warning and return
        if (gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be loaded");
            return;
        }

        // Push the loaded data to all other scripts that need it
        foreach (IData dataObj in dataObjects)
        {
            dataObj.LoadData(gameData);
        }
    }

    /// <summary>
    /// Delete profile data using the current profile ID
    /// </summary>
    /// <param name="profileID"></param>
    public void DeleteProfileData(string profileID)
    {
        // Delete the data for this proile ID
        dataHandler.Delete(profileID);

        // Initialize the selected profile ID
        InitializeSelectedProfileID();

        // Reload the game so that our data matches the newly selected profile ID
        LoadGame();
    }

    /// <summary>
    /// Set the selected profile ID to the most recently updated profile ID
    /// </summary>
    public void InitializeSelectedProfileID()
    {
        selectedProfileID = dataHandler.GetMostRecentlyUpdatedProfileID();
    }

    /// <summary>
    /// Find all objects that have Data that can be saved or loaded
    /// </summary>
    /// <returns></returns>
    private List<IData> FindAllDataObjects()
    {
        // Check for IData Interfaces in all objects, including inactive objects
        IEnumerable<IData> dataObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IData>();

        // Create a new list from IData objects
        return new List<IData>(dataObjects);
    }

    /// <summary>
    /// Find all objects that have Settings Data that can be saved or loaded
    /// </summary>
    /// <returns></returns>
    private List<ISettingsData> FindAllSettingsDataObjects()
    {
        // Check for ISettingsData Interfaces in all objects, including inactive objects
        IEnumerable<ISettingsData> dataObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<ISettingsData>();

        // Create a new list from ISettingsData objects
        return new List<ISettingsData>(dataObjects);
    }

    /// <summary>
    /// Check if there is GameData to save/load
    /// </summary>
    /// <returns>True is there is no gameData yet, false if there is</returns>
    public bool HasGameData()
    {
        // Return if gameData is null or not
        return gameData != null;
    }

    /// <summary>
    /// Check if there is SettingsData to save/load
    /// </summary>
    /// <returns>True is there is no SettingsData yet, false if there is</returns>
    public bool HasSettingsData()
    {
        // Return if settingsData is null or not
        return settingsData != null;
    }

    /// <summary>
    /// Retrieve all profiles' game data
    /// </summary>
    /// <returns>A call to the dataHandler's LoadAllProfiles() function</returns>
    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    /// <summary>
    /// Autosave in increments of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator AutoSave()
    {
        while(true)
        {
            // Wait for the amount of seconds
            Debug.Log("?");
            yield return new WaitForSeconds(autoSaveTimeSeconds);

            // Save the game
            SaveGame();

            // Notify the console for debugging
            Debug.Log("Auto Saved Game");
        }
    }
}
