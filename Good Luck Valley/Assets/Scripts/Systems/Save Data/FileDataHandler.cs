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
    #endregion

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string profileID)
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
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e.Message);
            }
        }

        // Return the loaded Data
        return loadedData;
    }

    public void Save(GameData data, string profileID)
    {
        // If the profileID is null, return right away
        if(profileID == null)
        {
            return;
        }

        // Use Path.Combine() to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);

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
                    Debug.Log("Wrote data!");
                }
            }
        } catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e.Message);
        }
    }

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

        return profileDictionary;
    }

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

        return mostRecentProfileID;
    }

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
}
