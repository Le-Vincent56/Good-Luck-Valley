using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    public class FileSettingsService
    {
        #region FIELDS
        ISerializer serializer;
        string dataPath;
        string fileExtension;
        #endregion

        public FileSettingsService(ISerializer serializer)
        {
            dataPath = Application.persistentDataPath;
            fileExtension = "json";
            this.serializer = serializer;
        }

        string GetPathToFile(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        }

        public void Save(SettingsData data, bool overwrite = true)
        {
            // Get a reference to the file location
            string fileLocation = GetPathToFile(data.Name);

            // Check if we should overwrite/if the file already exists
            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"The file '{data.Name}.{fileExtension}' already exists and cannot be overwritten");
            }

            // Write all text and serialize it to the file
            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        public SettingsData Load(string name)
        {
            // Get a reference to the file location
            string fileLocation = GetPathToFile(name);

            // Make sure the file exists with the same name
            if (!File.Exists(fileLocation))
            {
                return null;
            }

            return serializer.Deserialize<SettingsData>(File.ReadAllText(fileLocation));
        }
    }
}