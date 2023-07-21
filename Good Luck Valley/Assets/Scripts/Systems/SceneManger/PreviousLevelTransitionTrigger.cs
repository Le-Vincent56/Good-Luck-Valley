using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PreviousLevelTransitionTrigger : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private PauseScriptableObj pauseEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private MushroomScriptableObj mushroomEvent;
    [SerializeField] private LevelDataObj levelDataObj;
    [SerializeField] private GameObject levelLoader;
    #endregion

    #region FIELDS
    [SerializeField] private Vector2 movementDirection;
    [SerializeField] private int directionToFace;
    [SerializeField] private string levelToLoad;
    [SerializeField] private string currentLevel;
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
            // End the mushroom throw and clear mushrooms
            mushroomEvent.EndThrow();
            mushroomEvent.ClearShrooms();

            // Reset player turn
            movementEvent.ResetTurn();

            // Lock the player
            disableEvent.DisableInput();

            // Set player movement
            movementEvent.SetTurnDirection(directionToFace);
            movementEvent.SetMovementDirection(movementDirection);
            movementEvent.ApplyMovementDirection();

            // Set inside load triger
            loadLevelEvent.SetInLoadTrigger(true);

            // Set level pos
            levelDataObj.SetLevelPos(levelToLoad, LEVELPOS.RETURN);
            levelDataObj.SetLevelPos(currentLevel, LEVELPOS.ENTER);

            // Set variablesa
            pauseEvent.SetCanPause(false);

            // Set the cutscene event and play it
            cutsceneEvent.SetLeaveCutscene(levelDataObj.levelPosData[currentLevel].exitCutscene);
            cutsceneEvent.StartLeaveCutscene();

            // Trigger footstep sounds
            movementEvent.StartCutsceneFootstepEvent();
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
        Debug.Log("Trigger Load");
        // Set variables
        loadLevelEvent.SetInLoadTrigger(true);
        loadLevelEvent.SetLoadingThroughCutscene(true);
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
