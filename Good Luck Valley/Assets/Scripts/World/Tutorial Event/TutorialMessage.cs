using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialMessage : MonoBehaviour
{
    #region REFERENCES
    private Text messageText;
    [SerializeField] private InputActionAsset controls;
    private TutorialManager manager;
    #endregion

    #region FIELDS
    private bool shown;
    [SerializeField] private string textValue;
    [SerializeField] private float fadeAmount;
    [SerializeField] private int[] controlIndexes;
    private string[] controlChars;
    string[] textValueSplit;
    private bool removeMessage;
    #endregion

    #region PROPERTIES
    public bool Shown { get { return shown; } set {  shown = value; } }
    public bool RemoveMessage { get { return removeMessage; } set {  removeMessage = value; } }
    #endregion

    private void OnEnable()
    {
        manager = GetComponentInParent<TutorialManager>();

        messageText = GetComponent<Text>();
        controlChars = new string[controlIndexes.Length];

        InitializeTextValue();

        messageText.color = new Color(1, 1, 1, 0);
    }

    private void Update()
    {
        if (manager.UpdateMessages)
        {
            Debug.Log("initializeing data :D");
            InitializeTextValue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StopCoroutine(FadeIn());
        StopCoroutine(FadeOut());
        if (gameObject.activeSelf != false)
        {
            StartCoroutine(FadeIn());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StopCoroutine(FadeIn());
        StopCoroutine(FadeOut());
        if (gameObject.activeSelf != false)
        {
            StartCoroutine(FadeOut());
        }
    }

    public IEnumerator FadeIn()
    {
        while (messageText.color.a <= 1)
        {
            messageText.color = new Color(1, 1, 1, messageText.color.a + fadeAmount);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        while (messageText.color.a >= 0)
        {
            messageText.color = new Color(1, 1, 1, messageText.color.a - fadeAmount);
            yield return null;
        }

        if (removeMessage)
        {
            Debug.Log("remove message");
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Adjusts the text value to display based on the 
    /// </summary>
    private void InitializeTextValue()
    {
        // Gets the split values for the given text string, using " as the split character
        textValueSplit = textValue.Split("\"");

        InputActionMap actionMap = controls.actionMaps[0];

        for (int i = 0; i < controlIndexes.Length; i++)
        {
            string bindChar;
            if (controlIndexes[i] < 0)
            {
                if (!string.IsNullOrEmpty(actionMap.actions[0].bindings[Mathf.Abs(controlIndexes[i])].overridePath))
                {
                    bindChar = actionMap.actions[0].bindings[Mathf.Abs(controlIndexes[i])].overridePath.Split("/")[1];
                }
                else
                {
                    bindChar = actionMap.actions[0].bindings[Mathf.Abs(controlIndexes[i])].path.Split("/")[1];
                }
            }
            else if (!string.IsNullOrEmpty(actionMap.actions[controlIndexes[i]].bindings[0].overridePath))
            {
                bindChar = actionMap.actions[controlIndexes[i]].bindings[0].overridePath.Split("/")[1];
            }
            else
            {
                bindChar = actionMap.actions[controlIndexes[i]].bindings[0].path.Split("/")[1];
            }
            controlChars[i] = bindChar;
        }

        for (int i = 0; i < textValueSplit.Length; i++)
        {
            if (i % 2 != 0)
            {
                textValueSplit[i] = "\"" + controlChars[i / 2].ToUpper() + "\"";
                if (textValueSplit[i].Contains("BUTTON"))
                {
                    textValueSplit[i] = textValueSplit[i].Split("B")[0] + "-CLICK\"";
                }
            }
        }

        textValue = "";
        for (int i = 0; i < textValueSplit.Length; i++)
        {
            textValue += textValueSplit[i];
        }

        messageText.text = textValue;
    }
}
