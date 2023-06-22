using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IData
{
    /// <summary>
    /// Load data
    /// </summary>
    /// <param name="data">The GameData object to load data from</param>
    void LoadData(GameData data);

    /// <summary>
    /// Save data
    /// </summary>
    /// <param name="data">The GameData object to save data to</param>
    void SaveData(GameData data);
}
