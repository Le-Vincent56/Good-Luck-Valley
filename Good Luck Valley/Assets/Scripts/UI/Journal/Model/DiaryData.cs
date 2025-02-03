using UnityEngine;

namespace GoodLuckValley.UI.Journal.Model
{
    [CreateAssetMenu(fileName = "DiaryData", menuName = "Journal/Diary Data")]
    public class DiaryData : JournalData
    {
        public void OnEnable()
        {
            Tab = TabType.Diary;
        }
    }
}
