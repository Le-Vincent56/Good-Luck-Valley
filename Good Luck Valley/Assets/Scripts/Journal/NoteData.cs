using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    public enum Tab
    {
        Index = 0,
        Anari = 1,
        Hana = 2,
        Seiji = 3
    }

    [CreateAssetMenu(fileName = "Note Data")]
    public class NoteData : ScriptableObject
    {
        public int ID;          // Defines the placement in the journal
        public string Name;     // Name of the note
        public string Content;  // Text content of the note
        public Tab Tab;         // What tab it goes under
    }
}