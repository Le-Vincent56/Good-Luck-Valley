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
    public bool playCutscene;
    public Vector3 playerPosition;
    public SerializableDictionary<string, bool> notesCollected;
    #endregion

    // Constructor will have default values for when the game starts when there's no data to load
    public GameData()
    {
        levelName = "Prologue";
        playtime = "0:00:00";
        playCutscene = true;

        // Default position - Tutorial start
        playerPosition = new Vector3(-27.47f, 7.798f, 0f);
        notesCollected = new SerializableDictionary<string, bool>();
    }
}
