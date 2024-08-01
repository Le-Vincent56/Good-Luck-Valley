using GoodLuckValley.Persistence;
using GoodLuckValley.SceneManagement;
using UnityEngine;

namespace GoodLuckValley.Player.Control
{
    public class PlayerSaveHandler : MonoBehaviour, IBind<PlayerSaveData>
    {
        [SerializeField] private PlayerSaveData data;
        [SerializeField] private bool trySave;
        [SerializeField] private bool fromSceneLoader;
        [SerializeField] private float saveTimer;
        [SerializeField] private float saveBuffer;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        private void Awake()
        {
            SaveLoadSystem.Instance.BindData();
        }

        // Update is called once per frame
        void Update()
        {
            // Update save data
            UpdateSaveData();

            //if (trySave)
            //{
            //    SaveLoadSystem.Instance.SaveGame();
            //    trySave = false;
            //    saveTimer = saveBuffer;
            //    return;
            //}

            //if(saveTimer > 0)
            //{
            //    saveTimer -= Time.deltaTime;
            //    return;
            //}

            //if(fromSceneLoader)
            //{
            //    SceneLoader.Instance.FinalizeLevel();
            //    fromSceneLoader = false;
            //    return;
            //}
        }

        public void UpdateSaveData()
        {
            // Save transform data
            data.position = transform.position;
        }

        /// <summary>
        /// Force a save update
        /// </summary>
        public void LevelPositionUpdate()
        {
            UpdateSaveData();

            trySave = true;
            fromSceneLoader = true;
        }

        /// <summary>
        /// Bind Player data for persistence
        /// </summary>
        /// <param name="data"></param>
        public void Bind(PlayerSaveData data, bool applyData = true)
        {
            this.data = data;
            this.data.ID = ID;

            if (applyData)
            {
                transform.position = data.position;
            }
        }
    }
}