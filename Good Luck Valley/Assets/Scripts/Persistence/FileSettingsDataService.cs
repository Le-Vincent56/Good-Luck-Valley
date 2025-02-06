using System.IO;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    public class FileSettingsDataService
    {
        private readonly ISerializer serializer;
        private readonly string dataPath;
        private readonly string fileExtension;

        public FileSettingsDataService(ISerializer serializer)
        {
            dataPath = Application.persistentDataPath;
            fileExtension = "json";
            this.serializer = serializer;
        }

        /// <summary>
        /// Get the path to the file
        /// </summary>
        private string GetPathToFile(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        }

        /// <summary>
        /// Save the Settings Data using File IO
        /// </summary>
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

        /// <summary>
        /// Load the Settings Data using File IO
        /// </summary>
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
