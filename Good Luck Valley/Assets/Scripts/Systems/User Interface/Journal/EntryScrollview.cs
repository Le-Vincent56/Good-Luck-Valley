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
    [SerializeField] private List<GameObject> destroyedEntries;
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
                if(entries.GetComponentInChildren<Text>().text == note.ContentsTitle)
                {
                    noteAlreadyAdded = true;
                }
            }

            if(!noteAlreadyAdded)
            {
                GameObject instance = Instantiate(entryPrefab, contentTransform);
                instance.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 80f);
                instance.GetComponentInChildren<Text>().text = note.ContentsTitle;
                instance.GetComponentInChildren<EntryButton>().Note = note;
                entries.Add(instance);
            }
        }
    }

    public void RemoveEntries()
    {
        // Add all entries to the destroyedEntries list
        foreach (GameObject entry in entries)
        {
            destroyedEntries.Add(entry);
        }

        // Check if there are any destroyed entries
        if(destroyedEntries.Count > 0)
        {
            // If so, compare the destroyedEntries list to the entries list
            for(int i = 0; i < destroyedEntries.Count; i++)
            {
                for(int j = 0; j < entries.Count; j++)
                {
                    // If an entry equals a destroyed entry, destroy it from the scene
                    if (destroyedEntries[i].Equals(entries[j]))
                    {
                        Destroy(entries[j]);
                        entries.Remove(entries[j]);
                    }
                }
            }
        }

        // Clear the destroyedEntries list and the entries list
        entries.Clear();
        destroyedEntries.Clear();
    }

}
