using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelPosData 
{
    public LEVELPOS levelPos;
    public Vector3 playerEnterPosition;
    public Vector3 playerReturnPosition;

    public LevelPosData(LEVELPOS levelPos, Vector3 playerEnterPosition, Vector3 playerReturnPosition)
    {
        this.levelPos = levelPos;
        this.playerEnterPosition = playerEnterPosition;
        this.playerReturnPosition = playerReturnPosition;
    }
}

[System.Serializable]
[CreateAssetMenu(menuName = "Persistent Level Data")]
public class LevelDataObj : ScriptableObject, IData
{
    #region FIELDS
    public SerializableDictionary<string, LevelPosData> levelPosData = new SerializableDictionary<string, LevelPosData>()
    {
        { "Prologue", new LevelPosData(LEVELPOS.ENTER, new Vector3(-27.46f, 7.85f, 0f), new Vector3(16.06508f, -0.7317045f, 0f)) },
        { "Level 1", new LevelPosData(LEVELPOS.ENTER, new Vector3(-39.82f, 6.31f, 0f), new Vector3(-23.1495f, 72.47359f, 0f)) }
    };

    public void SetLevelPos(string key, LEVELPOS levelPosToSet)
    {
        levelPosData[key].levelPos = levelPosToSet;
    }

    public void LoadData(GameData data)
    {
        levelPosData["Prologue"].levelPos = data.levelData["Prologue"].levelPos;
    }

    public void SaveData(GameData data)
    {
        data.levelData["Prologue"].levelPos = levelPosData["Prologue"].levelPos;
    }
    #endregion
}
