using GoodLuckValley.Events;
using GoodLuckValley.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Mushroom
{
    public struct SporeData
    {
        public Vector2 LaunchForce;
        public GameObject Spore;

        public SporeData(Vector2 launchForce, GameObject spore)
        {
            LaunchForce = launchForce;
            Spore = spore;
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
        [SerializeField] private GameEvent onGetCountToLimit;
        [SerializeField] private GameEvent onRemoveFirstShroom;
        [SerializeField] private GameEvent onGetThrowPath;
        [SerializeField] private GameEvent onEnableThrowUI;
        [SerializeField] private GameEvent onDisableThrowUI;

        [Header("Prefabs")]
        [SerializeField] private GameObject spore;
        #endregion

        #region FIELDS
        [SerializeField] private Vector2 throwDirection;
        [SerializeField] private List<Vector2> path;
        [SerializeField] private float throwMultiplier;
        [SerializeField] private bool throwUnlocked;
        [SerializeField] private bool canThrow;
        [SerializeField] private ThrowState throwState;
        #endregion

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Set the path for the spore
        /// </summary>
        /// <param name="launchForce"></param>
        public void SetPath(List<Vector2> path)
        {
            this.path = path;
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

            // Get launch force
            // Calls to:
            // - ThrowLine.GetThrowDirection()
            onGetThrowPath.Raise(this, null);

            // Disable throw UI
            // Calls to:
            //  - ThrowLine.HideLine()
            onDisableThrowUI.Raise(this, false);

            // Create a spore and apply force
            GameObject newSpore = Instantiate(spore, transform.position, Quaternion.identity);
            newSpore.GetComponent<Spore>().SetPath(path);
        }

        public void OnThrow(Component sender, object data)
        {
            // Exit case - throw not unlocked, or the player cannot throw
            if (!throwUnlocked || !canThrow) return;

            // Check if the data type is correct
            if (data is not PlayerInputHandler.ContextData) return;

            // Cast data
            PlayerInputHandler.ContextData contextData = (PlayerInputHandler.ContextData)data;

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
                if (!canThrow) return;

                // Get the count to limit
                // Calls to:
                //  - MushroomTracker.CheckCountLimit()
                onGetCountToLimit.Raise(this, null);
            }
        }
    }
}
