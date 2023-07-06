using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShroomType
{
    Regular,
    Wall
}

public interface IShroomeable
{
    /// <summary>
    /// Get the type of shroom that will spawn on the tile
    /// </summary>
    /// <returns>The type of shroom that will spawn on the tile</returns>
    ShroomType GetType();
}
