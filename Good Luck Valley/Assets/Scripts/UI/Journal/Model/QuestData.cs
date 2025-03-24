using UnityEngine;

namespace GoodLuckValley.UI.Journal.Model
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Journal/Quest Data")]
    public class QuestData : JournalData
    {
        public void OnEnable()
        {
            Tab = TabType.Quest;
        }
    }
}
