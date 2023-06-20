using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{
    #region REFERENCES
    private GameData gameData;
    [SerializeField] private List<IData> dataObjects;
    private FileDataHandler dataHandler;
    #endregion

    #region FIELDS
    [SerializeField] private string fileName = "PlayerSave.json";
    [SerializeField] private bool useEncryption = true;
    #endregion

    #region PROPERTIES
    public static DataManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Found more than one Data Manager in the scene");
        }
        Instance = this;
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        dataObjects = FindAllDataObjects();

        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void SaveGame()
    {
        Debug.Log("Saved " + dataObjects.Count + " objects!");

        // Pass the data to other scripts so they can update it
        foreach(IData dataObj in dataObjects)
        {
            dataObj.SaveData(ref gameData);
        }

        // Save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    public void LoadGame()
    {
        // Load any saved data from a file using the data handler
        gameData = dataHandler.Load();

        // If no data can be loaded, initialize to a new game
        if (gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        // Push the loaded data to all other scripts that need it
        foreach (IData dataObj in dataObjects)
        {
            dataObj.LoadData(gameData);
        }
    }

    private List<IData> FindAllDataObjects()
    {
        IEnumerable<IData> dataObjects = FindObjectsOfType<MonoBehaviour>().OfType<IData>();

        return new List<IData>(dataObjects);
    }
}
