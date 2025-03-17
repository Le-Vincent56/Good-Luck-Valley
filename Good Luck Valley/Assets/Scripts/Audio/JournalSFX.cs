using UnityEngine;

namespace GoodLuckValley.Audio
{
    public class JournalSFX : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event journalOpen;
        [SerializeField] private AK.Wwise.Event journalEntrySelectedEmpty;
        [SerializeField] private AK.Wwise.Event journalEntrySelectedFull;
        [SerializeField] private AK.Wwise.Event journalClose;

        /// <summary>
        /// Play the Open Journal SFX
        /// </summary>
        public void Open() => journalOpen.Post(gameObject);

        /// <summary>
        /// Play the Entry Selected SFX
        /// </summary>
        public void EntrySelected(bool isEmpty) => (isEmpty ? journalEntrySelectedEmpty : journalEntrySelectedFull).Post(gameObject);

        /// <summary>
        /// Play the Close Journal SFX
        /// </summary>
        public void Close() => journalClose.Post(gameObject);
    }
}
