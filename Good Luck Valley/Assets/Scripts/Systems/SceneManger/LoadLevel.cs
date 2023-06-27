using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    #region REFERENCES
    private GameObject player;

    #endregion

    #region FIELDS
    public Animator transition;
    public float transitionTime = 1f;
    #endregion

    #region PROPERTIES

    #endregion

    public void LoadNextLevel()
    {
        StartCoroutine(LoadingLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    /// <summary>
    /// Pauses before triggering level change animation
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    IEnumerator LoadingLevel(int levelIndex)
    {
        // Play aimation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load scene
        SceneManager.LoadScene(levelIndex);
    }

 
}
