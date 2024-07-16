using GoodLuckValley.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.Journal.UI
{
    public class InspectWindow : FadePanel
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text contentText;

        public void SetEntry(JournalEntry entryToSet)
        {
            titleText.text = entryToSet.Title;
            contentText.text = entryToSet.Content;
        }
    }
}