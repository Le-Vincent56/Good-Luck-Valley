using GoodLuckValley.Patterns.Blackboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Level
{
    public class PitPlatformController : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private GameObject postMushroomObject;
        [SerializeField] private GameObject preMushroomObject;
        #endregion

        #region FIELDS
        private Blackboard playerBlackboard;
        private BlackboardKey unlockedThrow;
        #endregion

        private void Awake()
        {
            playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
            unlockedThrow = playerBlackboard.GetOrRegisterKey("UnlockedThrow");
            Debug.Log(playerBlackboard);
            playerBlackboard.TryGetValue(unlockedThrow, out bool unlockedThrowValue);
            if (unlockedThrowValue)
            {
                ShowPlatforms();
            }
        }

        private void ShowPlatforms()
        {
            if (postMushroomObject != null)
                postMushroomObject.SetActive(!postMushroomObject.activeSelf);
            if (preMushroomObject != null)
                preMushroomObject.SetActive(!preMushroomObject.activeSelf);
        }
    }
}