using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveablePlatform : MonoBehaviour
{
    #region FIELDS
    protected bool isTriggered;
    public bool IsTriggered { get { return isTriggered; } set { isTriggered = value; } }

    protected List<GameObject> stuckShrooms = new List<GameObject>();
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered)
        {
            Move();
        }
    }

    public abstract void Move();

    /// <summary>
    /// Checks if the required amount of shroom to move the platform is on it
    /// </summary>
    /// <param name="shrooms"></param>
    public void CheckWeight(GameObject shroom)
    {
        Debug.Log(shroom);
        Debug.Log(stuckShrooms);
        stuckShrooms.Add(shroom);
        isTriggered = true;
    }
}
