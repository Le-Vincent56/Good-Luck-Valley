using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "LoadLevelScriptableObject", menuName = "ScriptableObjects/Load Level Event")]
public class LoadLevelScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool activeTriggers = false;
    [SerializeField] private bool isLoading = false;
    [SerializeField] private bool loadingThroughCutscene = false;
    [SerializeField] private bool insideLoadTrigger = false;

    #region EVENTS
    public UnityEvent startLoad;
    public UnityEvent<float> startMusicLoad;
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

        if(startMusicLoad == null)
        {
            startMusicLoad = new UnityEvent<float>();
        }
        #endregion
    }

    /// <summary>
    /// Set whether loading triggers are active or not
    /// </summary>
    /// <param name="triggersActive">Whether loading triggers are active or not</param>
    public void SetTriggersActive(bool triggersActive)
    {
        activeTriggers = triggersActive;
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
    /// Set whether the player is inside a load trigger or not
    /// </summary>
    /// <param name="insideLoadTrigger">Whether the player is inside a load trigger or not</param>
    public void SetInLoadTrigger(bool insideLoadTrigger)
    {
        this.insideLoadTrigger = insideLoadTrigger;
    }

    /// <summary>
    /// Set whether a player is loading through a cutscene or not
    /// </summary>
    /// <param name="loadingThroughCutscene">Whether a player is loading through a cutscene or not</param>
    public void SetLoadingThroughCutscene(bool loadingThroughCutscene)
    {
        this.loadingThroughCutscene = loadingThroughCutscene;
    }

    /// <summary>
    /// Get whether loading triggers are active or not
    /// </summary>
    /// <returns>Whether loading triggers are active or not</returns>
    public bool GetTriggersActive()
    {
        return activeTriggers;
    }

    /// <summary>
    /// Get whether the level is loading or not
    /// </summary>
    /// <returns>Whether the level is loading or not</returns>
    public bool GetIsLoading()
    {
        return isLoading;
    }

    /// <summary>
    /// Get whether the player is inside a load trigger or not
    /// </summary>
    /// <returns>Whether the player is inside a load trigger or not</returns>
    public bool GetInLoadTrigger()
    {
        return insideLoadTrigger;
    }

    /// <summary>
    /// Get whether the player is loading through a cutscene or not
    /// </summary>
    /// <returns>Whether the player is loading through a cutscene or not</returns>
    public bool GetLoadingThroughCutscene()
    {
        return loadingThroughCutscene;
    }

    /// <summary>
    /// Trigger all events relating to loading a level
    /// </summary>
    public void StartLoad()
    {
        isLoading = true;
        startLoad.Invoke();
    }

    /// <summary>
    /// Trigger all events relating to updating music through loading
    /// </summary>
    /// <param name="progressLevel">The level to progress to</param>
    public void StartMusicLoad(float progressLevel)
    {
        startMusicLoad.Invoke(progressLevel);
    }

    /// <summary>
    /// Trigger all events relating to ending the loading process of a level
    /// </summary>
    public void EndLoad()
    {
        isLoading = false;
        endLoad.Invoke();
    }

    /// <summary>
    /// Reset object variables
    /// </summary>
    public void ResetObj()
    {
        activeTriggers = false;
        isLoading = false;
        loadingThroughCutscene = false;
        insideLoadTrigger = false;
    }
}
