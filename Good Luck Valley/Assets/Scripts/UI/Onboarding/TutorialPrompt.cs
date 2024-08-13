using GoodLuckValley.World.AreaTriggers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI
{
    public class TutorialPrompt : MonoBehaviour
    {
        #region REFERENCES
        private Text tutorialPromptText;
        private AreaCollider areaCollider;
        #endregion

        #region FIELDS
        [SerializeField] private float fadeTime;
        private bool lockFading;
        private bool queueCoroutine;
        #endregion

        private void Awake()
        {
            // Get component references
            areaCollider = GetComponent<AreaCollider>();
            tutorialPromptText = GetComponent<Text>();
        }

        private void OnEnable()
        {
            // Hook up delegates
            areaCollider.OnTriggerEnter += StartFadeIn;
            areaCollider.OnTriggerExit += StartFadeOut;
        }

        private void OnDisable()
        {
            // Hook up delegates
            areaCollider.OnTriggerEnter -= StartFadeIn;
            areaCollider.OnTriggerExit -= StartFadeOut;
        }

        /// <summary>
        /// Starts the fading in process
        /// </summary>
        /// <param name="collidingObject">Object the area collider collided with</param>
        private void StartFadeIn(GameObject collidingObject)
        {
            // Check if fading is locked (currently fading in/out)
            if (lockFading)
            {
                // If so, then we need to queue a coroutine to start after 
                queueCoroutine = true;
            }
            else
            {
                // If not, start the coroutine now
                StartCoroutine(FadingIn());
            }

        }

        /// <summary>
        /// Starts the fading out process
        /// </summary>
        /// <param name="collidingObject">Object the area collider collided with</param>
        private void StartFadeOut(GameObject collidingObject)
        {
            // Check if fading is locked (currently fading in/out)
            if (lockFading)
            {
                // If so, then we need to queue a coroutine to start after 
                queueCoroutine = true;
            }
            else
            {
                // If not, start the coroutine now
                StartCoroutine(FadingOut());
            }
        }

        /// <summary>
        /// Coroutine to fade out text
        /// </summary>
        private IEnumerator FadingIn()
        {
            // Locks fading because we are fading in
            lockFading = true;

            // Loops until the apha is at 1
            while (tutorialPromptText.color.a < 1.0f)
            {
                // Increment alpha using deltaTime * fadeTime
                tutorialPromptText.color = new Color(tutorialPromptText.color.r, tutorialPromptText.color.g, tutorialPromptText.color.b, tutorialPromptText.color.a + (Time.deltaTime * fadeTime));
                yield return null;
            }

            // Unlocks fading
            lockFading = false;

            // Check if a coroutine is queued
            if (queueCoroutine)
            {
                // No longer queueing a coroutine
                queueCoroutine = false;

                // Start the fading out
                StartCoroutine(FadingOut());
            }
        }

        /// <summary>
        /// Coroutine to fade in text
        /// </summary>
        private IEnumerator FadingOut()
        {
            // Locks fading because we are fading out
            lockFading = true;

            // Loops until the apha is 0
            while (tutorialPromptText.color.a > 0.0f)
            {
                // Increment alpha using deltaTime * fadeTime
                tutorialPromptText.color = new Color(tutorialPromptText.color.r, tutorialPromptText.color.g, tutorialPromptText.color.b, tutorialPromptText.color.a - (Time.deltaTime * fadeTime));
                yield return null;
            }

            // Unlocks fading 
            lockFading = false;

            // Check if a coroutine is queued
            if (queueCoroutine)
            {
                // No longer queueing a coroutine
                queueCoroutine = false;

                // Start fading in
                StartCoroutine(FadingIn());
            }
        }
    }
}
