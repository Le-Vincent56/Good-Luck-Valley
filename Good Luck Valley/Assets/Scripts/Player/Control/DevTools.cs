using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.Patterns.ServiceLocator;
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
        [SerializeField] private bool developerConsole = false;
        private Blackboard playerBlackboard;
        private BlackboardKey unlockedThrow;
        private BlackboardKey unlockedWallJump;

        public bool Active { get { return usingDevTools; } }
        public bool NoClip { get { return noClipActive; } }

        private void OnEnable()
        {
            input.DevTools += OnDevTools;
            input.NoClip += OnNoClip;
            input.UnlockPowers += OnUnlockPowers;
            input.DeveloperConsole += ToggleDeveloperConsole;
        }

        private void OnDisable()
        {
            input.DevTools -= OnDevTools;
            input.NoClip -= OnNoClip;
            input.UnlockPowers -= OnUnlockPowers;
            input.DeveloperConsole -= ToggleDeveloperConsole;
        }

        private void Start()
        {
            // Register Blackboard and get keys
            playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
            unlockedThrow = playerBlackboard.GetOrRegisterKey("UnlockedThrow");
            unlockedWallJump = playerBlackboard.GetOrRegisterKey("UnlockedWallJump");
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
                if (playerBlackboard.TryGetValue(unlockedThrow, out bool unlockValue))
                    playerBlackboard.SetValue(unlockedThrow, powersUnlocked);

                if (playerBlackboard.TryGetValue(unlockedWallJump, out bool wallJumpValue))
                    playerBlackboard.SetValue(unlockedWallJump, powersUnlocked);

                // Update UI
                onUpdateDevToolsUI.Raise(this, new Data(noClipActive, powersUnlocked));
            }
        }

        /// <summary>
        /// Handle showing the developer console
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        private void ToggleDeveloperConsole(bool started)
        {
            if(started)
            {
                developerConsole = !developerConsole;

                // Show the developer console
                Debug.developerConsoleVisible = developerConsole;
            }
        }
    }
}