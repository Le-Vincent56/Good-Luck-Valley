using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryScrollview : MonoBehaviour
{
    public Journal journal;
    public GameObject entryPrefab;
    public ScrollRect scrollView;
    public RectTransform contentTransform;
    public List<GameObject> entries;


    // Start is called before the first frame update
    void Start()
    {
        journal = GameObject.Find("JournalUI").GetComponent<Journal>();
        scrollView = GameObject.Find("EntryPanel").GetComponent<ScrollRect>();
        contentTransform = GameObject.Find("Entries").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEntries()
    {
        foreach(Note note in journal.Notes)
        {
            bool noteAlreadyAdded = false;

            foreach(GameObject entries in entries)
            {
                if(entries.GetComponentInChildren<Text>().text == note.noteTitle)
                {
                    noteAlreadyAdded = true;
                }
            }

            if(!noteAlreadyAdded)
            {
                GameObject instance = Instantiate(entryPrefab, contentTransform);
                instance.GetComponent<RectTransform>().sizeDelta = new Vector2(385f, 40);
                instance.GetComponentInChildren<Text>().text = note.noteTitle;
                instance.GetComponentInChildren<EntryButton>().Note = note;
                entries.Add(instance);
            }
        }
    }
}
