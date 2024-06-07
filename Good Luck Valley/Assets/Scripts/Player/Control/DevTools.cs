using GoodLuckValley.Events;
using GoodLuckValley.Player.Input;
using UnityEngine;

namespace GoodLuckValley.Player.Control
{
    public class DevTools : MonoBehaviour
    {
        public struct Data
        {
            public bool NoClip;
            public bool PowersUnlocked;

            public Data(bool noClip, bool powersUnlocked)
            {
                NoClip = noClip;
                PowersUnlocked = powersUnlocked;
            }
        }

        [Header("Events")]
        [SerializeField] private GameEvent onUnlockPowers;
        [SerializeField] private GameEvent onToggleDevToolsUI;
        [SerializeField] private GameEvent onUpdateDevToolsUI;

        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private Transform playerTransform;

        [Header("Fields")]
        [SerializeField] private bool usingDevTools = false;
        [SerializeField] private bool noClipActive = false;
        [SerializeField] private float noClipSpeed;
        [SerializeField] private bool powersUnlocked = false;

        public bool Active { get { return usingDevTools; } }
        public bool NoClip { get { return noClipActive; } }

        private void OnEnable()
        {
            input.DevTools += OnDevTools;
            input.NoClip += OnNoClip;
            input.UnlockPowers += OnUnlockPowers;
        }

        private void OnDisable()
        {
            input.DevTools -= OnDevTools;
            input.NoClip -= OnNoClip;
            input.UnlockPowers -= OnUnlockPowers;
        }

        /// <summary>
        /// Handle no clip movement
        /// </summary>
        public void HandleNoClip()
        {
            // Move around in no clip
            Vector2 movement = new Vector2(input.NormMoveX * noClipSpeed, input.NormMoveY * noClipSpeed);
            playerTransform.Translate(movement);
        }

        /// <summary>
        /// Handle Dev Tools input
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        private void OnDevTools(bool started)
        {
            if(started)
            {
                // Toggle DevTools
                usingDevTools = !usingDevTools;

                onToggleDevToolsUI.Raise(this, usingDevTools);
            }
        }

        /// <summary>
        /// Handle NoClip input
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        private void OnNoClip(bool started)
        {
            // Check if the button has been pressed once
            // and that dev tools are active
            if(started && usingDevTools)
            {
                // Toggle no clip
                noClipActive = !noClipActive;

                // Update UI
                onUpdateDevToolsUI.Raise(this, new Data(noClipActive, powersUnlocked));
            }
        }

        /// <summary>
        /// Handle Unlock Powers input
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        private void OnUnlockPowers(bool started)
        {
            // Check if the button has been pressed once
            // and that dev tools are active
            if (started && usingDevTools)
            {
                // Toggle powers
                powersUnlocked = !powersUnlocked;

                // Unlock powers
                onUnlockPowers.Raise(this, powersUnlocked);

                // Update UI
                onUpdateDevToolsUI.Raise(this, new Data(noClipActive, powersUnlocked));
            }
        }
    }
}