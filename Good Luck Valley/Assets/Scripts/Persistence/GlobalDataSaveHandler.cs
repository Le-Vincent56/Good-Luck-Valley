using GoodLuckValley.Patterns.Blackboard;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    public class GlobalDataSaveHandler : MonoBehaviour, IBind<GlobalData>
    {
        [Header("Saving")]
        [SerializeField] private GlobalData globalData;
        [SerializeField] private GlobalData initialData;
        [SerializeField] private bool registered;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        private Blackboard playerBlackboard;
        private BlackboardKey unlockedThrowKey;
        private BlackboardKey unlockedWallJumpKey; 

        private void Start()
        {
            if (!registered)
                RegisterKeys();
        }

        /// <summary>
        /// Save global data
        /// </summary>
        public void SaveData(Component sender, object data)
        {
            globalData.unlockedThrow = GetData(unlockedThrowKey);
            globalData.unlockedWallJump = GetData(unlockedWallJumpKey);
        }

        /// <summary>
        /// Retrieve data from a Blackboard using a Key
        /// </summary>
        /// <param name="key">The Key of the value to retrieve from the Blackboard</param>
        /// <returns>A boolean value</returns>
        private bool GetData(BlackboardKey key)
        {
            // Try to get the value out of the blackboard
            if (playerBlackboard.TryGetValue(key, out bool blackboardValue))
            {
                // Return the value
                return blackboardValue;
            }

            // Otherwise, return false
            return false;
        }

        /// <summary>
        /// Register the Blackboard and relevant keys
        /// </summary>
        private void RegisterKeys()
        {
            playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
            unlockedThrowKey = playerBlackboard.GetOrRegisterKey("UnlockedThrow");
            unlockedWallJumpKey = playerBlackboard.GetOrRegisterKey("UnlockedWallJump");

            registered = true;
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

            // Register keys if they haven't been registered yet
            if (!registered)
                RegisterKeys();

            // Check whether or not to apply data
            if(applyData)
            {
                // Set Blackboard values
                playerBlackboard.SetValue(unlockedThrowKey, data.unlockedThrow);
                playerBlackboard.SetValue(unlockedWallJumpKey, data.unlockedWallJump);
            }
        }
    }
}