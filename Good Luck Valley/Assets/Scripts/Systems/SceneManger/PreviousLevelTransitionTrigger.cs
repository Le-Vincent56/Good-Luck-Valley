using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreviousLevelTransitionTrigger : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    [SerializeField] private LevelDataObj levelDataObj;
    [SerializeField] GameObject levelLoader;
    #endregion

    #region FIELDS
    [SerializeField] private string levelToLoad;
    private bool transition;
    private float timeBeforeTransitionTrigger = 1f;
    #endregion

    #region PROPERTIES

    #endregion

    private void Awake()
    {
        transition = false;
        StartCoroutine(CheckTransition());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && transition)
        {
            // Save the game before loading the next level
            loadLevelEvent.SetInLoadTrigger(true);
            levelDataObj.SetLevelPos(levelToLoad, LEVELPOS.RETURN);
            levelLoader.GetComponent<LoadLevel>().UseLevelData = true;
            levelLoader.GetComponent<LoadLevel>().LoadPrevLevel();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            loadLevelEvent.SetInLoadTrigger(false);
            transition = true;
        }

    }

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
