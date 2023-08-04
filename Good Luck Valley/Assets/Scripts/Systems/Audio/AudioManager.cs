using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UIElements;

public enum MusicArea
{
    MAIN_MENU = 0,
    FOREST = 1
}

public enum ForestLevel
{
    MAIN = 0,
    END = 1
}

public enum ForestLayer
{
    FOREST_0,
    FOREST_1,
    FOREST_2,
    FOREST_3,
    FOREST_4,
    FOREST_5,
    FOREST_6,
    FOREST_7
}

public class AudioManager : MonoBehaviour, IData
{
    #region FIELDS
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    private EventInstance lotusPulseEventInstance;

    [Header("Areas")]
    [SerializeField] private MusicArea currentArea;
    [SerializeField] private MusicArea lastArea;

    [Header("Forest")]
    [SerializeField] private ForestLevel currentForestLevel;
    [SerializeField] private SerializableDictionary<string, float> forestLayers;

    #region TIMED AMBIENT NOISES
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

    [Header("Tree Noises")]
    [SerializeField] private bool playingTreeNoise = false;
    [SerializeField] private float treeAmbientCooldown = -1f;
    [SerializeField] private float minTreeWait = 0.5f;
    [SerializeField] private float maxTreeWait = 2f;

    [Header("Wind Noises")]
    [SerializeField] private bool playingWindNoise = false;
    [SerializeField] private float windAmbientCooldown = -1f;
    [SerializeField] private float minWindWait = 0.5f;
    [SerializeField] private float maxWindWait = 2f;
    #endregion

    #region VOLUME CONTROL
    public Bus masterBus;
    public Bus musicBus;
    public Bus ambienceBus;
    public Bus sfxBus;
    #endregion
    #endregion

    #region PROPERTIES
    public static AudioManager Instance { get; private set; }
    public List<EventInstance> EventInstances { get { return eventInstances; } }
    public EventInstance MusicEventInstance { get { return musicEventInstance; } }
    public EventInstance AmbienceEventInstance { get { return ambienceEventInstance; } }

    public EventInstance LotusPulseEventInstance { get { return lotusPulseEventInstance; } }
    public MusicArea CurrentArea { get { return currentArea; } }
    public ForestLevel CurrentForestLevel { get { return currentForestLevel; } }
    public SerializableDictionary<string, float> ForestLayers { get { return forestLayers; } set { forestLayers = value; } }
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
        lotusPulseEventInstance = CreateEventInstance(FMODEvents.Instance.LotusPulse);
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
        switch (SceneManager.GetActiveScene().name)
        {
            case "Title Screen":
                // Set the music area
                SetMusicArea(MusicArea.MAIN_MENU);
                break;

            case "Main Menu":
                // Set the music area
                SetMusicArea(MusicArea.MAIN_MENU);
                break;

            case "Prologue":
                // Set the music area
                SetMusicArea(MusicArea.FOREST);

                // Set default values, if they're not set already
                if (currentForestLevel != ForestLevel.MAIN)
                {
                    SetForestLevel(ForestLevel.MAIN);
                }

                // Set each music parameter
                foreach(KeyValuePair<string, float> layer in forestLayers)
                {
                    musicEventInstance.setParameterByName(layer.Key, layer.Value);
                }
                break;

            case "Level 1":
                // Set the music area
                SetMusicArea(MusicArea.FOREST);

                // Set default values, if they're not set already
                if (currentForestLevel != ForestLevel.MAIN)
                {
                    SetForestLevel(ForestLevel.MAIN);
                }

                // Set each music parameter
                foreach (KeyValuePair<string, float> layer in forestLayers)
                {
                    musicEventInstance.setParameterByName(layer.Key, layer.Value);
                }
                break;
        }

