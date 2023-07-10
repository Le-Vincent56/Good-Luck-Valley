using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "LoadLevelScriptableObject", menuName = "ScriptableObjects/Load Level Event")]
public class LoadLevelScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool isLoading = false;

    #region EVENTS
    public UnityEvent startLoad;
    public UnityEvent endLoad;
    #endregion
    #endregion

    private void OnEnable()
    {
        #region CREATE EVENTS
        if (startLoad == null)
        {
            startLoad = new UnityEvent();
        }

        if (endLoad == null)
        {
            endLoad = new UnityEvent();
        }
        #endregion
    }

    /// <summary>
    /// Set whether the level is loading or not
    /// </summary>
    /// <param name="isLoading">Whether the level is loading or not</param>
    public void SetIsLoading(bool isLoading)
    {
        this.isLoading = isLoading;
    }

    /// <summary>
    /// Get whether the level is loading or not
    /// </summary>
    /// <returns>Whether the level is loading or not</returns>
    public bool GetIsLoading()
    {
        return isLoading;
    }

    public void StartLoad()
    {
        isLoading = true;
        startLoad.Invoke();
    }

    public void EndLoad()
    {
        isLoading = false;
        endLoad.Invoke();
    }
}
