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
    ShroomType GetType();
}