        // Stop all music if changing places to prevent overlap
        if (currentArea != lastArea)
        {
            musicBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        switch(currentArea)
        {
            case MusicArea.MAIN_MENU:
                // Stop all coroutines - ambient sounds
                StopAllCoroutines();
                
                // Stop ambient sounds
                ambienceBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                // Stop lotus pulses
                LotusPulseEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                break;

            case MusicArea.FOREST:
                if(!CheckInstancePlaying(ambienceEventInstance))
                {
                    ambienceEventInstance.start();
                }

                // Set ambient cooldowns
                cricketAmbientCooldown = Random.Range(minCricketWait * 60, maxCricketWait * 60);
                birdAmbientCooldown = Random.Range(minBirdWait * 60, maxBirdWait * 60);
                treeAmbientCooldown = Random.Range(minTreeWait * 60, maxTreeWait * 60);
                windAmbientCooldown = Random.Range(minWindWait * 60, maxWindWait * 60);

                // Start ambient coroutines
                StartCoroutine(CheckCricketSounds());
                StartCoroutine(CheckBirdSounds());
                StartCoroutine(CheckTreeSounds());
                StartCoroutine(CheckWindSounds());
                break;
        }

        // Start the music if music had been stopped
        if (currentArea != lastArea)
        {
            musicEventInstance.start();
        }

        // Set lastArea to currentArea
        lastArea = currentArea;
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
        if(forestLayers.Count == 0)
        {
            forestLayers = new SerializableDictionary<string, float>()
            {
                {"FOREST_0", 1.00f },
                {"FOREST_1", 0.00f },
                {"FOREST_2", 0.00f },
                {"FOREST_3", 0.00f },
                {"FOREST_4", 0.00f },
                {"FOREST_5", 0.00f },
                {"FOREST_6", 0.00f },
                {"FOREST_7", 0.00f },
            };
        }

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
    /// Set the music associated with the forest level
    /// </summary>
    /// <param name="level">The forest level to associate the music with</param>

    public void SetForestLevel(ForestLevel level)
    {
        currentForestLevel = level;
        musicEventInstance.setParameterByName("ForestLevel", (float)level);
    }

    /// <summary>
    /// Set a certain forest layer to a specific value
    /// </summary>
    /// <param name="parameterName">Name of the layer parameter</param>
    /// <param name="value">The value of the parameter</param>
    public void SetForestLayer(string parameterName, float value)
    {
        // Set the forest layer
        forestLayers[parameterName] = value;
        musicEventInstance.setParameterByName(parameterName, value);
    }

    /// <summary>
    /// Set whether the forest music should end or not
    /// </summary>
    /// <param name="value">0f if false, 0f if true</param>
    public void SetForestEnd(float value)
    {
        musicEventInstance.setParameterByName("FOREST_END", value);
    }

    /// <summary>
    /// Dampen the music by a percentage
    /// </summary>
    /// <param name="percentage">The percentage to dampen the music by, 0 is no dapmen, 1 is fully dampened</param>
    public void DampenMusic(float percentage)
    {
        RuntimeManager.StudioSystem.setParameterByName("Dampen", percentage);
    }

    public float GetDampen()
    {
        float dampenLevel;
        RuntimeManager.StudioSystem.getParameterByName("Dampen", out dampenLevel);
        return dampenLevel;
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
        int soundIndex = (int)Random.Range(0f, sounds.Count);

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

        // If the list doesn't contain the event instance, add it
        if (!eventInstances.Contains(eventInstance))
        {
            eventInstances.Add(eventInstance);
            return eventInstance;
        } else
        {
            // If it already exists, give them the current event instance instead of creating a new one
            return eventInstances[eventInstances.IndexOf(eventInstance)];
        }
        
    }

    /// <summary>
    /// Set the master volume
    /// </summary>
    /// <param name="volumePercentage">The percent of volume as a float between 0 and 1</param>
    public void SetMasterVolume(float volumePercentage)
    {
        masterBus.setVolume(volumePercentage);
    }

    /// <summary>
    /// Set the music volume
    /// </summary>
    /// <param name="volumePercentage">The percent of volume as a float between 0 and 1</param>
    public void SetMusicVolume(float volumePercentage)
    {
        musicBus.setVolume(volumePercentage);
    }

    /// <summary>
    /// Set the ambience volume
    /// </summary>
    /// <param name="volumePercentage">The percent of volume as a float between 0 and 1</param>
    public void SetAmbienceVolume(float volumePercentage)
    {
        ambienceBus.setVolume(volumePercentage);
    }

    /// <summary>
    /// Set the SFX volume
    /// </summary>
    /// <param name="volumePercentage">The percent of volume as a float between 0 and 1</param>
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

    private IEnumerator CheckTreeSounds()
    {
        while (true)
        {
            if (!playingTreeNoise)
            {
                // Wait for a certain amount of seconds in real time
                yield return new WaitForSecondsRealtime(treeAmbientCooldown);

                // Set playingBirdNoise
                playingTreeNoise = true;
            }
            else
            {
                // Play a random bird call
                PlayRandomizedOneShot(FMODEvents.Instance.TreeSettles, transform.position);

                // Set the cooldown - multiply by 60 to transfer minutes to seconds
                treeAmbientCooldown = Random.Range(minTreeWait * 60, maxTreeWait * 60);

                // Rseet playingBirdNoise
                playingTreeNoise = false;
            }

            // Allow other code to run
            yield return null;
        }
    }

    private IEnumerator CheckWindSounds()
    {
        while (true)
        {
            if (!playingWindNoise)
            {
                // Wait for a certain amount of seconds in real time
                yield return new WaitForSecondsRealtime(windAmbientCooldown);

                // Set playingBirdNoise
                playingWindNoise = true;
            }
            else
            {
                // Play a random bird call
                PlayRandomizedOneShot(FMODEvents.Instance.Wind, transform.position);

                // Set the cooldown - multiply by 60 to transfer minutes to seconds
                windAmbientCooldown = Random.Range(minWindWait * 60, maxWindWait * 60);

                // Rseet playingBirdNoise
                playingWindNoise = false;
            }

            // Allow other code to run
            yield return null;
        }
    }

    #region DATA HANDLING

    public void LoadData(GameData data)
    {
        currentForestLevel = data.currentForestLevel;
        if(data.forestLayers.Count != 0)
        {
            forestLayers = data.forestLayers;
        }
    }

    public void SaveData(GameData data)
    {
        data.currentForestLevel = currentForestLevel;
        data.forestLayers = forestLayers;
    }
    #endregion
}
