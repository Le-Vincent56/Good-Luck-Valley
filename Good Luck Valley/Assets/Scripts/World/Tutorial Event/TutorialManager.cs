using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    #endregion

    #region PROPERTIES
    public bool UpdateMessages { get { return updateMessages; } set {  updateMessages = value; } }
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
    }

    private void Start()
    {
        if (movementPrompt != null)
        {
            movementPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (interactableObject.Remove == true || interactableObject.gameObject.activeSelf == false)
        {
            interactPrompt.RemoveMessage = true;
            StartCoroutine(interactPrompt.FadeOut());
        }

        if (interactPrompt.gameObject.activeSelf == false)
        {
            interactPrompt2.SetActive(true);

            if(!showThrowMessageTriggered)
            {
                showThrowMessageTriggered = true;
                mushroomEvent.ShowThrowMessage();
            }
        }

        if (journalEvent.GetOpenedOnce())
        {
            interactPrompt2.SetActive(false);
        }
        else
        {
            interactPrompt2.SetActive(true);
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
        if(mushroomEvent.GetFirstThrow() && mushroomThrowPrompt != null)
        {
            ShowOverPlayer(mushroomThrowPrompt);
            mushroomThrowPrompt.GetComponent<TutorialMessage>().Show();
        }
    }

    public void HideMushThrowMessage()
    {
        // Hide the prompt
        if(mushroomThrowPrompt != null)
        {
            mushroomThrowPrompt.GetComponent<TutorialMessage>().RemoveMessage = true;
            mushroomThrowPrompt.GetComponent<TutorialMessage>().Hide();
        }
    }

    public void ShowMushBounceMessage()
    {
        // Show the prompt
        if(mushroomEvent.GetFirstBounce() && mushroomBouncePrompt != null)
        {
            ShowOverPlayer(mushroomBouncePrompt);
            mushroomBouncePrompt.GetComponent<TutorialMessage>().Show();
        }
    }

    public void HideMushBouceMessage()
    {
        // Hide the prompt
        if(mushroomBouncePrompt != null)
        {
            mushroomBouncePrompt.GetComponent<TutorialMessage>().RemoveMessage = true;
            mushroomBouncePrompt.GetComponent<TutorialMessage>().Hide();
        }
    }

    public void ShowMushMaxMessage()
    {
        // Show the prompt
        if(mushroomEvent.GetFirstFull() && mushroomLimitPrompt != null)
        {
            waitingToShow = true;
            mushroomLimitPrompt.SetActive(true);
            StartCoroutine(WaitUntilGrounded());
        }
    }

    public void HideMushMaxMessage()
    {
        if(mushroomLimitPrompt != null)
        {
            // Hide the prompt
            mushroomLimitPrompt.GetComponent<TutorialMessage>().RemoveMessage = true;
            mushroomLimitPrompt.GetComponent<TutorialMessage>().Hide();
        }
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
        if(canvasPos != null)
        {
            canvasPos = new Vector2(canvasPos.x, Mathf.Clamp(canvasPos.y, cameraEvent.GetBottomBound(), cameraEvent.GetTopBound() - 1f));
        }

        // Set the objects position within canvas pos
        UIObject.transform.localPosition = canvasPos;
    }

    private IEnumerator WaitUntilGrounded()
    {
        // Wait until grounded
        while(waitingToShow)
        {
            if(movementEvent.GetIsGrounded())
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
