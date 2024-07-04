using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.Patterns.ServiceLocator;
using GoodLuckValley.Player.Input;
using GoodLuckValley.VFX;
using UnityEngine;
using UnityEngine.VFX;

namespace GoodLuckValley.Mushroom
{
    public struct ShroomSpawnData
    {
        public Vector2 Point;
        public int Rotation;
        public bool Valid;

        public ShroomSpawnData(Vector2 point, int rotation, bool valid)
        {
            Point = point;
            Rotation = rotation;
            Valid = valid;
        }
    }

    public class MushroomThrow : MonoBehaviour
    {
        public enum ThrowState
        {
            NotThrowing,
            Throwing
        }

        #region REFERENCES
        [Header("Events")]
        [SerializeField] private GameEvent onRequestThrowUnlock;
        [SerializeField] private GameEvent onGetCountToLimit;
        [SerializeField] private GameEvent onRemoveFirstShroom;
        [SerializeField] private GameEvent onGetThrowDirection;
        [SerializeField] private GameEvent onGetSpawnData;
        [SerializeField] private GameEvent onGetFinalSpawnInfo;
        [SerializeField] private GameEvent onEnableThrowUI;
        [SerializeField] private GameEvent onDisableThrowUI;
        [SerializeField] private GameEvent onSetThrowing;

        [Header("Prefabs")]
        [SerializeField] private GameObject spore;
        #endregion

        #region FIELDS
        [SerializeField] private Vector2 throwDirection;
        [SerializeField] private ShroomSpawnData spawnData;
        [SerializeField] private FinalSpawnInfo finalSpawnInfo;
        [SerializeField] private float throwMultiplier;
        [SerializeField] private bool canThrow;
        [SerializeField] private ThrowState throwState;
        [SerializeField] private VisualEffect sporeVFX;
        private Blackboard playerBlackboard;
        private BlackboardKey unlockedThrow;
        private BlackboardKey isCrawling;
        #endregion

        private void Start()
        {
            // Register blackboards and keys
            playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
            unlockedThrow = playerBlackboard.GetOrRegisterKey("UnlockedThrow");
            isCrawling = playerBlackboard.GetOrRegisterKey("IsCrawling");
        }

        /// <summary>
        /// Set the throw direction for the spore
        /// </summary>
        /// <param name="throwDirection">The direction to throw the spore</param>
        public void SetThrowDirection(Vector2 throwDirection)
        {
            this.throwDirection = throwDirection;
        }

        /// <summary>
        /// Set the spawn data for the spore
        /// </summary>
        /// <param name="data">The ShroomSpawnData for the spore for Mushroom spawning</param>
        public void SetSpawnData(ShroomSpawnData data)
        {
            spawnData = data;
        }

        public void SetFinalSpawnInfo(FinalSpawnInfo finalSpawnInfo)
        {
            this.finalSpawnInfo = finalSpawnInfo;
        }

        /// <summary>
        /// Handle throwing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void HandleThrow(Component sender, object data)
        {
            // Check if the data is the correct type
            if (data is not bool) return;

            // Get line collision data
            // Calls to:
            // - ThrowLine.GetSpawnData();
            onGetSpawnData.Raise(this, null);

            // Check if the spawn point is valid
            if (!spawnData.Valid)
            {
                // Disable throw UI if not
                // Calls to:
                //  - ThrowLine.HideLine();
                onDisableThrowUI.Raise(this, false);

                // Exit early to prevent shroom throwing
                return;
            }

            // Cast data
            bool atOrOverLimit = (bool)data;

            // Check if at or over the mushroom limit
            if (atOrOverLimit)
            {
                // Remove the first shroom
                // Calls to:
                //  - MushroomTracker.RemoveFirstShroom()
                onRemoveFirstShroom.Raise(this, null);
            }

            // Reset throwing
            if (throwState == ThrowState.Throwing)
                throwState = ThrowState.NotThrowing;

            // Get the throw direction
            // Calls to:
            //  - ThrowLine.GetThrowDirection();
            onGetThrowDirection.Raise(this, null);

            // Disable throw UI
            // Calls to:
            //  - ThrowLine.HideLine();
            onDisableThrowUI.Raise(this, false);

            // Get final spawn info
            // Calls to:
            //  - MushroomIndicator.GetFinalSpawnInfo();
            onGetFinalSpawnInfo.Raise(this, null);

            // Create a spore and set collision data
            GameObject newSpore = Instantiate(spore, transform.position, Quaternion.identity);
            newSpore.GetComponent<Spore>().SetSpawnInfo(finalSpawnInfo);
            newSpore.GetComponent<Spore>().ThrowSpore(throwDirection);

            // Set throwing for the PlayerController
            // Calls to:
            //  - PlayerController.SetThrow();
            onSetThrowing.Raise(this, true);
        }

        /// <summary>
        /// Handle Throw Input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OnThrow(Component sender, object data)
        {
            // Check if the player has unlocked the spirit power
            if (playerBlackboard.TryGetValue(unlockedThrow, out bool unlockValue) && !unlockValue) return;

            // Check if the player is crawling
            if (playerBlackboard.TryGetValue(isCrawling, out bool crawlingValue) && crawlingValue) return;

            // Check if the data type is correct
            if (data is not ContextData) return;

            // Cast data
            ContextData contextData = (ContextData)data;

            // Aim on hold press
            if(contextData.Started)
            {
                // If not throwing, set to throwing
                if (throwState == ThrowState.NotThrowing)
                    throwState = ThrowState.Throwing;

                // Enable throw UI
                // Calls to:
                //  - ThrowLine.ShowLine()
                onEnableThrowUI.Raise(this, true);
            }

            // Throw on release
            if(contextData.Canceled)
            {
                // Check if can throw updated
                if (!canThrow || throwState != ThrowState.Throwing) return;

                // Get the count to limit
                // Calls to:
                //  - MushroomTracker.CheckCountLimit()
                onGetCountToLimit.Raise(this, null);
            }
        }

        /// <summary>
        /// Cancel the mushroom throw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void CancelThrow(Component sender, object data)
        {
            // Check if already throwing
            if (throwState == ThrowState.Throwing)
            {
                // Set throw state to "not throwing"
                throwState = ThrowState.NotThrowing;

                // Disable the throw UI
                onDisableThrowUI.Raise(this, false);
            }
        }
    }
}
