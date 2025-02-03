using UnityEngine;

namespace GoodLuckValley.UI.Journal.Model
{
    [CreateAssetMenu(fileName = "RecordingData", menuName = "Journal/Recording Data")]
    public class RecordingData : JournalData
    {
        // TODO: Wwise Event

        public void OnEnable()
        {
            Tab = TabType.Recording;
        }
    }
}
