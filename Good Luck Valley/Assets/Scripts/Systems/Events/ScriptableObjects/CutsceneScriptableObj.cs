using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CutsceneScriptableObject", menuName = "ScriptableObjects/Cutscene Event")]
public class CutsceneScriptableObj : ScriptableObject
{
    #region FIELDS

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent startLotusCutscene;
    public UnityEvent endLotusCutscene;
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
        #endregion
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
}
