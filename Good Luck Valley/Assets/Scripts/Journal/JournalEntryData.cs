using UnityEngine;

namespace GoodLuckValley.Journal
{
    public enum Tab
    {
        Hana = 0,
        Seiji = 1
    }

    [CreateAssetMenu(fileName = "EntryData", menuName = "Journal/Entry Data")]
    public class JournalEntryData : ScriptableObject
    {
        public int ID;                              // Defines the placement in the journal
        public string Title;                        // Name of the note
        public string[] TotalContent = new string[3];    // Content of the journal
        public Tab Tab;                             // What tab it goes under

        public void Print()
        {
            Debug.Log($"Journal Entry ID: {ID}\n" +
                $"Journal Entry Title: {Title}\n" +
                $"Journal Entry Tab: {Tab}");
        }
    }
}