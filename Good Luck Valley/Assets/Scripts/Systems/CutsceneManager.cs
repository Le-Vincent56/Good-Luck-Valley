using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    [SerializeField] private PlayableDirector playerDirector;
    [SerializeField] private DisableScriptableObj disableEvent;
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    #endregion

    private void OnEnable()
    {
        cutsceneEvent.startEnterCutscene.AddListener(PlayEnterCutscene);
        cutsceneEvent.endEnterCutscene.AddListener(EndEnterCutscene);
        cutsceneEvent.startLeaveCutscene.AddListener(PlayLeaveCutscene);
    }

    private void OnDisable()
    {
        cutsceneEvent.startEnterCutscene.RemoveListener(PlayEnterCutscene);
        cutsceneEvent.endEnterCutscene.RemoveListener(EndEnterCutscene);
        cutsceneEvent.startLeaveCutscene.RemoveListener(PlayLeaveCutscene);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerDirector = GetComponent<PlayableDirector>();
    }

    /// <summary>
    /// Play the enter cutscene for a level
    /// </summary>
    private void PlayEnterCutscene()
    {
        disableEvent.SetDisableParallax(true);

        // Set playing cutscene to true
        cutsceneEvent.SetPlayingCutscene(true);

        // Lock the player
        disableEvent.DisableInput();

        // Set the player director asset and play it
        playerDirector.playableAsset = cutsceneEvent.GetEnterCutscene();
        playerDirector.Play();
    }

    /// <summary>
    /// End the enter cutscene for a level
    /// </summary>
    private void EndEnterCutscene()
    {
        disableEvent.SetDisableParallax(false);

        // Save the game
        DataManager.Instance.SaveGame();

        // Enable input
        disableEvent.EnableInput();

        // Set playing cutscene to false
        cutsceneEvent.SetPlayingCutscene(false);
    }

    /// <summary>
    /// Play the level cutscene for a level
    /// </summary>
    private void PlayLeaveCutscene()
    {
        disableEvent.SetDisableParallax(true);

        // Save the game
        DataManager.Instance.SaveGame();

        // Set playing cutscene to true
        cutsceneEvent.SetPlayingCutscene(true);

        // Set the player director asset and play it
        playerDirector.playableAsset = cutsceneEvent.GetLeaveCutscene();
        playerDirector.Play();
    }
}
