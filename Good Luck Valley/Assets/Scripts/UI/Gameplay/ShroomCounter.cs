using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HiveMind.Events;

namespace HiveMind.UI
{
    public class ShroomCounter : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] UIScriptableObj UIEvent;
        [SerializeField] DisableScriptableObj disableEvent;
        [SerializeField] MushroomScriptableObj mushroomEvent;
        private GameObject shroomIcon1;
        private GameObject shroomIcon2;
        private GameObject shroomIcon3;
        private Image shroomOutline1;
        private Image shroomOutline2;
        private Image shroomOutline3;
        private Image shroomFill1;
        private Image shroomFill2;
        private Image shroomFill3;
        #endregion

        #region FIELDS
        private float originalR;
        private float originalG;
        private float originalB;
        private float newR;
        private float newG;
        private float newB;
        private List<GameObject> shroomIconList;
        private bool displayCounter;
        #endregion

        #region PROPERTIES
        public GameObject ShroomIcon1 { get { return shroomIcon1; } }
        public GameObject ShroomIcon2 { get { return shroomIcon2; } }
        public GameObject ShroomIcon3 { get { return shroomIcon3; } }
        public List<GameObject> ShroomIconQueue { get { return shroomIconList; } }
        public float oR { get { return originalR; } }
        public float oG { get { return originalG; } }
        public float oB { get { return originalB; } }
        #endregion

        private void OnEnable()
        {
            UIEvent.addToShroomCounter.AddListener(AddShroom);
            UIEvent.removeFromShroomCounter.AddListener(RemoveShroom);
            UIEvent.resetCounterQueue.AddListener(ResetQueue);
            mushroomEvent.unlockThrowEvent.AddListener(ShowCounter);
            disableEvent.enableHUD.AddListener(ShowCounter);
            disableEvent.disableHUD.AddListener(HideCounter);
        }

        private void OnDisable()
        {
            UIEvent.addToShroomCounter.RemoveListener(AddShroom);
            UIEvent.removeFromShroomCounter.RemoveListener(RemoveShroom);
            UIEvent.resetCounterQueue.RemoveListener(ResetQueue);
            mushroomEvent.unlockThrowEvent.RemoveListener(ShowCounter);
            disableEvent.enableHUD.RemoveListener(ShowCounter);
            disableEvent.disableHUD.RemoveListener(HideCounter);
        }

        // Start is called before the first frame update
        void Start()
        {
            // Gets references to shroom icon components
            shroomIcon1 = GameObject.Find("Shroom Icon 1");
            shroomIcon2 = GameObject.Find("Shroom Icon 2");
            shroomIcon3 = GameObject.Find("Shroom Icon 3");
            originalR = shroomIcon1.GetComponent<Image>().color.r;
            originalG = shroomIcon1.GetComponent<Image>().color.g;
            originalB = shroomIcon1.GetComponent<Image>().color.b;
            newR = shroomIcon1.GetComponent<Image>().color.r - .5f;
            newG = shroomIcon1.GetComponent<Image>().color.g - .5f;
            newB = shroomIcon1.GetComponent<Image>().color.b - .5f;
            shroomIcon1.GetComponent<Image>().fillAmount = 0f;
            shroomIcon2.GetComponent<Image>().fillAmount = 0f;
            shroomIcon3.GetComponent<Image>().fillAmount = 0f;

            shroomOutline1 = GameObject.Find("Shroom Outline 1").GetComponent<Image>();
            shroomOutline2 = GameObject.Find("Shroom Outline 2").GetComponent<Image>();
            shroomOutline3 = GameObject.Find("Shroom Outline 3").GetComponent<Image>();

            shroomFill1 = GameObject.Find("ShroomFill 1").GetComponent<Image>();
            shroomFill2 = GameObject.Find("ShroomFill 2").GetComponent<Image>();
            shroomFill3 = GameObject.Find("ShroomFill 3").GetComponent<Image>();

            shroomIconList = new List<GameObject>()
            {
                shroomIcon3,
                shroomIcon2,
                shroomIcon1
            };

            if (SceneManager.GetActiveScene().buildIndex >= 6)
            {
                ResetQueue();
            }

            UIEvent.SetShroomCounter(shroomIconList);
        }

        /// <summary>
        /// Resets the queue and the color values of all shroom icons
        /// </summary>
        public void ResetQueue()
        {
            // Clears the queue
            if (!shroomIconList.Contains(shroomIcon1))
            {
                shroomIcon1.GetComponent<ParticleSystem>().Play();
            }

            if (!shroomIconList.Contains(shroomIcon2))
            {
                shroomIcon2.GetComponent<ParticleSystem>().Play();
            }

            if (!shroomIconList.Contains(shroomIcon3))
            {
                shroomIcon3.GetComponent<ParticleSystem>().Play();
            }

            shroomIconList.Clear();
            shroomIconList.Add(shroomIcon3);
            // Enqueues the first shroom icon and resets its color values
            shroomIcon3.GetComponent<Image>().fillAmount = 1;
            shroomIconList.Add(shroomIcon2);
            // Enqueues the first shroom icon and resets its color values
            shroomIcon2.GetComponent<Image>().fillAmount = 1;
            shroomIconList.Add(shroomIcon1);
            // Enqueues the first shroom icon and resets its color values
            shroomIcon1.GetComponent<Image>().fillAmount = 1;
        }

        /// <summary>
        /// Hide the Shroom Counter
        /// </summary>
        private void HideCounter()
        {
            shroomIcon1.GetComponent<Image>().color = new Color(originalR, originalG, originalB, 0);
            shroomIcon2.GetComponent<Image>().color = new Color(originalR, originalG, originalB, 0);
            shroomIcon3.GetComponent<Image>().color = new Color(originalR, originalG, originalB, 0);
            shroomOutline1.color = new Color(originalR, originalG, originalB, 0);
            shroomOutline2.color = new Color(originalR, originalG, originalB, 0);
            shroomOutline3.color = new Color(originalR, originalG, originalB, 0);
            shroomFill1.color = new Color(originalR, originalG, originalB, 0);
            shroomFill2.color = new Color(originalR, originalG, originalB, 0);
            shroomFill3.color = new Color(originalR, originalG, originalB, 0);
        }

        /// <summary>
        /// Show the Shroom Counter, given the player can throw Mushrooms
        /// </summary>
        private void ShowCounter()
        {
            // Check if the player has unlocked the throw ability
            if (mushroomEvent.GetThrowUnlocked())
            {
                shroomOutline1.color = new Color(originalR, originalG, originalB, 1);
                shroomOutline2.color = new Color(originalR, originalG, originalB, 1);
                shroomOutline3.color = new Color(originalR, originalG, originalB, 1);
                shroomFill1.color = new Color(originalR, originalG, originalB, 0.3882353f);
                shroomFill2.color = new Color(originalR, originalG, originalB, 0.3882353f);
                shroomFill3.color = new Color(originalR, originalG, originalB, 0.3882353f);
                shroomIcon1.GetComponent<Image>().color = new Color(originalR, originalG, originalB, 1);
                shroomIcon2.GetComponent<Image>().color = new Color(originalR, originalG, originalB, 1);
                shroomIcon3.GetComponent<Image>().color = new Color(originalR, originalG, originalB, 1);
                shroomIcon1.GetComponent<Image>().fillAmount = 1;
                shroomIcon2.GetComponent<Image>().fillAmount = 1;
                shroomIcon3.GetComponent<Image>().fillAmount = 1;
                displayCounter = false;
            }
        }

        #region EVENT FUNCTIONS
        /// <summary>
        ///  Add a shroom to the queue
        /// </summary>
        /// <param name="shroomToAdd">The shroom to add</param>
        public void AddShroom(GameObject shroomToAdd)
        {
            shroomIconList.Add(shroomToAdd);
        }

        /// <summary>
        /// Remove a shroom from the queue
        /// </summary>
        /// <param name="shroomToRemove">The shroom to remove</param>
        public void RemoveShroom(GameObject shroomToRemove)
        {
            shroomIconList.Remove(shroomToRemove);
        }
        #endregion
    }
}
