using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    public class FileDataService : IDataService
    {
        #region FIELDS
        ISerializer serializer;
        string dataPath;
        string fileExtension;
        #endregion

        public FileDataService(ISerializer serializer)
        {
            dataPath = Application.persistentDataPath;
            fileExtension = "json";
            this.serializer = serializer;
        }

        string GetPathToFile(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        }

        public void Save(GameData data, bool overwrite = true)
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

        public GameData Load(string name)
        {
            // Get a reference to the file location
            string fileLocation = GetPathToFile(name);

            // Make sure the file exists with the same name
            if(!File.Exists(fileLocation))
            {
                throw new ArgumentException($"No persisted GameData with name '{name}'");
            }

            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name)
        {
            // Get a reference to the file location
            string fileLocation = GetPathToFile(name);

            // If the file exists, delete it
            if(File.Exists(fileLocation)) File.Delete(fileLocation);
        }

        public void DeleteAll()
        {
            // Loop through all the files in the directory
            foreach(string filePath in Directory.GetFiles(dataPath))
            {
                // Delete the files
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            // Loop through all of the files in the directory
            foreach (string path in Directory.EnumerateFiles(dataPath))
            {
                // If the file extension is the same,return the file name without the extension
                // as an IEnumerable
                if(Path.GetExtension(path) == $".{fileExtension}")
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}