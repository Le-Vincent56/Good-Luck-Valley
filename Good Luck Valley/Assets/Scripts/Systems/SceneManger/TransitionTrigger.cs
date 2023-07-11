using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionTrigger : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] GameObject levelLoader;
    [SerializeField] private LevelDataObj levelDataObj;
    #endregion

    #region FIELDS
    [SerializeField] private string levelToLoad;
    private bool transition;
    private float timeBeforeTransitionTrigger = 5f;
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
            levelDataObj.SetLevelPos(levelToLoad, LEVELPOS.ENTER);
            levelLoader.GetComponent<LoadLevel>().UseLevelData = true;
            levelLoader.GetComponent<LoadLevel>().LoadNextLevel();
       }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
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
