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
        // Initalizes the tutorial manager
        manager = GetComponentInParent<TutorialManager>();

        // Gets the text component from the object
        messageText = GetComponent<Text>();
        // Initializes the controls characters array,
        //  this will hold the characters that will be used to replace the default
        //  controls in the tutorial message prompts
        controlChars = new string[controlIndexes.Length];

        // Runs the initialization of the tutorial prompts 
        InitializeTextValue();

        // Sets the default message text color to fully transparent
        messageText.color = new Color(1, 1, 1, 0);
    }

    private void Update()
    {
        // If we should update the tutorial messages, run the initialization method again to do so
        if (manager.UpdateMessages) 
        {
            InitializeTextValue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Show();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Hide();
    }

    public void Show()
    {
        // Stop other coroutines
        StopAllCoroutines();

        if(gameObject.activeSelf != false)
        {
            // Fade in the message
            StartCoroutine(FadeIn());
        }
    }

    public void Hide()
    {
        // Stop other coroutines
        StopAllCoroutines();

        // Make sure the game object isnt set to inactive
        if (gameObject.activeSelf != false)
        {
            // Fade out the message
            StartCoroutine(FadeOut());
        }
    }

    public IEnumerator FadeIn()
    {
        // Fade in the message text
        while (messageText.color.a <= 1)
        {
            messageText.color = new Color(1, 1, 1, messageText.color.a + fadeAmount);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        // Fade out the message text
        while (messageText.color.a >= 0)
        {
            messageText.color = new Color(1, 1, 1, messageText.color.a - fadeAmount);

            // Check if we need to remove this message
            if (messageText.color.a < 0 && removeMessage)
            {
                // If so, set it to inactive
                gameObject.SetActive(false);
            }

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

        // Initialize our action map reference
        InputActionMap actionMap = controls.actionMaps[0];

        // Loop through the control indexes array
        //  (manually input values for the indexes of the controls that relate to the message)
        for (int i = 0; i < controlIndexes.Length; i++)
        {
            // A string to hold the currently set bind character (keybind)
            string bindChar;

            // Check if the control index is less than 0, if it is then we need set up a composite control prompt, so the code is different
            if (controlIndexes[i] < 0)
            {
                // Check if the override path string isnt null / empty
                if (!string.IsNullOrEmpty(actionMap.actions[0].bindings[Mathf.Abs(controlIndexes[i])].overridePath))
                {
                    // If not, assign the bind character by getting the override path at the given control index
                    // (absolute value because its negative), splitting by a /, the key used follows this / in the string
                    bindChar = actionMap.actions[0].bindings[Mathf.Abs(controlIndexes[i])].overridePath.Split("/")[1];
                }
                else
                {
                    // If it is null / empty, then get the binding using the default path
                    bindChar = actionMap.actions[0].bindings[Mathf.Abs(controlIndexes[i])].path.Split("/")[1];
                }
            }
            // Otherwise, check if the override path string is null or empty
            else if (!string.IsNullOrEmpty(actionMap.actions[controlIndexes[i]].bindings[0].overridePath))
            {
                // If not, assign the bind character by getting the override path at the given control index
                // (absolute value because its negative), splitting by a /, the key used follows this / in the string
                bindChar = actionMap.actions[controlIndexes[i]].bindings[0].overridePath.Split("/")[1];
            }
            else
            {
                // If it is null / empty, then get the binding sing the default path
                bindChar = actionMap.actions[controlIndexes[i]].bindings[0].path.Split("/")[1];
            }

            // Save the bind character in the control characters array
            controlChars[i] = bindChar;
        }

        // Loop through the text value split array
        for (int i = 0; i < textValueSplit.Length; i++)
        {
            // If we are looking at an odd value, update the character here
            // This is because each tutorial message control character is on on an odd number of splits
            // I.E. If the string can be split into 7 sub strings, then there is a character at indexes 1, 3, 5
            if (i % 2 != 0)
            {
                // Override the value by adding the in the " on either side, and using the control characters value at i / 2
                // This is because if i is 1, then i / 2 = 0, and so on
                textValueSplit[i] = "\"" + controlChars[i / 2].ToUpper() + "\"";

                // Check if the character contains the string BUTTON
                if (textValueSplit[i].Contains("BUTTON"))
                {
                    // If it does, then we need to replace the word BUTTON with -CLICK
                    // Split it at the character B, and then add -CLICK" to the end of the string
                    textValueSplit[i] = textValueSplit[i].Split("B")[0] + "-CLICK\"";
                }
            }
        }

        // Set the text value to be empty
        textValue = "";

        // Concatenate the text value string using the values in the text value split array
        for (int i = 0; i < textValueSplit.Length; i++)
        {
            textValue += textValueSplit[i];
        }

        // Set the message text to the new text value string
        messageText.text = textValue;
    }
}
