using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SaveMenuScriptableObject", menuName = "ScriptableObjects/Save Menu Event")]
public class SaveMenuScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool saveMenuOpen = false;
    [SerializeField] private float saveCloseBuffer = 0.25f;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent activateSaveMenuEvent;
    #endregion
    #endregion

    private void OnEnable()
    {
        #region CREATE EVENTS
        if (activateSaveMenuEvent == null)
        {
            activateSaveMenuEvent = new UnityEvent();
        }
        #endregion
    }

    /// <summary>
    /// Set whether the save menu is open or not
    /// </summary>
    /// <param name="saveMenuOpen">Whether the save menu is open</param>
    public void SetSaveMenuOpen(bool saveMenuOpen)
    {
        this.saveMenuOpen = saveMenuOpen;
    }

    /// <summary>
    /// Set the save menu close buffer
    /// </summary>
    /// <param name="saveCloseBuffer">The save menu close buffer</param>
    public void SetSaveCloseBuffer(float saveCloseBuffer)
    {
        this.saveCloseBuffer = saveCloseBuffer;
    }

    /// <summary>
    /// Get whether the save menu is open or not
    /// </summary>
    /// <returns>Whether the save menu is open</returns>
    public bool GetSaveMenuOpen()
    {
        return saveMenuOpen;
    }

    /// <summary>
    /// Get the save close buffer
    /// </summary>
    /// <returns>The save close buffer</returns>
    public float GetSaveCloseBuffer()
    {
        return saveCloseBuffer;
    }

    /// <summary>
    /// Trigger events related to opening the save menu
    /// </summary>
    public void ActivateSaveMenu()
    {
        activateSaveMenuEvent.Invoke();
    }
}
