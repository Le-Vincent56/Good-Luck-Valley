using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public enum LEVELPOS
{
    DEFAULT,
    ENTER,
    RETURN
}

public class LoadLevel : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    [SerializeField] private LevelDataObj levelDataObj;
    private GameObject player;
    public Animator transition;
    #endregion

    #region FIELDS
    [SerializeField] private bool useLevelData = false;
    public float transitionTime = 1f;
    #endregion

    #region PROPERTIES
    public bool UseLevelData { get { return useLevelData; } set { useLevelData = value; } }
    #endregion

    public void LoadNextLevel()
    {
        // Save the level before loading
        DataManager.Instance.SaveGame();

        StartCoroutine(LoadingLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadPrevLevel()
    {
        // Save the level before loading
        DataManager.Instance.SaveGame();

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
        // Set player position if necessary
        Debug.Log(levelDataObj.levelPosData[SceneManager.GetActiveScene().name].levelPos);
        switch (levelDataObj.levelPosData[SceneManager.GetActiveScene().name].levelPos)
        {
            case LEVELPOS.ENTER:
                DataManager.Instance.Data.levelData[SceneManager.GetActiveScene().name].playerPosition =
                    levelDataObj.levelPosData[SceneManager.GetActiveScene().name].playerEnterPosition;
                break;

            case LEVELPOS.RETURN:
                DataManager.Instance.Data.levelData[SceneManager.GetActiveScene().name].playerPosition =
                    levelDataObj.levelPosData[SceneManager.GetActiveScene().name].playerReturnPosition;
                break;

            case LEVELPOS.DEFAULT:
                break;
        }

        // Save game after loading as well
        DataManager.Instance.SaveGame();

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
