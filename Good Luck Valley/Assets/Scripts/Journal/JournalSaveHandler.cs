using GoodLuckValley.Persistence;
using UnityEngine;

namespace GoodLuckValley.Journal.Persistence
{
    public class JournalSaveHandler : MonoBehaviour, IBind<JournalSaveData>
    {
        [SerializeField] private JournalSaveData data;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        // Update is called once per frame
        void Update()
        {
            // Update save data
            UpdateSaveData();
        }

        /// <summary>
        /// Update Journal save data
        /// </summary>
        public void UpdateSaveData()
        {
            data.unlocked = Journal.Instance.Unlocked;
            data.unlockedEntries = Journal.Instance.GetUnlockedEntriesList();
            data.progressingIndex = Journal.Instance.GetCurrentIndex();
            data.lastOpenedIndex = Journal.Instance.GetLastOpenedIndex();
            data.notesCollectedNum = Journal.Instance.GetNumOfNotesCollected();
            data.journalEntriesUnlocked = Journal.Instance.GetNumOfUnlockedEntries();
        }

        /// <summary>
        /// Force a save update
        /// </summary>
        public void ForceUpdate() => UpdateSaveData();

        /// <summary>
        /// Bind Journal data for persistence
        /// </summary>
        /// <param name="data"></param>
        public void Bind(JournalSaveData data, bool applyData = true)
        {
            this.data = data;
            this.data.ID = ID;

            if (applyData)
            {
                Journal.Instance.SetUnlocked(data.unlocked);
                Journal.Instance.SetUnlockedEntriesList(data.unlockedEntries);
                Journal.Instance.SetProgressingEntry(data.progressingIndex);
                Journal.Instance.SetLastOpenedEntry(data.lastOpenedIndex);
                Journal.Instance.SetNotesCollectedNum(data.notesCollectedNum);
                Journal.Instance.SetEntriesUnlockedNum(data.journalEntriesUnlocked);
                Journal.Instance.SetNotificationInput(false);
            }
        }
    }
}