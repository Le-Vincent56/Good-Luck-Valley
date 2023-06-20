using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IData
{
    void LoadData(GameData data);

    // Pass by reference so that the implementing script can modify the data
    void SaveData(ref GameData data);
}
