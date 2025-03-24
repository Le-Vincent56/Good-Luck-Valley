using System.Collections.Generic;

namespace GoodLuckValley.Persistence
{
    public interface IDataService
    {
        /// <summary>
        /// Save a GameData
        /// </summary>
        void Save(GameData data, bool overwrite = true);

        /// <summary>
        /// Load a GameData
        /// </summary>
        GameData Load(string name);

        /// <summary>
        /// Delete a GameData using a string
        /// </summary>
        void Delete(string name);

        /// <summary>
        /// Delete all GameData
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// List all GameDatas by their name
        /// </summary>
        IEnumerable<string> ListSaves();
    }
}
