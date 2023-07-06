using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousLevelTransitionTrigger : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] GameObject levelLoader;

    #endregion
    private bool transition;
    private float timeBeforeTransitionTrigger = 5f;
    #region FIELDS

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
            DataManager.Instance.SaveGame();
            levelLoader.GetComponent<LoadLevel>().LoadPrevLevel();
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
