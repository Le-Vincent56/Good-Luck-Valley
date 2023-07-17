using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TransitionTrigger : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private PauseScriptableObj pauseEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    [SerializeField] private LevelDataObj levelDataObj;
    [SerializeField] private GameObject levelLoader;
    [SerializeField] private PlayableAsset cutsceneToPlayLeave;
    [SerializeField] private PlayableAsset cutsceneToPlayEnter;
    #endregion

    #region FIELDS
    [SerializeField] private string levelToLoad;
    [SerializeField] private bool transition;
    [SerializeField] private float timeBeforeTransitionTrigger = 1f;
    #endregion

    private void Awake()
    {
        transition = false;
        StartCoroutine(CheckTransition());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.tag == "Player" && transition && loadLevelEvent.GetTriggersActive())
       {
            // Set inside load trigger
            loadLevelEvent.SetInLoadTrigger(true);

            // Set variables
            pauseEvent.SetCanPause(false);

            // Set the cutscene event and play it
            cutsceneEvent.SetLeaveCutscene(cutsceneToPlayLeave);
            cutsceneEvent.SetEnterCutscene(cutsceneToPlayEnter);
            cutsceneEvent.StartLeaveCutscene();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Reset triggers and variables
            pauseEvent.SetCanPause(true);
            loadLevelEvent.SetInLoadTrigger(false);
            transition = true;
            timeBeforeTransitionTrigger = 1f;
        }
    }

    /// <summary>
    /// Trigger the loading screen
    /// </summary>
    public void TriggerLoad()
    {
        // Set variables
        loadLevelEvent.SetInLoadTrigger(true);
        loadLevelEvent.SetLoadingThroughCutscene(true);
        levelDataObj.SetLevelPos(levelToLoad, LEVELPOS.ENTER);
        levelLoader.GetComponent<LoadLevel>().UseLevelData = true;

        // Load the next level
        levelLoader.GetComponent<LoadLevel>().LoadNextLevel();
    }

    /// <summary>
    /// Check for transition times
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckTransition()
    {
        while (timeBeforeTransitionTrigger > 0)
        {
            timeBeforeTransitionTrigger -= Time.deltaTime;
            yield return null;
        }
        transition = true;
    }
}
