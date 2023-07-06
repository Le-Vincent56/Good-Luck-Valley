using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    #region FIELDS
    #region PLAYER
    public Vector3 playerPosition;
    #endregion

    #region CAMERA
    public bool playCutscene;
    #endregion

    #endregion

    // The values defined in this constructor will be the default values the game stars with when there's no data to load
    public LevelData()
    {
        playerPosition = Vector3.zero;
        playCutscene = true;
    }

    public LevelData(Vector3 playerPosition)
    {
        this.playerPosition = playerPosition;
        playCutscene = true;
    }
}
