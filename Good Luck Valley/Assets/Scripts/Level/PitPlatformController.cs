using GoodLuckValley.Patterns.Blackboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Level
{
    public class PitPlatformController : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameObject postMushroomGrid;
        [SerializeField] private GameObject postMushroomPlatformAssets;
        [SerializeField] private GameObject preMushroomGrid;
        #endregion

        #region FIELDS
        private Blackboard playerBlackboard;
        private BlackboardKey unlockedThrow;
        #endregion

        private void Awake()
        {
            playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
            unlockedThrow = playerBlackboard.GetOrRegisterKey("UnlockedThrow");

            if (playerBlackboard.TryGetValue(unlockedThrow, out bool unlockedThrowValue))
            {
                ShowPlatforms();
            }
        }

        private void ShowPlatforms()
        {
            if (postMushroomGrid != null)
                postMushroomGrid.SetActive(true);
            if (postMushroomPlatformAssets != null)
                postMushroomPlatformAssets.SetActive(true);
            if (preMushroomGrid != null)
                preMushroomGrid.SetActive(false);
        }
    }
}