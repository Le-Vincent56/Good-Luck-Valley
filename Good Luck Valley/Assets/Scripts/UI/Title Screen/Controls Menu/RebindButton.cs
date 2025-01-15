using GoodLuckValley.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.Controls
{
    [System.Serializable]
    public struct InputBindingInfo
    {
        public InputActionAsset ActionAsset;
        public string ActionMapName;
        public string ActionName;
        public int BindingIndex;
    }

    public class RebindButton : MonoBehaviour
    {
        [SerializeField] private InputBindingInfo bindingInfo;
        [SerializeField] private Button targetButton;
        [SerializeField] private Animator animator;
        [SerializeField] private Image keyImage;
        [SerializeField] private bool validRebind;
        [SerializeField] private bool rebinded;

        private void Awake()
        {
            // Get components
            targetButton = GetComponent<Button>();
            keyImage = GetComponentInChildren<Image>();
            animator = GetComponentInChildren<Animator>();

            // Set variables
            validRebind = true;

            // Add event listeners

        }

        /// <summary>
        /// Check if the rebinding button is an overriden binding
        /// </summary>
        public bool CheckIfRebinded()
        {
            // Get the targeted action
            InputAction action = bindingInfo.ActionAsset.FindAction(bindingInfo.ActionName);

            // Exit case - the action does not exist
            if (action == null) return false;

            // Get the current binding
            InputBinding currentBinding = action.bindings[bindingInfo.BindingIndex];

            // Check for duplicate composite bindings
            if (action.bindings[bindingInfo.BindingIndex].isPartOfComposite)
            {
                // Start the composite index at 1 (0 is the "head" or the describer)
                int compositeIndex = bindingInfo.BindingIndex;
                while (compositeIndex > 0 && action.bindings[compositeIndex].isPartOfComposite)
                {
                    compositeIndex--;
                }

                // Loop through each control that's part of the same composite
                for (int i = compositeIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
                {
                    // If the action is the same as not the one being checked, continue
                    if (action.actionMap.bindings[i].name != currentBinding.name)
                        continue;

                    return CheckForOverride(action.actionMap.bindings[i].overridePath);
                }
            }
            else
            {
                return CheckForOverride(currentBinding.overridePath);
            }

            return false;
        }

        /// <summary>
        /// Check if a keybind's override path exists
        /// </summary>
        private bool CheckForOverride(string overridePath)
        {
            if (overridePath != null)
                return true;
            else return false;
        }

        /// <summary>
        /// Set the binding image
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="keysDictionary"></param>
        private void SetBindingImage(InputActionAsset inputActionAsset, InputKeyDictionary keysDictionary)
        {
            // Get the input action
            InputAction action = inputActionAsset.FindAction(bindingInfo.ActionName);

            // Set the default image
            SetImage(keysDictionary.GetKey(action.bindings[bindingInfo.BindingIndex].ToDisplayString()));
        }

        /// <summary>
        /// Set the binding imagae
        /// </summary>
        /// <param name="sprite"></param>
        public void SetImage(Sprite sprite) => keyImage.sprite = sprite;
    }
}
