using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    #region REFERENCES
    private GameObject player;
    #endregion

    #region FIELDS
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    public Animator transition;
    public float transitionTime = 1f;
    #endregion

    public void LoadNextLevel()
    {
        StartCoroutine(LoadingLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadPrevLevel()
    {
        StartCoroutine(LoadingLevel(SceneManager.GetActiveScene().buildIndex - 1));
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

    /// <summary>
    /// Start loading a level
    /// </summary>
    public void StartLoading()
    {
        // Trigger start load event
        loadLevelEvent.StartLoad();
    }

    /// <summary>
    /// End loading a level
    /// </summary>
    public void EndLoading()
    {
        // Trigger end load event
        loadLevelEvent.EndLoad();

        // Check to see if the lotus cutscene needs to be played
        PlayableDirector camDirector = GameObject.Find("Main Camera").GetComponent<PlayableDirector>();
        if(camDirector != null)
        {
            if(camDirector.enabled)
            {
                cutsceneEvent.StartLotusCutscene();
            }
            
        }
    }
 
}
