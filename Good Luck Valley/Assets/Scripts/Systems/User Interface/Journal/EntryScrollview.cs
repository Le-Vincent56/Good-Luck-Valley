using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryScrollview : MonoBehaviour
{
    #region REFERENCES
    public Journal journal;
    public GameObject entryPrefab;
    public ScrollRect scrollView;
    public RectTransform contentTransform;
    #endregion

    #region FIELDS
    [SerializeField] private List<GameObject> entries;
    #endregion

    #region PROPERTIES
    public List<GameObject> Entries { get { return entries; } set { entries = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        journal = GameObject.Find("JournalUI").GetComponent<Journal>();
        scrollView = GameObject.Find("EntryPanel").GetComponent<ScrollRect>();
        contentTransform = GameObject.Find("Entries").GetComponent<RectTransform>();
    }

    public void SetEntries()
    {
        foreach(Note note in journal.Notes)
        {
            bool noteAlreadyAdded = false;

            foreach(GameObject entries in entries)
            {
                if(entries.GetComponentInChildren<Text>().text == note.NoteTitle)
                {
                    noteAlreadyAdded = true;
                }
            }

            if(!noteAlreadyAdded)
            {
                GameObject instance = Instantiate(entryPrefab, contentTransform);
                instance.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 80f);
                instance.GetComponentInChildren<Text>().text = note.NoteTitle;
                instance.GetComponentInChildren<EntryButton>().Note = note;
                entries.Add(instance);
            }
        }
    }
}
