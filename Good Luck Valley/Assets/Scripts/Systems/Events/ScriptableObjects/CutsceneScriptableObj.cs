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
    [SerializeField] private bool enterCutsceneActive = false;
    [SerializeField] private bool playingCutscene = false;

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
    /// Set whether a cutscene is playing or not
    /// </summary>
    /// <param name="playingCutscene">Whether a cutscene is playing or not</param>
    public void SetPlayingCutscene(bool playingCutscene)
    {
        this.playingCutscene = playingCutscene;
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

    public bool GetEnterCutsceneActive()
    {
        return enterCutsceneActive;
    }

    /// <summary>
    /// Get whether a cutscene is playing or not
    /// </summary>
    /// <returns>Whether a cutcene is playing or not</returns>
    public bool GetPlayingCutscene()
    {
        return playingCutscene;
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
        enterCutsceneActive = true;
        startLeaveCutscene.Invoke();
    }

    /// <summary>
    /// Invoke all events relating to starting an enter cutscene for a level
    /// </summary>
    public void StartEnterCutscene()
    {
        enterCutsceneActive = true;
        startEnterCutscene.Invoke();
    }

    /// <summary>
    /// Invoke all events relating to ending a leaving cutscene for a level
    /// </summary>
    public void EndLeaveCutscene()
    {
        enterCutsceneActive = false;
        endLeaveCutscene.Invoke();
    }

    /// <summary>
    /// Invoke all events relating to ending an enter cutscene for a level
    /// </summary>
    public void EndEnterCutscene()
    {
        Debug.Log("????");
        enterCutsceneActive = false;
        endEnterCutscene.Invoke();
    }

    /// <summary>
    /// Reset object variables
    /// </summary>
    public void ResetObj()
    {
        playingCutscene = false;
        leaveCutscene = null;
        enterCutscene = null;
        enterCutsceneActive = false;
        playingCutscene = false;
    }
}
