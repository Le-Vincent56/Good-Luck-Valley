using HiveMind.SaveData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HiveMind.Environment
{
    public abstract class AnguishVine : MonoBehaviour, IData
    {
        #region FIELDS
        // Create unique ids for each note
        [SerializeField] protected string id;
        [ContextMenu("Generate GUID for ID")]
        protected void GenerateGuid()
        {
            id = System.Guid.NewGuid().ToString();
        }

        protected bool active = true;
        #endregion

        #region PROPERTIES
        public bool Active { get { return active; } set { active = value; } }
        #endregion

        #region DATA HANDLING
        public void LoadData(GameData data)
        {
            // Get the data for all the notes that have been collected
            string currentLevel = SceneManager.GetActiveScene().name;

            // Try to get the value of the of the vine
            data.levelData[currentLevel].assetsActive.TryGetValue(id, out active);

            // Set if the gameobject is active
            gameObject.SetActive(active);
        }

        public void SaveData(GameData data)
        {
            string currentLevel = SceneManager.GetActiveScene().name;

            // Check to see if data has the id of the note
            if (data.levelData[currentLevel].assetsActive.ContainsKey(id))
            {
                // If so, remove it
                data.levelData[currentLevel].assetsActive.Remove(id);
            }

            // Add the id and the current bool to make sure everything is up to date
            data.levelData[currentLevel].assetsActive.Add(id, active);
        }
        #endregion
    }
}
