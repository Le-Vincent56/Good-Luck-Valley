using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    public class FileDataService : IDataService
    {
        private readonly ISerializer serializer;
        private readonly string dataPath;
        private readonly string fileExtension;

        public FileDataService(ISerializer serializer)
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
        /// Save the Game Data using File IO
        /// </summary>
        public void Save(GameData data, bool overwrite = true)
        {
            // Get the file location
            string fileLocation = GetPathToFile(data.Name);

            // Check if not overwriting but a file exists
            if (!overwrite && File.Exists(fileLocation))
            {
                // Throw an exception
                throw new IOException($"The file '{data.Name}.{fileExtension} already exists and cannot be overwritten");
            }

            // Serialize the data and write it to the file location
            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        /// <summary>
        /// Load the Game Data using File IO
        /// </summary>
        public GameData Load(string name)
        {
            // Get the file location
            string fileLocation = GetPathToFile(name);

            // Check if no file exists at the file location
            if(!File.Exists(fileLocation))
            {
                // Throw an exception
                throw new ArgumentException($"No persisted GameData with name '{name}'");
            }

            // Deserialize the file into GameData
            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        /// <summary>
        /// Delete a persisted Game Data file using its name
        /// </summary>
        public void Delete(string name)
        {
            // Get the file location
            string fileLocation = GetPathToFile(name);

            // Check if the file exists
            if (File.Exists(fileLocation)) 
                // Delete the file
                File.Delete(fileLocation);
        }

        /// <summary>
        /// Delete all persisted Game Data files
        /// </summary>
        public void DeleteAll()
        {
            // Iterate through each file in the persistence data path
            foreach(string filePath in Directory.GetFiles(dataPath))
            {
                // Delete the files
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// List all persisted Game Data files
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ListSaves()
        {
            // Iterate through each file in the persistence data path
            foreach (string path in Directory.EnumerateFiles(dataPath))
            {
                // Check if the file has the correct extension
                if (Path.GetExtension(path) == fileExtension)
                {
                    // Return the file name without the extension
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}
