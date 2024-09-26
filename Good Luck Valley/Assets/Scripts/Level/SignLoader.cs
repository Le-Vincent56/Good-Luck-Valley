using GoodLuckValley.Patterns.Blackboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Level
{
    public class SignLoader : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameObject beforeCave;
        [SerializeField] private GameObject afterCave;
        #endregion

        #region FIELDS
        private Blackboard playerBlackboard;
        private BlackboardKey unlockedThrow;
        #endregion

        private void Awake()
        {
            playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
            unlockedThrow = playerBlackboard.GetOrRegisterKey("UnlockedThrow");

            // Check if the player has the mushroom from the blackboard data
            if (playerBlackboard.TryGetValue(unlockedThrow, out bool unlockedThrowValue))
            {
                // If so, activate the 'after cave' signs
                ActivateSigns(false, true);
            }
            else
            {
                // If not, activate the 'before cave' signs
                ActivateSigns(true, false);
            }
        }

        /// <summary>
        /// Helper function to set the active state of the sign gameobject
        /// </summary>
        /// <param name="beforeCaveActie"> Whether the before cave sign is active or not </param>
        /// <param name="afterCaveActive"> Whether the after cave sign is active or not </param>
        private void ActivateSigns(bool beforeCaveActie, bool afterCaveActive)
        {
            if (afterCave != null)
                afterCave.SetActive(afterCaveActive);
            if (beforeCave != null)
                beforeCave.SetActive(beforeCaveActie);
        }
    }
}