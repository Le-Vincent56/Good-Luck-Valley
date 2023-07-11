using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    #region FIELDS
    #region PLAYER
    public LEVELPOS levelPos;
    public Vector3 playerPosition;
    #endregion

    #region CAMERA
    public bool playCutscene;
    #endregion

    #region NOTES
    public SerializableDictionary<string, bool> notesCollected;
    #endregion

    #region ENVIRONMENT
    public SerializableDictionary<string, bool> assetsActive;
    #endregion

    #endregion

    // The values defined in this constructor will be the default values the game stars with when there's no data to load
    public LevelData()
    {
        levelPos = LEVELPOS.ENTER;
        playerPosition = Vector3.zero;
        playCutscene = true;
        notesCollected = new SerializableDictionary<string, bool>();
        assetsActive = new SerializableDictionary<string, bool>();
    }

    public LevelData(Vector3 playerPosition)
    {
        this.playerPosition = playerPosition;
        playCutscene = true;
        notesCollected = new SerializableDictionary<string, bool>();
        assetsActive = new SerializableDictionary<string, bool>();
    }
}
