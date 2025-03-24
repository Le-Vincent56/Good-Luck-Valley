using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Input;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.Input
{
    public class CanvasInput : MonoBehaviour, ITransmutableInputUI
    {
        [Header("References")]
        [SerializeField] private ControlSchemeDetector controlSchemeDetector;

        [Header("Transmutables")]
        [SerializeField] private CanvasGroup keyboardAndMouseGroup;
        [SerializeField] private CanvasGroup xboxControllerGroup;
        [SerializeField] private CanvasGroup playstationGroup;
        private List<CanvasGroup> canvasGroups;

        private void Awake()
        {
            // Create the Canvas Group List
            canvasGroups = new List<CanvasGroup>
            {
                keyboardAndMouseGroup,
                xboxControllerGroup,
                playstationGroup
            };
        }

        private void OnEnable()
        {
            // Get the control scheme detector and register to it
            controlSchemeDetector = ServiceLocator.Global.Get<ControlSchemeDetector>();
            controlSchemeDetector.Register(this);
        }

        private void OnDisable()
        {
            // Deregister from the control scheme detector
            controlSchemeDetector.Deregister(this);
        }

        /// <summary>
        /// Transmute the input UI based on the current control scheme
        /// </summary>
        public void Transmute(string currentControlScheme)
        {
            switch(currentControlScheme)
            {
                case "Keyboard and Mouse":
                    SwitchCanvasGroup(keyboardAndMouseGroup);
                    break;
                case "Xbox Controller":
                    SwitchCanvasGroup(xboxControllerGroup);
                    break;
                case "PlayStation":
                    SwitchCanvasGroup(playstationGroup);
                    break;
            }
        }

        /// <summary>
        /// Switch the Canvas Group to display
        /// </summary>
        private void SwitchCanvasGroup(CanvasGroup activeGroup)
        {
            // Iterate through each Canvas Group
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                // Set the alpha of the Canvas Group
                canvasGroup.alpha = canvasGroup == activeGroup ? 1f : 0f;
            }
        }
    }
}
