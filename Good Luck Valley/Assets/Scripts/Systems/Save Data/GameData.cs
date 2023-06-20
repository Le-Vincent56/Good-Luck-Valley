using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    #region FIELDS
    public long lastUpdated;
    public string levelName;
    public string playtime;
    public Vector3 playerPosition;
    public SerializableDictionary<string, bool> notesCollected;
    #endregion

    // Constructor will have default values for when the game starts when there's no data to load
    public GameData()
    {
        levelName = "Prologue";
        playtime = "0:00:00";

        // Default position - Tutorial start
        playerPosition = new Vector3(-31.85f, 3.52f, 0f);
        notesCollected = new SerializableDictionary<string, bool>();
    }
}
