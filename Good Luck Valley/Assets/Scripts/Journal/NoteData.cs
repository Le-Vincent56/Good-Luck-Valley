using UnityEngine;

namespace GoodLuckValley.Journal
{
    public enum Tab
    {
        Hana = 1,
        Seiji = 2
    }

    [CreateAssetMenu(fileName = "Note Data", menuName = "Journal/Note Data")]
    public class NoteData : ScriptableObject
    {
        public int ID;          // Defines the placement in the journal
        public string Title;     // Name of the note
        public string Content;  // Text content of the note
        public Tab Tab;         // What tab it goes under

        public void Print()
        {
            Debug.Log($"Note ID: {ID}\n" +
                $"Note Title: {Title}\n" +
                $"Note Content: {Content}\n" +
                $"Note Tab: {Tab}");
        }
    }
}