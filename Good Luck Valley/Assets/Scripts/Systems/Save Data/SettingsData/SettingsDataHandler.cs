using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsDataHandler
{
    #region FIELDS
    private string dataDirPath = "";
    private string dataFileName = "";
    private readonly string backupExtension = ".bak";
    #endregion

    public SettingsDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    /// <summary>
    /// Load the settings data
    /// </summary>
    /// <param name="allowRestoreFromBackup">Allow attempts to rollback corrupted settings data</param>
    /// <returns>Settings data</returns>
    public SettingsData Load(bool allowRestoreFromBackup = true)
    {
        // Use Path.Combine() to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        SettingsData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        // Read all of the data
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Deserialize the data from Json back into the C# object
                loadedData = JsonUtility.FromJson<SettingsData>(dataToLoad);
                Debug.Log("Settings Path: " + fullPath);
            }
            catch (Exception e)
            {
                if(allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to load settings file. Attempting to roll back." + "\n" + e);

                    // Attempt to rollback
                    bool rollbackSuccess = AttemptRollback(fullPath);
                    if(rollbackSuccess)
                    {
                        // Try to load again recursively
                        loadedData = Load(false);
                    }
                } else
                {
                    Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e.Message);
                }
            }
        }

        // Return the loaded Data
        return loadedData;
    }

    /// <summary>
    /// Save the settings data
    /// </summary>
    /// <param name="data">The settings data to save</param>
    public void Save(SettingsData data)
    {
        // Use Path.Combine() to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        string backupFilePath = fullPath + backupExtension;

        // Use a try/catch to look for saving file errors
        try
        {
            // Create the director the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize the C# game data object into JSON
            string dataToStore = JsonUtility.ToJson(data, true);

            // Write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            // Verify the newly saved file can be loaded successfully
            SettingsData verifiedSettingsData = Load();

            // If the settings data can be verified, back it up
            if(verifiedSettingsData != null)
            {
                File.Copy(fullPath, backupFilePath, true);
            } else
            {
                throw new Exception("save file could not be verified and backup could not be created");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e.Message);
        }
    }

    /// <summary>
    /// Attempt to roll back corrupted data to a backup file
    /// </summary>
    /// <param name="fullPath">The path that includes both the corrupted file and the backup file</param>
    /// <returns>True if the rollback completed sucessfully, false if the rollback failed</returns>
    private bool AttemptRollback(string fullPath)
    {
        bool success = false;
        string backupFilePath = fullPath + backupExtension;

        try
        {
            // If the file exists, attempt to roll it back by overwriting the original file
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning("Had to roll back to backup file at: " + backupFilePath);
            } else
            {
                throw new Exception("Tried to roll back, but no backup file exists to roll back to");
            }
        } catch(Exception e)
        {
            Debug.LogError("Error occured when trying to roll back to backup file at: " + backupFilePath + "\n" + e);
        }

        return success;
    }
}
