using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI
{
    public class SaveSlot : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private Text dateText;
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text emptyText;
        [SerializeField] private bool isEmpty;

        public string Date { get { return dateText.text; } }
        public string Name { get { return nameText.text; } }
        public string Level { get { return levelText.text; } }
        public bool IsEmpty { get { return isEmpty; } }
        #endregion

        /// <summary>
        /// Set the UI Data
        /// </summary>
        /// <param name="saveName">The name of the Save Slot</param>
        /// <param name="lastUpdated">When the Save Slot was last updated</param>
        /// <param name="levelName">The current level of the Save Slot</param>
        public void SetData(string saveName, string lastUpdated, string levelName)
        {
            // Set text
            dateText.text = lastUpdated;
            nameText.text = saveName;
            levelText.text = levelName;

            UpdateUI();
        }

        public void UpdateUI()
        {
            bool validData = true;

            // Check if any data was not sent
            if (dateText.text == "Date") validData = false;
            if (nameText.text.Contains("Save Slot")) validData = false;
            if (levelText.text == "Level Name") validData = false;

            // Check which data to set
            if (validData)
            {
                // If all data is valid, set the saved data as text
                dateText.gameObject.SetActive(true);
                nameText.gameObject.SetActive(true);
                levelText.gameObject.SetActive(true);

                // Set the emptyText to be inactive
                emptyText.gameObject.SetActive(false);

                isEmpty = false;
            }
            else
            {
                // If data is not valid, set all saved data Text's
                // to be inactive
                dateText.gameObject.SetActive(false);
                nameText.gameObject.SetActive(false);
                levelText.gameObject.SetActive(false);

                // Set the emptyText to be active
                emptyText.gameObject.SetActive(true);

                isEmpty = true;
            }
        }

        public void ResetData()
        {
            dateText.text = "Date";
            nameText.text = "Save Slot";
            levelText.text = "Level Name";
        }
    }
}