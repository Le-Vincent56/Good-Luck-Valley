using GoodLuckValley.Patterns.Blackboard;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    public class GlobalDataSaveHandler : MonoBehaviour, IBind<GlobalData>
    {
        [Header("Saving")]
        [SerializeField] private GlobalData globalData;
        [SerializeField] private GlobalData initialData;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        private bool tryBind;
        private Blackboard playerBlackboard;
        private BlackboardKey unlockedThrowKey;
        private BlackboardKey unlockedWallJumpKey; 

        private void Start()
        {
            playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
            unlockedThrowKey = playerBlackboard.GetOrRegisterKey("UnlockedThrow");
            unlockedWallJumpKey = playerBlackboard.GetOrRegisterKey("UnlockedWallJump");
        }

        private void Update()
        {
            // Check if the data needs to be binded
            CheckBind();

            // Update the data
            SetGlobalDataBool(unlockedThrowKey, ref globalData.unlockedThrow);
            SetGlobalDataBool(unlockedWallJumpKey, ref globalData.unlockedWallJump);
        }

        /// <summary>
        /// Set global data for boolean values
        /// </summary>
        /// <param name="dataToGet">The data to retrieve from the blackboard</param>
        /// <param name="dataToSet">The data to change from the current blackboard value</param>
        private void SetGlobalDataBool(BlackboardKey dataToGet, ref bool dataToSet)
        {
            if (playerBlackboard.TryGetValue(dataToGet, out bool blackboardValue))
                dataToSet = blackboardValue;
        }

        /// <summary>
        /// Validate the Blackboard
        /// </summary>
        private void ValidateBlackboard()
        {
            if (playerBlackboard == null)
                playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");

            if (unlockedThrowKey == null)
                unlockedThrowKey = playerBlackboard.GetOrRegisterKey("UnlockedThrow");

            if (unlockedWallJumpKey == null)
                unlockedWallJumpKey = playerBlackboard.GetOrRegisterKey("UnlockedWallJump");
        }

        /// <summary>
        /// Check whether or not the data needs to bind
        /// </summary>
        private void CheckBind()
        {
            // If not trying to bind, return
            if (!tryBind) return;

            // Set throw unlock data
            if (playerBlackboard.TryGetValue(unlockedThrowKey, out bool unlockedThrowValue))
                playerBlackboard.SetValue(unlockedThrowKey, initialData.unlockedThrow);

            // Set wall jump unlock data
            if (playerBlackboard.TryGetValue(unlockedWallJumpKey, out bool unlockedWallJumpValue))
                playerBlackboard.SetValue(unlockedWallJumpKey, initialData.unlockedWallJump);

            // Stop trying to bind
            tryBind = false;
        }

        /// <summary>
        /// Bind the global data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="applyData"></param>
        public void Bind(GlobalData data, bool applyData = true)
        {
            globalData = data;
            globalData.ID = data.ID;

            // Validate the blackboard
            ValidateBlackboard();

            // Set initial data and try to bind
            tryBind = true;
            initialData = data;
        }
    }
}