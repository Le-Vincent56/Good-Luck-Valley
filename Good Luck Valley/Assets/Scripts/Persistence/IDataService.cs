using System.Collections.Generic;

namespace GoodLuckValley.Persistence
{
    public interface IDataService
    {
        /// <summary>
        /// Save the GameData
        /// </summary>
        /// <param name="data">The GameData to save</param>
        /// <param name="overwrite">Whether or not to overwrite the current data</param>
        void Save(GameData data, bool overwrite = true);

        /// <summary>
        /// Load a GameData with a given name
        /// </summary>
        /// <param name="name">The name of the GameData</param>
        /// <returns></returns>
        GameData Load(string name);

        /// <summary>
        /// Delete a GameData with a given name
        /// </summary>
        /// <param name="name">The name of the GameData</param>
        void Delete(string name);

        /// <summary>
        /// Delete all data
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// List all Saves
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> ListSaves();
    }
}