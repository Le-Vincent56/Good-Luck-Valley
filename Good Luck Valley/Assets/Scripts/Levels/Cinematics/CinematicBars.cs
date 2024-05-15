using HiveMind.Events;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HiveMind.Cinematics
{
    public class CinematicBars : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] CutsceneScriptableObj cutsceneEvent;
        [SerializeField] private float targetSize;
        [SerializeField] private float showSize = 200;
        [SerializeField] private float changeSizeAmount = 5;
        [SerializeField] private float timeToMove = 0.3f;
        [SerializeField] private bool isActive = true;
        private RectTransform topBar;
        private RectTransform bottomBar;
        #endregion

        private void Awake()
        {
            // Create the top bar
            GameObject gameObject = new GameObject("topBar", typeof(Image));
            gameObject.transform.SetParent(transform, false);
            gameObject.GetComponent<Image>().color = Color.black;
            topBar = gameObject.GetComponent<RectTransform>();

            topBar.anchorMin = new Vector2(0, 1);
            topBar.anchorMax = new Vector2(1, 1);
            topBar.sizeDelta = new Vector2(0, 0);

            // Create the bottom bar
            gameObject = new GameObject("bottomBar", typeof(Image));
            gameObject.transform.SetParent(transform, false);
            gameObject.GetComponent<Image>().color = Color.black;
            bottomBar = gameObject.GetComponent<RectTransform>();

            bottomBar.anchorMin = new Vector2(0, 0);
            bottomBar.anchorMax = new Vector2(1, 0);
            bottomBar.sizeDelta = new Vector2(0, 0);
        }

        private void OnEnable()
        {
            cutsceneEvent.startLotusCutscene.AddListener(Show);
            cutsceneEvent.endLotusCutscene.AddListener(Hide);
        }

        private void OnDisable()
        {
            cutsceneEvent.startLotusCutscene.RemoveListener(Show);
            cutsceneEvent.endLotusCutscene.RemoveListener(Hide);
        }

        public void Show()
        {
            targetSize = showSize;
            changeSizeAmount = (targetSize - topBar.sizeDelta.y) / timeToMove;
            isActive = true;

            StartCoroutine(MoveCinematicBars());
        }

        public void Hide()
        {
            targetSize = 0f;
            changeSizeAmount = (targetSize - topBar.sizeDelta.y) / timeToMove;
            isActive = true;

            StartCoroutine(MoveCinematicBars());
        }

        private IEnumerator MoveCinematicBars()
        {
            while (isActive)
            {
                // Allow other code to run
                yield return null;

                Vector2 sizeDelta = topBar.sizeDelta;
                sizeDelta.y += changeSizeAmount * Time.deltaTime;

                if (changeSizeAmount > 0)
                {
                    if (sizeDelta.y >= targetSize)
                    {
                        sizeDelta.y = targetSize;
                        isActive = false;
                    }
                }
                else
                {
                    if (sizeDelta.y <= targetSize)
                    {
                        sizeDelta.y = targetSize;
                        isActive = false;
                    }
                }

                // Change the topBar and bottomBar size deltas
                topBar.sizeDelta = sizeDelta;
                bottomBar.sizeDelta = sizeDelta;
            }
        }
    }
}
