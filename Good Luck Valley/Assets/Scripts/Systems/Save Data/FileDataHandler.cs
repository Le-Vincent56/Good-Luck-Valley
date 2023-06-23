using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    #region FIELDS
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "and swing by the 410, beef patty, cornbread";
    private readonly string backupExtension = ".bak";
    #endregion

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    /// <summary>
    /// Load game dat depending on the profile ID
    /// </summary>
    /// <param name="profileID">The profile ID to load data for</param>
    /// <param name="allowRestoreFromBackup">Allow attempts to rollback corrupted data</param>
    /// <returns>GameData that matches the given profile ID</returns>
    public GameData Load(string profileID, bool allowRestoreFromBackup = true)
    {
        // If the profileID is null, return right away
        if(profileID == null)
        {
            return null;
        }

        // Use Path.Combine() to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);

        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Load the serialized data from the file
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        // Read all of the data
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Decrypt the data, if necessary
                if(useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Deserialize the data from Json back into the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            } catch(Exception e)
            {
                // Since we're calling Load(...) recursively, we need to account for the case where
                // the rollback succeeds, but the data is still failing to load for some other reason,
                // which, without this check, may cause an infinite recursion loop
                if(allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to load data file. Attempting to roll back." + "\n" + e);

                    // Attempt to rollback
                    bool rollbackSuccess = AttemptRollback(fullPath);
                    if (rollbackSuccess)
                    {
                        // Try to load again recursively
                        loadedData = Load(profileID, false);
                    }
                } else
                {
                    Debug.LogError("Error occured when trying to load file at path: " + fullPath + " and backup did not work." + "\n" + e);
                }
                
            }
        }

        // Return the loaded Data
        return loadedData;
    }

    /// <summary>
    /// Save game data to a profile ID
    /// </summary>
    /// <param name="data">The GameData to save</param>
    /// <param name="profileID">The profile ID to save to</param>
    public void Save(GameData data, string profileID)
    {
        // If the profileID is null, return right away
        if(profileID == null)
        {
            Debug.Log("No ProfileID");
            return;
        }

        // Use Path.Combine() to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        string backupFilePath = fullPath + backupExtension;

        // Use a try/catch to look for saving file errors
        try
        {
            // Create the director the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize the C# game data object into JSON
            string dataToStore = JsonUtility.ToJson(data, true);

            // Encrypt the data, if necessary
            if(useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // Write the serialized data to the file
            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            // Verify the newly saved file can be loaded successfully
            GameData verifiedGameData = Load(profileID);

            // If the data can be verified, back it up
            if(verifiedGameData != null)
            {
                File.Copy(fullPath, backupFilePath, true);
            } else
            {
                throw new Exception("Save file could not be verified and backup could not be created");
            }

        } catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e.Message);
        }
    }

    /// <summary>
    /// Delete the game data of a profile ID
    /// </summary>
    /// <param name="profileID">The profile ID to delete the game data of</param>
    public void Delete(string profileID)
    {
        // If the profileID is null, return right away
        if (profileID == null)
        {
            return;
        }

        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        try
        {
            // Ensure the data file exists at this path before deleting the directory
            if(File.Exists(fullPath))
            {
                // Delete the profile folder and everything within it
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            } else
            {
                Debug.LogWarning("Tried to delete profile data, but data does not exist at this path");
            }

        } catch (Exception e)
        {
            Debug.LogError("Failed to delete profile data for profileID: " + profileID + " at path: " + fullPath + "\n" + e); 
        }
    }

    /// <summary>
    /// Load all profiles
    /// </summary>
    /// <returns>A dictionary that uses profile IDs as the keys and their associated GameData's as their values</returns>
    public Dictionary<string, GameData> LoadAllProfiles()
    {
        // Create a new directory for profiles
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        // Loop over all directory names in the data directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach(DirectoryInfo dirInfo in dirInfos)
        {
            string profileID = dirInfo.Name;

            // Check if the data file exists if it doesn't, then this folder
            // isn't a profile and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
            if(!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: " + profileID);
                continue;
            }

            // Load the game data for this profile and put it in the dictionary
            GameData profileData = Load(profileID);

            // Ensure the profile data isn't null, because if it is, then something
            // went wrong and we should let ourselves know
            if(profileData != null)
            {
                // If the profile is valid, add it to the dictionary
                profileDictionary.Add(profileID, profileData);
            } else
            {
                Debug.LogError("Tried to load profile but something went wrong. Profile ID: " + profileID);
            }
        }

        // Return the profile dictionary
        return profileDictionary;
    }

    /// <summary>
    /// Get the most recently updated profile ID (the most recently played)
    /// </summary>
    /// <returns>The most recently updated profile ID</returns>
    public string GetMostRecentlyUpdatedProfileID()
    {
        string mostRecentProfileID = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach(KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileID = pair.Key;
            GameData gameData = pair.Value;

            // Skip this entry if the gameData is null
            if(gameData == null)
            {
                continue;
            }

            // If this is the first data we've come across that exists, it's the most recent so far
            if(mostRecentProfileID == null)
            {
                mostRecentProfileID = profileID;
            } else
            {
                // Otherwise, compare to see which date is the most recent
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileID].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);

                // The greatest DateTime value is the most recent
                if(newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileID = profileID;
                }
            }
        }

        // Return the most recent profile ID
        return mostRecentProfileID;
    }

    /// <summary>
    /// Encrypt and decrypt game data
    /// </summary>
    /// <param name="data">The data to encrypt/decrypt</param>
    /// <returns>Encrypted/decrypted data</returns>
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        
        // Use XOR to encrypt data so that players cannot cheat easily
        for(int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifiedData;
    }

    /// <summary>
    /// Attempt to roll back corrupted data to a backup file
    /// </summary>
    /// <param name="fullPath">The path that includes both the corrupted file and the backup file</param>
    /// <returns>True, if the rollback completed successfully, false if the rollback failed</returns>
    private bool AttemptRollback(string fullPath)
    {
        bool success = false;
        string backupFilePath = fullPath + backupExtension;

        try
        {
            // If the file exists, attempt to roll it back by overwriting the original file
            if(File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning("Had to roll back to backup file at: " + backupFilePath);
            } else
            {
                throw new Exception("Tried to roll back, but no backup file exists to roll back to");
            }
        } catch (Exception e)
        {
            Debug.LogError("Error occured when trying to roll back to backup file at: " + backupFilePath + "\n" + e);
        }

        return success;
    }
}
