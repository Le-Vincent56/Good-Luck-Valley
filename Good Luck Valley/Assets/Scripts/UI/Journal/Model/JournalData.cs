using UnityEngine;

namespace GoodLuckValley.UI.Journal.Model
{
    public enum TabType
    {
        None,
        Diary,
        Note,
        Recording,
        Quest
    }
    
    public class JournalData : ScriptableObject
    {
        public TabType Tab;
        public int Index;
        public string Title;
        public string Content;
    }
}
