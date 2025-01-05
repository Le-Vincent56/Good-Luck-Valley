using GoodLuckValley.Architecture.EventBus;
using GoodLuckValley.Input;
using GoodLuckValley.Player.Development.Events;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.Development
{
    public class DevelopmentTools : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        private PlayerController controller;

        [Header("General")]
        [SerializeField] private bool active;
        [SerializeField] private bool debug;

        [Header("NoClip")]
        [SerializeField] private bool noClip;
        [SerializeField] private float noClipSpeed = 10f;

        public bool NoClip { get => noClip; }

        private void Awake()
        {
            // Get components
            controller = GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            inputReader.Dev += ToggleDev;
            inputReader.NoClip += ToggleNoClip;
        }

        private void OnDisable()
        {
            inputReader.Dev -= ToggleDev;
            inputReader.NoClip -= ToggleNoClip;
        }

        private void Update()
        {
            // Exit case - Development Tools are not active
            if (!active) return;

            // Update noClip logic
            UpdateNoClip();
        }

        /// <summary>
        /// Update noClip logic
        /// </summary>
        private void UpdateNoClip()
        {
            // Exit case - noClip is not enabled
            if (!noClip) return;

            // Calculate the movement vector
            Vector2 movement = new Vector2(inputReader.NormMoveX * noClipSpeed * Time.deltaTime, inputReader.NormMoveY * noClipSpeed * Time.deltaTime);
            transform.Translate(movement);
        }

        /// <summary>
        /// Handle input for toggling Development Tools
        /// </summary>
        private void ToggleDev(bool started)
        {
            // Exit case - the button is being lifted
            if (!started) return;

            // Toggle Development Tools
            active = !active;

            // Check if Development Tools inactive
            if(!active)
            {
                // Toggle off any tools
                noClip = false;
                controller.Active = true;
            }

            // Raise the toggle devent
            EventBus<ToggleDevelopmentTools>.Raise(new ToggleDevelopmentTools()
            {
                Active = active,
                Debug = debug,
                NoClip = noClip
            });
        }

        /// <summary>
        /// Handle input for toggling noClip
        /// </summary>
        private void ToggleNoClip(bool started)
        {
            // Exit case - the button is being lifted
            if (!started) return;

            // Exit case - Developemnt Tools are not active
            if (!active) return;

            // Toggle noClip
            noClip = !noClip;
            controller.Active = !noClip;

            // Raise the change event
            EventBus<ChangeDevelopmentTools>.Raise(new ChangeDevelopmentTools() 
            { 
                Debug = debug, 
                NoClip = noClip 
            }); 
        }
    }
}
