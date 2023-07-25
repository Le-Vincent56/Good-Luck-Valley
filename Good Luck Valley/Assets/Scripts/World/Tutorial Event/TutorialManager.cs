using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        if (cutsceneEvent.GetEnterCutsceneActive())
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

        if (movementPrompt != null)
        {
            if (cutsceneEvent.GetPlayingCutscene())
            {
                movementPrompt.SetActive(false);
            }
            else if (!cutsceneEvent.GetEnterCutsceneActive())
            {
                movementPrompt.SetActive(true);
            }
        }
    }

    public void EnableMessageUpdates()
    {
        updateMessages = true;
        StartCoroutine(WaitFor1Sec());
    }

    private IEnumerator WaitFor1Sec()
    {
        yield return new WaitForSeconds(1);
        updateMessages = false;
    }    
}
