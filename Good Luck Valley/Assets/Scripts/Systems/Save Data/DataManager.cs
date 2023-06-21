using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DataManager : MonoBehaviour
{
    #region REFERENCES
    private GameData gameData;
    [SerializeField] private List<IData> dataObjects;
    public FileDataHandler dataHandler;
    #endregion

    #region FIELDS
    [SerializeField] private string fileName = "PlayerSave.json";
    [SerializeField] private bool useEncryption = true;
    [SerializeField] private bool initializeDataIfNull = false; // Use if you don't want to go through the main menu to test data persistence
    private string selectedProfileID = "";
    #endregion

    #region PROPERTIES
    public static DataManager Instance { get; private set; }
    public string Level { get { return gameData.levelName; } }
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
    /// Load the save ddata whenever a scene is loaded
    /// </summary>
    /// <param name="scene">The scene being loaded</param>
    /// <param name="mode">The LoadSceneMode</param>
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataObjects = FindAllDataObjects();

        // Load the game
        LoadGame();
    }

    public void ChangeSelectedProfileID(string newProfileID)
    {
        // Update the profile to use for saving and loading
        selectedProfileID = newProfileID;

        // Load the game, which will use that profile, updating our game data accordingly
        LoadGame();
    }

    /// <summary>
    /// Create a new game
    /// </summary>
    public void NewGame()
    {
        gameData = new GameData();

        // If there is no profileID, make one
        if(selectedProfileID == null)
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

        Debug.Log("Saved " + dataObjects.Count + " objects!");

        // Pass the data to other scripts so they can update it
        foreach(IData dataObj in dataObjects)
        {
            dataObj.SaveData(gameData);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // Save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileID);
    }

    /// <summary>
    /// Load the game with previously saved data
    /// </summary>
    public void LoadGame()
    {
        // Load any saved data from a file using the data handler
        gameData = dataHandler.Load(selectedProfileID);

        // Start a new game if the data is null and we're configured to initialize data for debugging purposes
        if(gameData == null && initializeDataIfNull)
        {
            NewGame();
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

    public void DeleteProfileData(string profileID)
    {
        // Delete the data for this proile ID
        dataHandler.Delete(profileID);

        // Initialize the selected profile ID
        InitializeSelectedProfileID();

        // Reload the game so that our data matches the newly selected profile ID
        LoadGame();
    }

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
    /// Check if there is GameData to save/load
    /// </summary>
    /// <returns>True is there is no gameData yet, false if there is</returns>
    public bool HasGameData()
    {
        // Return if gameData is null or not
        return gameData != null;
    }

    /// <summary>
    /// Retrieve all profiles' game data
    /// </summary>
    /// <returns>A call to the dataHandler's LoadAllProfiles() function</returns>
    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }
}
