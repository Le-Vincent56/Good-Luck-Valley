using UnityEngine;

namespace GoodLuckValley.UI.Journal.Model
{
    public enum TabType
    {
        Diary,
        Notes,
        Recordings,
        Quests
    }
    
    public class JournalData : ScriptableObject
    {
        public TabType Tab;
        public int Index;
    }

    [CreateAssetMenu(fileName = "DiaryData", menuName = "Journal/Diary Data")]
    public class DiaryData : JournalData
    {
        public string Title;
        public string Content;
    }

    [CreateAssetMenu(fileName = "NoteData", menuName = "Journal/Note Data")]
    public class NoteData : JournalData
    {
        public string Title;
        public string Content;
    }

    [CreateAssetMenu(fileName = "RecordingData", menuName = "Journal/Recording Data")]
    public class RecordingData : JournalData
    {
        public string Title;
        public string Subtitles;
        // TODO: Wwise Event
    }
}
