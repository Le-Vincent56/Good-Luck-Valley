using FMOD.Studio;
using HiveMind.Audio;
using HiveMind.Environment;
using HiveMind.Events;
using HiveMind.SaveData;
using HiveMind.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HiveMind.Interactables
{
    public class LotusPick : Interactable, IData
    {
        #region REFERENCES
        [SerializeField] private PauseScriptableObj pauseEvent;
        [SerializeField] private DisableScriptableObj disableEvent;
        [SerializeField] private PostProcessingScriptableObj postProcessingEvent;
        [SerializeField] GameObject vineWall;
        private VisualEffect lotusParticles;
        #endregion

        #region FIELDS
        [SerializeField] private float interactRange;
        [SerializeField] private float fadeAmount;
        [SerializeField] private float progressLevel;
        [SerializeField] private float playerDistance;
        [SerializeField] private bool lotusFinished;

        [Header("Sound")]
        [SerializeField] private bool progressesMusic;
        [SerializeField] private float maxSoundDistance;
        [SerializeField] private float soundPercentage;
        [SerializeField] private bool soundUnDampened;

        [Header("Lotus Vignette")]
        [SerializeField] private VolumeProfile lotusProfile;

        [Range(0.181f, 1f)]
        [SerializeField] private float maxVignetteIntensity = 0.520f;
        
        #endregion

        void Start()
        {
            gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Thickness", 0.02f);
            lotusParticles = GetComponentInChildren<VisualEffect>();

            remove = false;
            if (fadeAmount == 0)
            {
                fadeAmount = 0.01f;
            }

            // Get the radius of the CircleCollider2D
            float colliderRadius = GetComponent<CircleCollider2D>().radius;

            // Calculate the vector from the object's position to the center of the collider
            maxSoundDistance = GetComponent<CircleCollider2D>().bounds.extents.x;
        }

        void Update()
        {
            // Show the outline if in range
            if (inRange)
            {
                gameObject.GetComponent<SpriteRenderer>().material.SetInt("_Active", 1);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().material.SetInt("_Active", 0);
            }

            // Debug.Log(vineWall.activeSelf);
            // Check if interactable is triggered
            if (controlTriggered)
            {
                // Interact and set variables
                if (!interacting)
                {
                    Interact();
                }

                interacting = true;

                // If the inteaction has finished, reset the variables
                if (lotusFinished && soundUnDampened)
                {
                    controlTriggered = false;

                    StopAllCoroutines();

                    disableEvent.Unlock();
                }
            }
            else
            {
                // If the control is not triggered, set interacting to false
                interacting = false;
            }
        }

        /// <summary>
        /// Disable Lotus tutorial text and end the level
        /// </summary>
        public override void Interact()
        {
            // Save the game
            DataManager.Instance.SaveGame();

            // Lock the player
            disableEvent.Lock();

            // Progress music
            if (progressesMusic)
            {
                AudioManager.Instance.SetForestLayer("FOREST_END", 1f);
            }

            // Start the lotus pick by playing the sound
            StartCoroutine(PlayLotusSounds());
            playedSound = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player" && active)
            {
                // Check distance between lotus and player
                playerDistance = CalculateDistance(collision);

                // Start the lotus sound
                PLAYBACK_STATE playbackStatePulse;
                AudioManager.Instance.LotusPulseEventInstance.getPlaybackState(out playbackStatePulse);
                if (playbackStatePulse.Equals(PLAYBACK_STATE.STOPPED))
                {
                    // If so, start it
                    AudioManager.Instance.LotusPulseEventInstance.start();
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // Check if the collider is the player
            if (collision.gameObject.tag == "Player" && active)
            {
                // Check distance between lotus and player
                playerDistance = CalculateDistance(collision);

                // Check if the player is in range
                if (playerDistance < interactRange)
                {
                    inRange = true;
                }
                else
                {
                    inRange = false;
                }
                // Calculate the sound percentage for the pulse
                soundPercentage = playerDistance / maxSoundDistance;

                // Set the distance parameter for adaptive sound
                AudioManager.Instance.LotusPulseEventInstance.setParameterByName("LotusDistance", soundPercentage);

                // Dampen other sounds to really hone in on the effect
                AudioManager.Instance.DampenMusic(Mathf.Clamp(1f - soundPercentage, 0f, 1f));

                // Change the vignette
                Vignette vignette;
                if(lotusProfile.TryGet(out vignette))
                {
                    float currentIntensity = Mathf.Clamp(1f - soundPercentage, postProcessingEvent.GetVignetteDefaultIntensity(), vignette.intensity.value);
                    Color currentColor = postProcessingEvent.GetVignetteColor();
                    Color colorToSet = currentColor;

                    if (currentColor.r < vignette.color.value.r)
                    {
                        colorToSet.r = vignette.color.value.r * (1f - soundPercentage);
                    }

                    if (currentColor.g < vignette.color.value.g)
                    {
                        colorToSet.g = vignette.color.value.g * (1f - soundPercentage);
                    }

                    if (currentColor.b < vignette.color.value.b)
                    {
                        colorToSet.b = vignette.color.value.b * (1f - soundPercentage);
                    }

                    if(currentColor.a < vignette.color.value.a)
                    {
                        colorToSet.a = vignette.color.value.a * (1f - soundPercentage);
                    }

                    postProcessingEvent.SetVignetteSmoothness(Mathf.Clamp(1f - soundPercentage, postProcessingEvent.GetVignetteDefaultSmoothness(), vignette.smoothness.value));
                    postProcessingEvent.SetVignetteColor(colorToSet);
                    postProcessingEvent.SetVignetteIntensity(currentIntensity);
                    postProcessingEvent.ChangeVignette(colorToSet, collision.gameObject.transform.position, currentIntensity, true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player" && active)
            {
                PLAYBACK_STATE playbackStatePulse;
                AudioManager.Instance.LotusPulseEventInstance.getPlaybackState(out playbackStatePulse);
                if (!playbackStatePulse.Equals(PLAYBACK_STATE.STOPPED))
                {
                    // If so, start it
                    AudioManager.Instance.LotusPulseEventInstance.stop(STOP_MODE.IMMEDIATE);
                }

                // Reset the vignette
                postProcessingEvent.ResetVignette();
            }
        }

        public float CalculateDistance(Collider2D other)
        {
            Vector3 lotusCenter = transform.position;
            Vector3 playerCenter = other.gameObject.transform.position;
            return Vector3.Distance(playerCenter, lotusCenter);
        }

        private IEnumerator FadeVines()
        {
            pauseEvent.SetPaused(true);
            while (vineWall.GetComponent<Tilemap>().color.a > 0)
            {
                vineWall.GetComponent<Tilemap>().color = new Color(vineWall.GetComponent<Tilemap>().color.r,
                    vineWall.GetComponent<Tilemap>().color.g, vineWall.GetComponent<Tilemap>().color.b,
                    vineWall.GetComponent<Tilemap>().color.a - fadeAmount);

                yield return null;
            }
            vineWall.GetComponent<DecomposableVine>().Active = false;
            vineWall.SetActive(false);
        }

        private IEnumerator FadeLotus()
        {
            // While alpha values are under the desired numbers, increase them by an unscaled delta time (because we are paused)
            while (GetComponent<SpriteRenderer>().color.a > 0)
            {
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, GetComponent<SpriteRenderer>().color.a - fadeAmount);
                yield return null;
            }

            // Set active to false
            active = false;
            lotusParticles.enabled = false;
            yield return null;
        }

        private IEnumerator FadeEndScreen(GameObject endScreen)
        {
            while (endScreen.GetComponent<Text>().color.a <= 1)
            {
                endScreen.GetComponent<Text>().color = new Color(1, 1, 1, endScreen.GetComponent<Text>().color.a + fadeAmount);
                endScreen.GetComponentInChildren<Image>().color = new Color(1, 1, 1, endScreen.GetComponentInChildren<Image>().color.a + fadeAmount);
                endScreen.GetComponentInChildren<Image>().GetComponentInChildren<Text>().color = new Color(1, 1, 1, endScreen.GetComponentInChildren<Image>().GetComponentInChildren<Text>().color.a + fadeAmount);
                GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color.a + fadeAmount);

                if (GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color.a >= 0.85)
                {
                    GameObject.Find("EndSquare").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, .85f);
                }
                yield return null;
            }
            Debug.Log("Fade End Screen Finish");
            lotusFinished = true;
        }

        private IEnumerator PlayPickSound()
        {
            // Play lotus pick sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.LotusPick, transform.position);

            // Wait until the "pick" noise
            yield return new WaitForSeconds(2.5f);

            // Start the fading coroutines
            StartCoroutine(FadeVines());
            StartCoroutine(FadeLotus());

            yield return StartCoroutine(UndampenSound());

            GameObject endScreen = GameObject.Find("Demo Ending Text");
            if (endScreen != null)
            {
                finishedInteracting = false;
                endScreen.GetComponentInChildren<Button>().interactable = true;
                yield return StartCoroutine(FadeEndScreen(endScreen));
            }
            else
            {
                disableEvent.Unlock();
                pauseEvent.SetPaused(false);
                finishedInteracting = true;
            }
        }

        private IEnumerator PlayLotusSounds()
        {
            // Play the pick sound
            yield return StartCoroutine(PlayPickSound());

            // Play vine sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.VineDecompose, transform.position);

            // Return
            yield break;
        }

        private IEnumerator UndampenSound()
        {
            // Remove the vignette
            StartCoroutine(RemoveVignette());

            // Undampen the sound while it is dampened
            while (AudioManager.Instance.GetDampen() > 0)
            {
                // Let other code run
                yield return null;

                // Undampen the music
                AudioManager.Instance.DampenMusic(AudioManager.Instance.GetDampen() - (Time.deltaTime * 3));
            }

            // Check if pulse is still playing
            PLAYBACK_STATE playbackStatePulse;
            AudioManager.Instance.LotusPulseEventInstance.getPlaybackState(out playbackStatePulse);
            Debug.Log("Getting pulse: " + playbackStatePulse.ToString());
            if (!playbackStatePulse.Equals(PLAYBACK_STATE.STOPPED))
            {
                // If so, stop it
                AudioManager.Instance.LotusPulseEventInstance.stop(STOP_MODE.IMMEDIATE);
            }
        }

        private IEnumerator RemoveVignette()
        {
            while (postProcessingEvent.GetVignetteIntensity() > postProcessingEvent.GetVignetteDefaultIntensity()
                || postProcessingEvent.GetVignetteSmoothness() > postProcessingEvent.GetVignetteDefaultSmoothness()
                || postProcessingEvent.GetVignetteColor().r > postProcessingEvent.GetVignetteDefaultColor().r
                || postProcessingEvent.GetVignetteColor().g > postProcessingEvent.GetVignetteDefaultColor().g
                || postProcessingEvent.GetVignetteColor().b > postProcessingEvent.GetVignetteDefaultColor().b
                || postProcessingEvent.GetVignetteColor().a > postProcessingEvent.GetVignetteDefaultColor().a)
            {
                yield return null;

                float currentIntensity = postProcessingEvent.GetVignetteIntensity() - (Time.deltaTime * 3f);
                Color currentColor = postProcessingEvent.GetVignetteColor();
                Color colorToSet = new Color(currentColor.r - (Time.deltaTime * 3f), currentColor.g - (Time.deltaTime * 3f), currentColor.b - (Time.deltaTime * 3f), currentColor.a - (Time.deltaTime * 3f));
                postProcessingEvent.SetVignetteSmoothness(Mathf.Clamp(postProcessingEvent.GetVignetteSmoothness() - (Time.deltaTime * 3f), 0.2f, 1.0f));
                postProcessingEvent.SetVignetteColor(colorToSet);
                postProcessingEvent.SetVignetteIntensity(Mathf.Clamp(currentIntensity, 0f, 1f));

                postProcessingEvent.ChangeVignette(colorToSet, transform.position, currentIntensity, true);
            }
        }

        #region DATA HANDLING
        public void LoadData(GameData data)
        {
            // Get the data for all the notes that have been collected
            string currentLevel = SceneManager.GetActiveScene().name;

            // Try to get the value of the interactable
            data.levelData[currentLevel].assetsActive.TryGetValue(id, out active);

            // Check if the note has been added
            if (!active)
            {
                // Remove the note
                remove = true;
            }
            // Set if the gameobject is active
            gameObject.SetActive(active);
        }

        public void SaveData(GameData data)
        {
            string currentLevel = SceneManager.GetActiveScene().name;

            // Check to see if data has the id of the note
            if (data.levelData[currentLevel].assetsActive.ContainsKey(id))
            {
                // If so, remove it
                data.levelData[currentLevel].assetsActive.Remove(id);
            }

            // Add the id and the current bool to make sure everything is up to date
            data.levelData[currentLevel].assetsActive.Add(id, active);
        }
        #endregion
    }
}
