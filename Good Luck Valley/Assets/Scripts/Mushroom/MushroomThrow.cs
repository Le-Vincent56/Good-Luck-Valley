using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Mushroom
{
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

        [Header("Prefabs")]
        [SerializeField] private GameObject spore;
        #endregion

        #region FIELDS
        private ThrowState throwState;
        private bool canThrow;
        private bool throwLineOn;
        private float throwCooldown;
        private float bounceCooldown;
        private bool throwPrepared;
        private bool throwUnlocked;
        [SerializeField] bool throwing = false;
        [SerializeField] float throwAnimTimer = 2f;
        #endregion

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

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

            // Hide the UI line
            //DeleteThrowLine();

            // Add a mushroom to the list
            //  - Calls MushroomTracker.AddMushroom()
            onAddMushroom.Raise(this, spore);
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (!throwUnlocked || !canThrow) return;

            // Aim on hold press
            if (context.started)
            {
                // If not throwing, set to throwing
                if (throwState == ThrowState.NotThrowing)
                    throwState = ThrowState.Throwing;
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
