using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteNotification : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private bool triggerNotif = false;
    [SerializeField] private float fadeInTimer = 1.0f;
    [SerializeField] private float notifTimer = 3.0f;
    [SerializeField] private float fadeOutTimer = 1.0f;
    [SerializeField] private bool continueToLingerTimer = false;
    [SerializeField] private bool continueToFadeOutTimer = false;
    [SerializeField] private bool fadeCompleted = false;
    private Image panelImage;
    private Text entryAddedText;
    private Text noteTitle;
    #endregion

    #region PROPERTIES
    public bool TriggerNotif { get { return triggerNotif; } set { triggerNotif = value; } }
    public Text NoteTitle { get { return noteTitle; } set { noteTitle = value; } }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        panelImage = GameObject.Find("NoteEffectPanel").GetComponent<Image>();
        entryAddedText = GameObject.Find("EntryAddedText").GetComponent<Text>();
        noteTitle = GameObject.Find("NoteTitleText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(triggerNotif)
        {
            // Fade the notification in
            if (fadeInTimer < 0.5)
            {
                // Add deltaTime to the fadeInTimer as well as the a value of the text and panel
                fadeInTimer += Time.deltaTime;
                panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, panelImage.color.a + (Time.deltaTime*2));
                entryAddedText.color = new Color(entryAddedText.color.r, entryAddedText.color.g, entryAddedText.color.b, entryAddedText.color.a + (Time.deltaTime * 2));
                noteTitle.color = new Color(noteTitle.color.r, noteTitle.color.g, noteTitle.color.b, noteTitle.color.a + (Time.deltaTime * 2));

                // Set continueToLingerTimer to false for security
                continueToLingerTimer = false;
            }
            else continueToLingerTimer = true;

            // Leave text on the screen for a little
            if (notifTimer > 0 && continueToLingerTimer)
            {
                // Have the text and panel at full opacity until notifTimer runs out
                notifTimer -= Time.deltaTime;

                // Set continueToFadeOutTimer to false for security
                continueToFadeOutTimer = false;
            }
            else if(notifTimer <= 0)
            {
                // Once the notifTimer runs out, continue to the fade out timer
                continueToFadeOutTimer = true;
            }

            // Fade out text
            if (fadeOutTimer > 0 && continueToFadeOutTimer)
            {
                // Subtract deltaTime to the fadeInTimer as well as the a value of the text and panel
                panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, panelImage.color.a - Time.deltaTime);
                entryAddedText.color = new Color(entryAddedText.color.r, entryAddedText.color.g, entryAddedText.color.b, entryAddedText.color.a - Time.deltaTime);
                noteTitle.color = new Color(noteTitle.color.r, noteTitle.color.g, noteTitle.color.b, noteTitle.color.a - Time.deltaTime);
                fadeOutTimer -= Time.deltaTime;

                // Set fadeCompleted to false for security
                fadeCompleted = false;
            } else if(fadeOutTimer <= 0)
            {
                // Once the fadeOutTimer runs out, continue
                fadeCompleted = true;
            }

            // Set triggerNotif to false and set noteTitle to nothing once the fade is completed
            if(fadeCompleted)
            {
                triggerNotif = false;
                noteTitle.text = "";
            }
        }
    }

    /// <summary>
    /// Prepares the notification to trigger
    /// </summary>
    public void PrepareNotification()
    {
        // Set timers and show text
        fadeInTimer = 0.0f;
        notifTimer = 3.0f;
        fadeOutTimer = 1.0f;

        // Reset booleans
        continueToLingerTimer = false;
        continueToFadeOutTimer = false;
        fadeCompleted = false;
    }
}
