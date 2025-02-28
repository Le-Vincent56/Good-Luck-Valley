using GoodLuckValley.UI.Tutorial;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley
{
    public class TutorialKeybindMaster : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TutorialControlsAnimationDictionary animationDictionary;
        [SerializeField] private TutorialControlSpriteDictionary spriteDictionary;
        [SerializeField] private PlayerInput playerInput;
        private string currentControlScheme;



        [SerializeField] private List<TutorialKeybind> tutorialKeybinds = new List<TutorialKeybind>();

        private void Awake()
        {
            // Get the tutorial keybinds
            tutorialKeybinds = new List<TutorialKeybind>();
            GetComponentsInChildren<TutorialKeybind>();

            // Set the tutorial keybinds
            SetTutorialKeybinds();
        }

        private void Update()
        {
            // Exit case - the control scheme has not changed
            if (currentControlScheme == playerInput.currentControlScheme) return;

            // Set the current control scheme
            currentControlScheme = playerInput.currentControlScheme;

            // Set the tutorial keybinds
            SetTutorialKeybinds();
        }

        /// <summary>
        /// Set the tutorial keybinds
        /// </summary>
        private void SetTutorialKeybinds()
        {
            // Iterate through each Tutorial Keybind
            foreach(TutorialKeybind keybind in tutorialKeybinds)
            {
                // Get the action name
                string actionName = keybind.BindingInfo.ActionName;
                
                // Set the sprite and animator
                keybind.SetSprite(spriteDictionary.GetSprite(actionName));
                keybind.SetAnimatorController(animationDictionary.GetController(actionName));
            }
        }
    }
}
