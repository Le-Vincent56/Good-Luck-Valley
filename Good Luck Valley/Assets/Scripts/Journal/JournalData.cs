using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    [CreateAssetMenu(fileName = "Tab Data")]
    public class TabData : ScriptableObject
    {
        public int ID;
        public string Name;
        public float Duration;
    }

    [CreateAssetMenu(fileName = "Note Data")]
    public class NoteData : ScriptableObject
    {
        public int ID;
        public string Name;
        public string Content;
        public int Tab;
    }
}