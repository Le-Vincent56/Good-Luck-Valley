using HiveMind.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiveMind.NoteJournal
{
    public class EntryScrollview : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private JournalScriptableObj journalEvent;
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

        private void OnEnable()
        {
            // Subscribe to journal events
            journalEvent.refreshJournalEvent.AddListener(SetEntries);
            journalEvent.clearJournalEvent.AddListener(RemoveEntries);
        }

        private void OnDisable()
        {
            // Unsubscribe to journal events
            journalEvent.refreshJournalEvent.RemoveListener(SetEntries);
            journalEvent.clearJournalEvent.RemoveListener(RemoveEntries);
        }

        // Start is called before the first frame update
        void Start()
        {
            scrollView = GameObject.Find("EntryPanel").GetComponent<ScrollRect>();
            contentTransform = GameObject.Find("Entries").GetComponent<RectTransform>();
        }

        /// <summary>
        /// Add collected journal entries into the journal
        /// </summary>
        /// <param name="journalToRefresh"></param>
        public void SetEntries(Journal journalToRefresh)
        {
            foreach (Note note in journalToRefresh.Notes)
            {
                bool noteAlreadyAdded = false;

                foreach (GameObject entries in entries)
                {
                    if (entries.GetComponentInChildren<Text>().text == note.ContentsTitle)
                    {
                        noteAlreadyAdded = true;
                    }
                }

                if (!noteAlreadyAdded)
                {
                    GameObject instance = Instantiate(entryPrefab, contentTransform);
                    instance.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 80f);
                    instance.GetComponentInChildren<Text>().text = note.ContentsTitle;

                    // If the note hasn't been read, highlight it
                    if (!note.AlreadyRead)
                    {
                        instance.GetComponentInChildren<Text>().color = Color.white;
                    }
                    else
                    {
                        instance.GetComponentInChildren<Text>().color = new Color32(0x57, 0x57, 0x57, 0xFF);
                    }

                    instance.GetComponentInChildren<EntryButton>().Note = note;
                    entries.Add(instance);
                }
            }
        }

        /// <summary>
        /// Remove journal entries from the journal
        /// </summary>
        public void RemoveEntries()
        {
            // Add all entries to the destroyedEntries list
            foreach (GameObject entry in entries)
            {
                destroyedEntries.Add(entry);
            }

            // Check if there are any destroyed entries
            if (destroyedEntries.Count > 0)
            {
                // If so, compare the destroyedEntries list to the entries list
                for (int i = 0; i < destroyedEntries.Count; i++)
                {
                    for (int j = 0; j < entries.Count; j++)
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
}
