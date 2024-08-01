using GoodLuckValley.Patterns.Blackboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Level
{
    public class SignLoader : MonoBehaviour
    {
        [SerializeField] private BlackboardData playerBlackboardData;
        [SerializeField] private GameObject beforeCave;
        [SerializeField] private GameObject afterCave;

        private void Awake()
        {
            // Check if the player has the mushroom from the blackboard data
            if (playerBlackboardData.entries[1].value == true)
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