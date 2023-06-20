using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;


public class SaveSerial : MonoBehaviour
{
    #region REFERENCES
    private PlayerMovement playerMovementToSave;
    private MushroomManager mushroomManagerToSave;
    private Journal journalToSave;
    #endregion

    /// <summary>
    /// Save a file
    /// </summary>
    void SaveGame()
    {
        // Retrieve the necessary variables
        playerMovementToSave = GameObject.Find("Player").GetComponent<PlayerMovement>();
        mushroomManagerToSave = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        journalToSave = GameObject.Find("JournalUI").GetComponent<Journal>();

        // Create a Binary Formatter and open a file stream for saving
        BinaryFormatter bf = new BinaryFormatter();
        FileStream saveFile = File.Create(Application.persistentDataPath + "/PlayerSaveData.dat");

        // Create a new SaveData object and add necessary save data
        SaveData data = new SaveData();
        data.SavedPlayerMovement = playerMovementToSave;
        data.SavedMushroomManager = mushroomManagerToSave;
        data.SavedJournal = journalToSave;

        // Serialize data into the save file using Binary Formatter
        bf.Serialize(saveFile, data);

        // Close the filestream
        saveFile.Close();

        // Print a message
        Debug.Log("Game Data Saved!");
    }

    /// <summary>
    /// Load a save file
    /// </summary>
    void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerSaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream loadFile = File.Open(Application.persistentDataPath + "/PlayerSaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(loadFile);
            loadFile.Close();

            playerMovementToSave = data.SavedPlayerMovement;
            mushroomManagerToSave = data.SavedMushroomManager;
            journalToSave = data.SavedJournal;

            Debug.Log("Game Data Loaded!");
        } else
        {
            Debug.LogError("There is No Save Data!");
        }
    }

    /// <summary>
    /// Reset save files
    /// </summary>
    void ResetData()
    {
        if(File.Exists(Application.persistentDataPath + "/PlayerSaveData.dat"))
        {
            File.Delete(Application.persistentDataPath + "/PlayerSaveData.dat");
            playerMovementToSave = null;
            mushroomManagerToSave = null;

            Debug.Log("Data Reset Complete!");
        } else
        {
            Debug.LogError("No Save Data to Delete!");
        }
    }
}

[Serializable]
class SaveData
{
    #region REFERENCES
    private PlayerMovement savedPlayerMovement;
    private MushroomManager savedMushroomManager;
    private Journal savedJournal;
    #endregion

    #region PROPERTIES
    public PlayerMovement SavedPlayerMovement { get { return savedPlayerMovement; } set { savedPlayerMovement = value; } }
    public MushroomManager SavedMushroomManager { get { return savedMushroomManager; } set { savedMushroomManager = value; } }
    public Journal SavedJournal { get { return savedJournal; } set { savedJournal = value; } }
    #endregion
}
