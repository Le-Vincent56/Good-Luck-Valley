using HiveMind.Core;
using HiveMind.Events;
using System.Collections;
using UnityEngine;

namespace HiveMind.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameObject player;
        [SerializeField] private TutorialMessage interactPrompt;
        [SerializeField] private Interactable interactableObject;
        [SerializeField] private GameObject interactPrompt2;
        [SerializeField] private GameObject mushroomThrowPrompt;
        [SerializeField] private GameObject mushroomBouncePrompt;
        [SerializeField] private GameObject mushroomLimitPrompt;
        [SerializeField] private GameObject movementPrompt;
        [SerializeField] private GameObject journalPrompt;
        [SerializeField] private GameObject fastFallPrompt;
        [SerializeField] private GameObject wallBouncePrompt;
        [SerializeField] private GameObject quickBouncePrompt;
        [SerializeField] private CutsceneScriptableObj cutsceneEvent;
        [SerializeField] private JournalScriptableObj journalEvent;
        [SerializeField] private MushroomScriptableObj mushroomEvent;
        [SerializeField] private CameraScriptableObj cameraEvent;
        [SerializeField] private MovementScriptableObj movementEvent;
        [SerializeField] private Camera UICam;
        #endregion

        #region FIELDS
        private bool updateMessages;
        private bool showThrowMessageTriggered = false;
        private bool waitingToShow = false;
        private bool showingFastFall;
        private bool showingWallBounce;
        #endregion

        #region PROPERTIES
        public bool UpdateMessages { get { return updateMessages; } set { updateMessages = value; } }
        public bool ShowingFastFall { get { return showingFastFall; } set { showingFastFall = value; } }
        public bool ShowingWallBounce { get { return showingWallBounce; } set { showingWallBounce = value; } }
        #endregion

        private void OnEnable()
        {
            cutsceneEvent.endLotusCutscene.AddListener(EnableMovementPrompt);
            mushroomEvent.showThrowMessageEvent.AddListener(ShowMushThrowMessage);
            mushroomEvent.hideThrowMessageEvent.AddListener(HideMushThrowMessage);
            mushroomEvent.showBounceMessageEvent.AddListener(ShowMushBounceMessage);
            mushroomEvent.hideBounceMessageEvent.AddListener(HideMushBouceMessage);
            mushroomEvent.showMaxMessageEvent.AddListener(ShowMushMaxMessage);
            mushroomEvent.hideMaxMessageEvent.AddListener(HideMushMaxMessage);
            mushroomEvent.showWallBounceMessageEvent.AddListener(ShowWallBounceMessage);
            mushroomEvent.hideWallBounceMessageEvent.AddListener(HideWallBounceMessage);
            mushroomEvent.showQuickBounceMessageEvent.AddListener(ShowQuickBounceMessage);
            mushroomEvent.hideQuickBounceMessageEvent.AddListener(HideQuickBounceMessage);
            movementEvent.showFastFallMessageEvent.AddListener(ShowFastFallMessage);
            movementEvent.hideFastFallMessageEvent.AddListener(HideFastFallMessage);
        }

        private void OnDisable()
        {
            cutsceneEvent.endLotusCutscene.RemoveListener(EnableMovementPrompt);
            mushroomEvent.showThrowMessageEvent.RemoveListener(ShowMushThrowMessage);
            mushroomEvent.hideThrowMessageEvent.RemoveListener(HideMushThrowMessage);
            mushroomEvent.showBounceMessageEvent.RemoveListener(ShowMushBounceMessage);
            mushroomEvent.hideBounceMessageEvent.RemoveListener(HideMushBouceMessage);
            mushroomEvent.showMaxMessageEvent.RemoveListener(ShowMushMaxMessage);
            mushroomEvent.hideMaxMessageEvent.RemoveListener(HideMushMaxMessage);
            mushroomEvent.showWallBounceMessageEvent.RemoveListener(ShowWallBounceMessage);
            mushroomEvent.hideWallBounceMessageEvent.RemoveListener(HideWallBounceMessage);
            mushroomEvent.showQuickBounceMessageEvent.RemoveListener(ShowQuickBounceMessage);
            mushroomEvent.hideQuickBounceMessageEvent.RemoveListener(HideQuickBounceMessage);
            movementEvent.showFastFallMessageEvent.RemoveListener(ShowFastFallMessage);
            movementEvent.hideFastFallMessageEvent.RemoveListener(HideFastFallMessage);
        }

        private void Start()
        {
            if (movementPrompt != null)
            {
                movementPrompt.SetActive(false);
            }

            if (movementEvent.GetShowingFastFall() == false)
            {
                HideFastFallMessage();
            }

            if (mushroomEvent.GetShowingQuickBounceMessage() == false)
            {
                HideQuickBounceMessage();
            }

            if (mushroomEvent.GetFirstWallBounce() == false)
            {
                HideWallBounceMessage();
            }
        }

        private void Update()
        {
            if (interactableObject.Remove == true || interactableObject.gameObject.activeSelf == false)
            {
                interactPrompt.RemoveMessage = true;
                interactPrompt.GetComponent<TutorialMessage>().Hide();
            }

            if (interactPrompt.RemoveMessage == true && interactableObject.Remove == true)
            {
                Debug.Log("Interact Prompt Disabled, interactable object remove == true");
                interactPrompt2.SetActive(true);

                if (!showThrowMessageTriggered)
                {
                    Debug.Log("Show Message Throw trigger");
                    showThrowMessageTriggered = true;
                    mushroomEvent.ShowThrowMessage();
                }
            }

            if (journalEvent.GetOpenedOnce())
            {
                interactPrompt2.SetActive(false);
            }
        }

        public void EnableMovementPrompt()
        {
            if (movementPrompt != null)
            {
                movementPrompt.SetActive(true);
            }
        }

        public void EnableMessageUpdates()
        {
            updateMessages = true;
            StartCoroutine(WaitFor1Sec());
        }

        public void ShowMushThrowMessage()
        {
            // Show the prompt
            Debug.Log("Mush Throw Called");
            if (mushroomEvent.GetFirstThrow() && mushroomThrowPrompt != null)
            {
                Debug.Log("Mush Throw");
                ShowOverPlayer(mushroomThrowPrompt);
                mushroomThrowPrompt.GetComponent<TutorialMessage>().Show();
            }
        }

        public void HideMushThrowMessage()
        {
            // Hide the prompt
            if (mushroomThrowPrompt != null)
            {
                mushroomThrowPrompt.GetComponent<TutorialMessage>().RemoveMessage = true;
                mushroomThrowPrompt.GetComponent<TutorialMessage>().Hide();
            }
        }

        public void ShowMushBounceMessage()
        {
            // Show the prompt
            if (mushroomEvent.GetFirstBounce() && mushroomBouncePrompt != null)
            {
                ShowOverPlayer(mushroomBouncePrompt);
                mushroomBouncePrompt.GetComponent<TutorialMessage>().Show();
            }
        }

        public void HideMushBouceMessage()
        {
            // Hide the prompt
            if (mushroomBouncePrompt != null)
            {
                mushroomBouncePrompt.GetComponent<TutorialMessage>().RemoveMessage = true;
                mushroomBouncePrompt.GetComponent<TutorialMessage>().Hide();
            }
        }

        public void ShowMushMaxMessage()
        {
            // Show the prompt
            if (mushroomEvent.GetFirstFull() && mushroomLimitPrompt != null)
            {
                waitingToShow = true;
                mushroomLimitPrompt.SetActive(true);
                StartCoroutine(WaitUntilGrounded());
            }
        }

        public void HideMushMaxMessage()
        {
            if (mushroomLimitPrompt != null)
            {
                // Hide the prompt
                mushroomLimitPrompt.GetComponent<TutorialMessage>().RemoveMessage = true;
                mushroomLimitPrompt.GetComponent<TutorialMessage>().Hide();
            }
        }

        public void ShowFastFallMessage()
        {
            if (fastFallPrompt != null)
            {
                fastFallPrompt.GetComponent<TutorialMessage>().Show();
            }
        }

        public void HideFastFallMessage()
        {
            if (fastFallPrompt != null)
            {
                fastFallPrompt.GetComponent<TutorialMessage>().RemoveMessage = true;
                fastFallPrompt.GetComponent<TutorialMessage>().Hide();
                movementEvent.SetShowingFastFall(false);
            }
        }

        public void ShowWallBounceMessage()
        {
            if (wallBouncePrompt != null)
            {
                wallBouncePrompt.GetComponent<TutorialMessage>().Show();
            }
        }

        public void HideWallBounceMessage()
        {
            if (wallBouncePrompt != null)
            {
                wallBouncePrompt.GetComponent<TutorialMessage>().RemoveMessage = true;
                wallBouncePrompt.GetComponent<TutorialMessage>().Hide();
            }
        }

        public void ShowQuickBounceMessage()
        {
            if (quickBouncePrompt != null)
            {
                quickBouncePrompt.GetComponent<TutorialMessage>().Show();
            }
        }

        public void HideQuickBounceMessage()
        {
            if (quickBouncePrompt != null)
            {
                quickBouncePrompt.GetComponent<TutorialMessage>().RemoveMessage = true;
                quickBouncePrompt.GetComponent<TutorialMessage>().Hide();
            }
        }

        public void UpdateQuickBounce(bool showingQuickBounce)
        {
            mushroomEvent.SetTouchingQuickBounceMessage(showingQuickBounce);
        }

        public void UpdateFastFall(bool touchingFastFall)
        {
            movementEvent.SetTouchingFastFall(touchingFastFall);
        }

        public void ShowOverPlayer(GameObject UIObject)
        {
            // Create a position above the player in world space
            Vector2 offsetPos = new Vector2(player.transform.position.x, player.transform.position.y + 1.0f);

            // Calculate the screen position for the canvas
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

            // Convert screen position to Canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<Canvas>().GetComponent<RectTransform>(), screenPoint, UICam, out canvasPos);

            // Clamp canvas pos into the camera bounds
            if (canvasPos != null)
            {
                canvasPos = new Vector2(canvasPos.x, Mathf.Clamp(canvasPos.y, cameraEvent.GetBottomBound(), cameraEvent.GetTopBound() - 1f));
            }

            // Set the objects position within canvas pos
            UIObject.transform.localPosition = canvasPos;
        }

        private IEnumerator WaitUntilGrounded()
        {
            // Wait until grounded
            while (waitingToShow)
            {
                if (movementEvent.GetIsGrounded())
                {
                    // When grounded, show the mushroom limit prompt
                    ShowOverPlayer(mushroomLimitPrompt);
                    mushroomLimitPrompt.GetComponent<TutorialMessage>().Show();
                    waitingToShow = false;
                }

                yield return null;
            }
        }

        private IEnumerator WaitFor1Sec()
        {
            yield return new WaitForSeconds(0.25f);
            updateMessages = false;
        }
    }
}
