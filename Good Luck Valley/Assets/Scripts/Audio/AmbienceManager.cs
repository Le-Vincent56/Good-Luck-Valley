using GoodLuckValley.Audio.Sound;
using GoodLuckValley.Patterns.Singletons;
using UnityEngine;

public class AmbienceManager : PersistentSingleton<AmbienceManager>
{
    [Header("Fields - General Ambience")]
    [SerializeField] private SoundData ambience;

    [Header("Fields - Bird Ambience")]
    [SerializeField] private float minBirdWait;
    [SerializeField] private float maxBirdWait;
    [SerializeField] private float ambientBirdTimer;
    [SerializeField] private SoundData ambientBirdSFX;

    [Header("Fields - Insect Ambience")]
    [SerializeField] private float minInsectWait;
    [SerializeField] private float maxInsectWait;
    [SerializeField] private float ambientInsectTimer;
    [SerializeField] private SoundData ambientInsectSFX;

    [Header("Fields - Tree Ambience")]
    [SerializeField] private float minTreeWait;
    [SerializeField] private float maxTreeWait;
    [SerializeField] private float ambientTreeTimer;
    [SerializeField] private SoundData ambientTreeSFX;

    [Header("Fields - Wind Ambience")]
    [SerializeField] private float minWindWait;
    [SerializeField] private float maxWindWait;
    [SerializeField] private float ambientWindTimer;
    [SerializeField] private SoundData ambientWindSFX;

    private void Start()
    {
        // Set initial timers
        ambientBirdTimer = Random.Range(minBirdWait * 60f, maxBirdWait * 60f);
        ambientInsectTimer = Random.Range(minInsectWait * 60f, maxInsectWait * 60f);
        ambientTreeTimer = Random.Range(minTreeWait * 60f, maxTreeWait * 60f);
        ambientWindTimer = Random.Range(minWindWait * 60f, maxWindWait * 60f);

        // Play the general ambience
        SoundManager.Instance.CreateSoundBuilder()
            .WithSoundData(ambience)
            .Play();
    }

    private void Update()
    {
        // Check if the ambient bird timer has run out
        if (ambientBirdTimer <= 0)
        {
            // Set the new time for the ambient bird timer
            ambientBirdTimer = Random.Range(minBirdWait * 60f, maxBirdWait * 60f);

            // Play an ambient bird sound
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(ambientBirdSFX)
                .Play();
        }
        else
        {
            // Decrement the ambient bird timer
            ambientBirdTimer -= Time.deltaTime;
        }

        // Check if the ambient insect timer has run out
        if (ambientInsectTimer <= 0)
        {
            // Set the new time for the ambient insect timer
            ambientInsectTimer = Random.Range(minInsectWait * 60f, maxInsectWait * 60f);

            // Play an ambient insect sound
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(ambientInsectSFX)
                .Play();
        }
        else
        {
            // Decrement the ambient insect timer
            ambientInsectTimer -= Time.deltaTime;
        }

        // Check if the ambient tree timer has run out
        if (ambientTreeTimer <= 0)
        {
            // Set the new time for the ambient tree timer
            ambientTreeTimer = Random.Range(minTreeWait * 60f, maxTreeWait * 60f);

            // Play an ambient tree sound
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(ambientTreeSFX)
                .Play();
        }
        else
        {
            // Decrement the ambient tree timer
            ambientTreeTimer -= Time.deltaTime;
        }

        // Check if the ambient wind timer has run out
        if (ambientWindTimer <= 0)
        {
            // Set the new time for the ambient wind timer
            ambientWindTimer = Random.Range(minWindWait * 60f, maxWindWait * 60f);

            // Play an ambient wind sound
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(ambientWindSFX)
                .Play();
        }
        else
        {
            // Decrement the ambient wind timer
            ambientWindTimer -= Time.deltaTime;
        }
    }
}
