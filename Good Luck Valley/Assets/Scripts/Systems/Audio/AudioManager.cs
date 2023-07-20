using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public enum MusicArea
{
    MAIN_MENU = 0,
    FOREST = 1
}

public class AudioManager : MonoBehaviour
{
    #region FIELDS
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    [SerializeField] private MusicArea currentArea;

    [Header("Cricket Noises")]
    [SerializeField] private bool playingCricketNoise = false;
    [SerializeField] private float cricketAmbientCooldown = -1f;
    [SerializeField] private float minCricketWait = 0.1f;
    [SerializeField] private float maxCricketWait = 1f;

    [Header("Bird Noises")]
    [SerializeField] private bool playingBirdNoise = false;
    [SerializeField] private float birdAmbientCooldown = -1f;
    [SerializeField] private float minBirdWait = 0.1f;
    [SerializeField] private float maxBirdWait = 1f;

    #region VOLUME CONTROL
    public Bus masterBus;
    public Bus musicBus;
    public Bus ambienceBus;
    public Bus sfxBus;
    #endregion
    #endregion

    #region PROPERTIES
    public static AudioManager Instance { get; private set; }
    public EventInstance MusicEventInstance { get { return musicEventInstance; } }
    public EventInstance AmbienceEventInstance { get { return ambienceEventInstance; } }
    public MusicArea CurrentArea { get { return currentArea; } }
    #endregion

    private void Awake()
    {
        // Check if there's already an AudioManager
        if (Instance != null)
        {
            // If there is, destroy this one to retain singleton design
            Debug.LogWarning("Found more than one Audio Manager in the scene. Destroying the newest one");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Don't destroy audio management on load
        DontDestroyOnLoad(gameObject);

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initialize ambience and music
        InitializeMusic(FMODEvents.Instance.GameMusic);
        InitializeAmbience(FMODEvents.Instance.Ambience);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Get Music Area based on scene
    /// </summary>
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "Title Screen":
                SetMusicArea(MusicArea.MAIN_MENU);
                break;

            case "Main Menu":
                SetMusicArea(MusicArea.MAIN_MENU);
                break;

            case "Prologue":
                SetMusicArea(MusicArea.FOREST);
                break;

            case "Level 1":
                SetMusicArea(MusicArea.FOREST);
                break;
        }

        switch(currentArea)
        {
            case MusicArea.MAIN_MENU:
                // Stop all coroutines - ambient sounds
                StopAllCoroutines();
                break;

            case MusicArea.FOREST:
                // Set ambient cooldowns
                cricketAmbientCooldown = Random.Range(minCricketWait * 60, maxCricketWait * 60);
                birdAmbientCooldown = Random.Range(minBirdWait * 60, maxBirdWait * 60);

                // Start ambient coroutines
                StartCoroutine(CheckCricketSounds());
                StartCoroutine(CheckBirdSounds());
                break;
        }
    }

    /// <summary>
    /// Create an Ambience EventInstance and start it
    /// </summary>
    /// <param name="ambienceEventReference">The ambience EventReference</param>
    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    /// <summary>
    /// Initialize a Music EventInstance and start it
    /// </summary>
    /// <param name="musicEventReference"></param>
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
    }

    /// <summary>
    /// Set the music associated with an area
    /// </summary>
    /// <param name="area">The area to set the music for</param>
    public void SetMusicArea(MusicArea area)
    {
        currentArea = area;
        musicEventInstance.setParameterByName("Area", (float)area);
        ambienceEventInstance.setParameterByName("Area", (float)area);
    }

    /// <summary>
    /// Play a one shot sound
    /// </summary>
    /// <param name="sound">Sound to play</param>
    /// <param name="worldPos">Where to play it</param>
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        // From FMOD Unity
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    /// <summary>
    /// Play a random one shot sound out of a list of sounds
    /// </summary>
    /// <param name="sounds">Sound to play</param>
    /// <param name="worldPos">Where to play it</param>
    public void PlayRandomizedOneShot(List<EventReference> sounds, Vector3 worldPos)
    {
        // Get a random number in range of the list indexes
        int soundIndex = (int)Random.Range(0f, 4f);

        // Play one of the one shots within the list
        RuntimeManager.PlayOneShot(sounds[soundIndex], worldPos);
    }

    /// <summary>
    /// Create an Instance of an FMOD Event using an EventReference and add it to the list
    /// </summary>
    /// <param name="eventReference">The EventReference to create an EventInstance from</param>
    /// <returns>An EventInstance created from an EventReference</returns>
    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void SetMasterVolume(float volumePercentage)
    {
        masterBus.setVolume(volumePercentage);
    }

    public void SetMusicVolume(float volumePercentage)
    {
        musicBus.setVolume(volumePercentage);
    }

    public void SetAmbienceVolume(float volumePercentage)
    {
        ambienceBus.setVolume(volumePercentage);
    }

    public void SetSFXVolume(float volumePercentage)
    {
        sfxBus.setVolume(volumePercentage);
    }

    private bool CheckInstancePlaying(EventInstance instance)
    {
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != PLAYBACK_STATE.STOPPED;
    }

    private IEnumerator CheckCricketSounds()
    {
        while(true)
        {
            if (!playingCricketNoise)
            {
                // Wait for a certain amount of seconds in real time
                yield return new WaitForSecondsRealtime(cricketAmbientCooldown);

                // Set playingCricketNoise
                playingCricketNoise = true;
            }
            else
            {
                // Play a random cricket noise
                PlayRandomizedOneShot(FMODEvents.Instance.Crickets, transform.position);

                // Set the cooldown - multiply by 60 to transfer minutes to seconds
                cricketAmbientCooldown = Random.Range(minCricketWait * 60, maxCricketWait * 60);

                // Reset playingCricketNoise
                playingCricketNoise = false;
            }

            // Allow other code to run
            yield return null;
        }
    }

    private IEnumerator CheckBirdSounds()
    {
        while(true)
        {
            if (!playingBirdNoise)
            {
                // Wait for a certain amount of seconds in real time
                yield return new WaitForSecondsRealtime(birdAmbientCooldown);

                // Set playingBirdNoise
                playingBirdNoise = true;
            }
            else
            {
                // Play a random bird call
                PlayRandomizedOneShot(FMODEvents.Instance.BirdCalls, transform.position);

                // Set the cooldown - multiply by 60 to transfer minutes to seconds
                birdAmbientCooldown = Random.Range(minBirdWait * 60, maxBirdWait * 60);

                // Rseet playingBirdNoise
                playingBirdNoise = false;
            }

            // Allow other code to run
            yield return null;
        }
        
    }
}
