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
    #endregion

    public SettingsDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public SettingsData Load()
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
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e.Message);
            }
        }

        // Return the loaded Data
        return loadedData;
    }

    public void Save(SettingsData data)
    {
        Debug.Log("Saving Settings");
        // Use Path.Combine() to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);

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
                    Debug.Log("Wrote data!");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e.Message);
        }
    }

}
