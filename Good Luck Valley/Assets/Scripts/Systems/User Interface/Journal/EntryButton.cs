using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region REFERENCES
    [SerializeField] private JournalScriptableObj journalEvent;
    private Text panelTextTitle;
    private Text panelText;
    private Text entryName;
    private Note associatedNote;
    #endregion

    #region PROPERTIES
    public Note Note { get { return associatedNote; } set { associatedNote = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        panelTextTitle = GameObject.Find("EntryTextTitle").GetComponent<Text>();
        panelText = GameObject.Find("EntryText").GetComponent<Text>();
        entryName = GetComponentInChildren<Text>();
    }

    /// <summary>
    /// Get the associated text value of the Journal Entry
    /// </summary>
    public void RetrieveJournalEntry()
    {
        // Set note title, note value, and play journal entry sound
        panelTextTitle.text = associatedNote.NoteTitle;
        panelText.text = associatedNote.TextValue;

        // Play the journal entry selection sound
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.JournalEntrySelected, transform.position);
    }

    /// <summary>
    /// Change the color when the cursor hovers the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        entryName.color = Color.black;
    }

    /// <summary>
    /// Change the color when the cursor exits the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        entryName.color = new Color32(0x57, 0x57, 0x57, 0xFF); // dark gray
    }
}
