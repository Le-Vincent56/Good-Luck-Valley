using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionTrigger : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    [SerializeField] private LevelDataObj levelDataObj;
    [SerializeField] GameObject levelLoader;
    #endregion

    #region FIELDS
    [SerializeField] private string levelToLoad;
    #endregion

    #region PROPERTIES

    #endregion

    private void Awake()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.tag == "Player")
       {
            loadLevelEvent.SetInLoadTrigger(true);
            levelDataObj.SetLevelPos(levelToLoad, LEVELPOS.ENTER);
            levelLoader.GetComponent<LoadLevel>().UseLevelData = true;
            levelLoader.GetComponent<LoadLevel>().LoadNextLevel();
       }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            loadLevelEvent.SetInLoadTrigger(false);
        }
    }
}
