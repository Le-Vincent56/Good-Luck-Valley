using HiveMind.SaveData;
using UnityEngine;
using UnityEngine.UI;

namespace HiveMind.Menus
{
    public class SaveSlot : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameObject noDataContent;
        [SerializeField] private GameObject hasDataContent;
        [SerializeField] private Text playtime;
        [SerializeField] private Text levelName;
        private Button saveSlotButton;
        private bool hasData = true;
        [SerializeField] private bool selected = false;
        #endregion

        #region FIELDS
        [SerializeField] private string profileID = "";
        public bool HasData { get { return hasData; } }
        public bool Selected { get { return selected; } set { selected = value; } }
        #endregion

        private void Start()
        {
            // Some references set in Inspector
            saveSlotButton = GetComponent<Button>();
        }

        /// <summary>
        /// Set the data of the save slot according to a GameData object
        /// </summary>
        /// <param name="data">The GameDatObject to set the data from</param>
        public void SetData(GameData data)
        {
            // Check if data exists for this save slot
            if (data == null)
            {
                // If there is no data, then set noDataContent UI
                hasData = false;
                noDataContent.SetActive(true);
                hasDataContent.SetActive(false);
            }
            else
            {
                // If there is, then set hasDataContent UI
                hasData = true;
                noDataContent.SetActive(false);
                hasDataContent.SetActive(true);

                levelName.text = data.currentLevelName;
                playtime.text = data.playtimeString;
            }
        }

        /// <summary>
        /// Get the profile ID of the save slot
        /// </summary>
        /// <returns>The profile ID of the save slot</returns>
        public string GetProfileID()
        {
            return profileID;
        }

        /// <summary>
        /// Set the interactability of the save slot's button
        /// </summary>
        /// <param name="interactable">True to turn on interactability, false to turn off interactability</param>
        public void SetInteractable(bool interactable)
        {
            saveSlotButton.interactable = interactable;
        }
    }
}

