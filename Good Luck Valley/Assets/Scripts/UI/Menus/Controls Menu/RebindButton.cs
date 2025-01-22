using DG.Tweening;
using GoodLuckValley.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Controls
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
        [SerializeField] private MenuButton targetButton;
        [SerializeField] private Animator animator;
        [SerializeField] private Image keyImage;
        [SerializeField] private bool validRebind;
        [SerializeField] private bool rebinded;

        private float initialOpacity;
        private float glowDuration = 0.6f;
        private Sequence glowSequence;

        public bool Rebinded { get => rebinded; set => rebinded = value; }
        public bool ValidRebind { get => validRebind; set => validRebind = value; }

        public void Initialize(ControlsMenuController controller, InputActionAsset inputActionAsset, InputKeyDictionary keysDictionary)
        {
            // Get components
            targetButton = GetComponent<MenuButton>();
            keyImage = GetComponentInChildren<Image>();
            animator = GetComponentInChildren<Animator>();

            // Set variables
            validRebind = true;
            initialOpacity = keyImage.color.a;

            // Add a click listener
            targetButton.OnClick.AddListener(() => controller.StartRebinding(bindingInfo.ActionName, bindingInfo.BindingIndex, this));

            // Set image
            SetBindingImage(inputActionAsset, keysDictionary);

            // Check if the binding has been rebinded
            UpdateRebinded();
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
        /// Check if the binding has been updated to a new binding
        /// </summary>
        public void UpdateRebinded() => rebinded = CheckIfRebinded();

        /// <summary>
        /// Set the default values for the rebinding button
        /// </summary>
        public void SetDefault(InputActionAsset inputActionAsset, InputKeyDictionary keysDictionary)
        {
            // Get the input action
            InputAction action = inputActionAsset.FindAction(bindingInfo.ActionName);

            // Set the default image
            SetImage(keysDictionary.GetKey(action.bindings[bindingInfo.BindingIndex].ToDisplayString()));

            // Set valid rebind
            SetValidRebind(true);

            // Exit case - the button has not been rebinded yet
            if (!rebinded) return;

            // Glow the button
            Glow();

            // Set rebinded to false
            rebinded = false;
        }

        /// <summary>
        /// Set whether or not the rebinding button has a valid rebind
        /// </summary>
        public void SetValidRebind(bool validRebind)
        {
            // Set the valid rebind
            this.validRebind = validRebind;

            // Update colors
            float keyImageAlpha = keyImage.color.a;
            Color validationColor = (validRebind) ? new Color(1.0f, 1.0f, 1.0f, keyImageAlpha) : new Color(0.867f, 0.105f, 0.051f, keyImageAlpha);
            keyImage.color = validationColor;
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

        /// <summary>
        /// Enable the rebinding button's animator
        /// </summary>
        public void EnableAnimator()
        {
            // Enable the animator
            animator.enabled = true;

            // Update colors
            float keyImageAlpha = keyImage.color.a;
            keyImage.color = new Color(1.0f, 1.0f, 1.0f, keyImageAlpha);
        }

        /// <summary>
        /// Disable the rebinding button's animator
        /// </summary>
        public void DisableAnimator()
        {
            // Disable the animator
            animator.enabled = false;

            // Update colors
            float keyImageAlpha = keyImage.color.a;
            Color validationColor = (validRebind) ? new Color(1.0f, 1.0f, 1.0f, keyImageAlpha) : new Color(0.867f, 0.105f, 0.051f, keyImageAlpha);
            keyImage.color = validationColor;
        }

        /// <summary>
        /// Glow the Key Image
        /// </summary>
        private void Glow()
        {
            // Kill the Glow Sequence if it exists
            glowSequence?.Kill();

            // Create the Glow Sequence
            glowSequence = DOTween.Sequence();

            // Get half of the Glow duration
            float halfGlowDuration = glowDuration / 2f;

            // Append Tweens to the Sequence
            glowSequence.Append(keyImage.DOFade(1f, halfGlowDuration));
            glowSequence.Append(keyImage.DOFade(initialOpacity, halfGlowDuration));

            // Hook up completion action
            glowSequence.onComplete += () =>
            {
                // Set the final color
                keyImage.color = new Color(keyImage.color.r, keyImage.color.g, keyImage.color.b, initialOpacity);
            };

        }
    }
}
