using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "CutsceneScriptableObject", menuName = "ScriptableObjects/Cutscene Event")]
public class CutsceneScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private PlayableAsset leaveCutscene;
    [SerializeField] private PlayableAsset enterCutscene;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent startLotusCutscene;
    public UnityEvent endLotusCutscene;
    public UnityEvent startLeaveCutscene;
    public UnityEvent startEnterCutscene;
    public UnityEvent endLeaveCutscene;
    public UnityEvent endEnterCutscene;
    #endregion
    #endregion

    private void OnEnable()
    {
        #region CREATE EVENTS
        if(startLotusCutscene == null)
        {
            startLotusCutscene = new UnityEvent();
        }

        if (endLotusCutscene == null)
        {
            endLotusCutscene = new UnityEvent();
        }

        if (startLeaveCutscene == null)
        {
            startLeaveCutscene = new UnityEvent();
        }

        if (startEnterCutscene == null)
        {
            startEnterCutscene = new UnityEvent();
        }

        if (endLeaveCutscene == null)
        {
            endLeaveCutscene = new UnityEvent();
        }

        if (endEnterCutscene == null)
        {
            endEnterCutscene = new UnityEvent();
        }
        #endregion
    }

    /// <summary>
    /// Set the leave cutscene for the CURRENT level
    /// </summary>
    /// <param name="leaveCutscene">Set the leave cutscene for the current level</param>
    public void SetLeaveCutscene(PlayableAsset leaveCutscene)
    {
        this.leaveCutscene = leaveCutscene;
    }

    /// <summary>
    /// Set the enter cutscene for the NEXT level
    /// </summary>
    /// <param name="enterCutscene">The enter cutscene for the NEXT level</param>
    public void SetEnterCutscene(PlayableAsset enterCutscene)
    {
        this.enterCutscene = enterCutscene;
    }

    /// <summary>
    /// Get the leave cutscene
    /// </summary>
    /// <returns>The leave cutscene</returns>
    public PlayableAsset GetLeaveCutscene()
    {
        return leaveCutscene;
    }

    /// <summary>
    /// Get the enter cutscene
    /// </summary>
    /// <returns>The enter cutscene</returns>
    public PlayableAsset GetEnterCutscene()
    {
        return enterCutscene;
    }

    /// <summary>
    /// Trigger events relating to starting the Lotus Cutscene
    /// </summary>
    public void StartLotusCutscene()
    {
        startLotusCutscene.Invoke();
    }

    /// <summary>
    /// Trigger events relating to ending the Lotus Cutscene
    /// </summary>
    public void EndLotusCutscene()
    {
        endLotusCutscene.Invoke();
    }

    /// <summary>
    /// Invoke all events relating to starting a leaving cutscene for a level
    /// </summary>
    public void StartLeaveCutscene()
    {
        startLeaveCutscene.Invoke();
    }

    /// <summary>
    /// Invoke all events relating to starting an enter cutscene for a level
    /// </summary>
    public void StartEnterCutscene()
    {
        startEnterCutscene.Invoke();
    }

    /// <summary>
    /// Invoke all events relating to ending a leaving cutscene for a level
    /// </summary>
    public void EndLeaveCutscene()
    {
        endLeaveCutscene.Invoke();
    }

    /// <summary>
    /// Invoke all events relating to ending an enter cutscene for a level
    /// </summary>
    public void EndEnterCutscene()
    {
        endEnterCutscene.Invoke();
    }
}
