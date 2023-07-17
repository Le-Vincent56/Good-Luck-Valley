using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PreviousLevelTransitionTrigger : MonoBehaviour
{
    #region REFERENCES
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
            // Set inside load triger
            loadLevelEvent.SetInLoadTrigger(true);

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
        levelDataObj.SetLevelPos(levelToLoad, LEVELPOS.RETURN);
        levelLoader.GetComponent<LoadLevel>().UseLevelData = true;

        // Load the previous level
        levelLoader.GetComponent<LoadLevel>().LoadPrevLevel();
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
