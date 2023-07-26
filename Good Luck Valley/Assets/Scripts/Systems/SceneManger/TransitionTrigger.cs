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
    [SerializeField] private bool progressesMusic;
    [SerializeField] private float progressLevel;
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
            // Disable HUD
            disableEvent.DisableHUD();

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

            // Set level pos
            levelDataObj.SetLevelPos(levelToLoad, LEVELPOS.ENTER);
            levelDataObj.SetLevelPos(currentLevel, LEVELPOS.RETURN);

            // Set inside load trigger
            loadLevelEvent.SetInLoadTrigger(true);

            // Trigger any music loading
            if (progressesMusic)
            {
                loadLevelEvent.StartMusicLoad(progressLevel);
            }

            // Set variables
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
        // Set variables
        loadLevelEvent.SetInLoadTrigger(true);
        loadLevelEvent.SetLoadingThroughCutscene(true);
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
