using GoodLuckValley.Events;
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
        [SerializeField] private GameEvent onAddMushroom;
        [SerializeField] private GameEvent onGetLaunchForce;
        [SerializeField] private GameEvent onEnableThrowUI;
        [SerializeField] private GameEvent onDisableThrowUI;

        [Header("Prefabs")]
        [SerializeField] private GameObject spore;
        #endregion

        #region FIELDS
        [SerializeField] private Vector2 launchForce;
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
        /// Set the launch force for the throw
        /// </summary>
        /// <param name="launchForce"></param>
        public void SetLaunchForce(Vector2 launchForce)
        {
            this.launchForce = launchForce;
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
            // - ThrowLine.SetLaunchForce()
            onGetLaunchForce.Raise(this, null);

            // Disable throw UI
            // Calls to:
            //  - ThrowLine.HideLine()
            onDisableThrowUI.Raise(this, false);

            // Create spore throw data
            SporeData sporeData = new SporeData(launchForce, spore);

            // Add a mushroom to the list
            //  - Calls MushroomTracker.AddMushroom()
            onAddMushroom.Raise(this, sporeData);
        }

        /// <summary>
        /// Input handler for throwing
        /// </summary>
        /// <param name="context"></param>
        public void OnThrow(InputAction.CallbackContext context)
        {
            if (!throwUnlocked || !canThrow) return;

            // Aim on hold press
            if (context.started)
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
            if (context.canceled)
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
