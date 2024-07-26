using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Singletons;
using UnityEngine;

public class PauseManager : PersistentSingleton<PauseManager>
{
    [Header("Events")]
    [SerializeField] private GameEvent onPause;

    [Header("Fields")]
    [SerializeField] private bool isPaused;
    [SerializeField] private bool isSoftPaused;

    public bool IsPaused 
    { 
        get => isPaused; 
        private set => isPaused = value; 
    }
    public bool IsSoftPaused 
    { 
        get => isSoftPaused; 
        private set => isSoftPaused = value; 
    }

    private void Start()
    {
        // Set paused and soft-paused to false
        IsPaused = false;
        IsSoftPaused = false;
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        // Raise the paused event
        // Calls to:
        //  - PowerController.OnCancelThrow();
        onPause.Raise(this, null);
    }

    /// <summary>
    /// Unpause the game
    /// </summary>
    public void UnpauseGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Soft-pause the game (pause the game without raising the UI menu)
    /// </summary>
    public void SoftPauseGame()
    {
        IsSoftPaused = true;
        Time.timeScale = 0f;

        // Raise the paused event
        // Calls to:
        //  - PowerController.OnCancelThrow();
        onPause.Raise(this, null);
    }

    /// <summary>
    /// Unsoft-pause the game
    /// </summary>
    public void SoftUnpauseGame()
    {
        IsSoftPaused = false;
        Time.timeScale = 1f;
    }
}
