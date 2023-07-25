using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private TutorialMessage interactPrompt;
    [SerializeField] private Interactable interactableObject;
    [SerializeField] private GameObject interactPrompt2;
    [SerializeField] private GameObject movementPrompt;
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    #endregion

    #region FIELDS
    private bool updateMessages;
    #endregion

    #region PROPERTIES
    public bool UpdateMessages { get { return updateMessages; } set {  updateMessages = value; } }
    #endregion

    private void OnEnable()
    {
        cutsceneEvent.endEnterCutscene.AddListener(EnableMovementPrompt);
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
        if (interactableObject.Remove == true)
        {
            interactPrompt.RemoveMessage = true;
            StartCoroutine(interactPrompt.FadeOut());
        }

        if (interactPrompt.gameObject.activeSelf == false)
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

    private IEnumerator WaitFor1Sec()
    {
        yield return new WaitForSeconds(0.25f);
        updateMessages = false;
    }    
}
