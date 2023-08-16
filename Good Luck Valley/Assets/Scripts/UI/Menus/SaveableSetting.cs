using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveableSetting : MonoBehaviour
{
    #region REFERENCES
    #endregion

    #region FIELDS
    private bool reverted;
    private bool unsaveCheck;
    private string originalValue;
    private string currentValue;
    #endregion

    #region PROPERTIES
    public string CurrentValue { get { return currentValue; } set { currentValue = value; } }
    public string OriginalValue { get { return originalValue; } set { originalValue = value; } }
    public bool Reverted { get {  return reverted; } set { reverted = value; } }
    public bool UnsaveCheck { get { return unsaveCheck; } set {  unsaveCheck = value; } }
    #endregion

    private void Start()
    {
        reverted = true;
    }

    /// <summary>
    /// Compares the new value to the original value
    /// </summary>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public bool CheckValue()
    {
        Debug.Log(gameObject.name + " values: current; " + currentValue + ". original; " + originalValue + ".");
        if (originalValue == currentValue)
        {
            reverted = true;
            return true;
        }
        else
        {
            reverted = false;
            return false;
        }
    }

    public void UpdateCurrent(string newValue)
    {
        currentValue = newValue;
    }

    public void OverWriteOriginal()
    {
        originalValue = currentValue;
        reverted = true;
    }

    public void InitializeValues(string newValue)
    {
        currentValue = newValue;
        originalValue = currentValue;
    }
}
