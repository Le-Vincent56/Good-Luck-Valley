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
    #endregion

    #region FIELDS
    private bool shown;
    [SerializeField] private string textValue;
    [SerializeField] private float fadeAmount;
    private bool fadingOut;
    private bool fadingIn;
    [SerializeField] private int[] controlIndexes;
    private string[] controlChars;
    private int textValueControlsCount;
    string[] textValueSplit;
    #endregion

    #region PROPERTIES
    public bool Shown { get { return shown; } set {  shown = value; } }
    #endregion

    private void OnEnable()
    {
        // Debug.Log(controls.actionMaps[0].actions[3].bindings[0].path);
        // Debug.Log(tex    tValue + " split length: " + textValue.Split("\"").Length);

        // Debug.Log(textValue + " controls count: " + textValueControlsCount);

        messageText = GetComponent<Text>();
        controlChars = new string[controlIndexes.Length];

        InitializeTextValue();

        messageText.color = new Color(1, 1, 1, 0);
    }

    private void Update()
    {
        InitializeTextValue();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fadingOut)
        {
            StopCoroutine(FadeOut());
        }
        StartCoroutine(FadeIn());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (fadingIn)
        {
            StopCoroutine(FadeIn());
        }
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        fadingIn = true;
        while (messageText.color.a <= 1)
        {
            messageText.color = new Color(1, 1, 1, messageText.color.a + fadeAmount);
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        fadingOut = true;
        while (messageText.color.a >= 0)
        {
            messageText.color = new Color(1, 1, 1, messageText.color.a - fadeAmount);
            yield return null;
        }

    }

    /// <summary>
    /// Adjusts the text value to display based on the 
    /// </summary>
    private void InitializeTextValue()
    {
        // Gets the split values for the given text string, using " as the split character
        textValueSplit = textValue.Split("\"");

        // Checks if the lenght is greater than 1 (there is at least 1 character within the string that needs to be changed)
        textValueControlsCount = controlIndexes.Length;

        InputActionMap actionMap = controls.actionMaps[0];

        for (int i = 0; i < controlIndexes.Length; i++)
        {
            string bindChar;
            if (actionMap.actions[controlIndexes[i]].bindings[0].isComposite)   
            {
                bindChar = actionMap.actions[controlIndexes[i]].bindings[i + 3].path.Split("/")[1];
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
            }
        }

        textValue = "";
        for (int i = 0; i < textValueSplit.Length; i++)
        {
            Debug.Log(textValueSplit[i]);
            textValue += textValueSplit[i];
        }

        messageText.text = textValue;
    }
}
