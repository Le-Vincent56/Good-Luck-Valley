using UnityEngine;

namespace GoodLuckValley.UI.Journal.Model
{
    [CreateAssetMenu(fileName = "NoteData", menuName = "Journal/Note Data")]
    public class NoteData : JournalData
    {
        public void OnEnable()
        {
            Tab = TabType.Note;
        }
    }
}
