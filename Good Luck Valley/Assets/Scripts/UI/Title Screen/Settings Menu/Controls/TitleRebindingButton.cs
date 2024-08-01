using GoodLuckValley.UI.Elements;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using GoodLuckValley.UI.Settings.Controls;

namespace GoodLuckValley.UI.TitleScreen.Settings.Controls
{
    [System.Serializable]
    public class InputBindingInfo
    {
        public InputActionAsset actionAsset;
        public string actionMapName;
        public string actionName;
        public int bindingIndex;
    }

    public class TitleRebindingButton : MonoBehaviour
    {
        public InputBindingInfo bindingInfo;
        [SerializeField] private Button targetButton;
        [SerializeField] private MultiElementSelector multiElementSelector;
        [SerializeField] private Animator animator;
        [SerializeField] private Image keyImage;
        [SerializeField] private bool validRebind = true;
        private Coroutine glowCoroutine;
        [SerializeField] private bool rebinded;

        /// <summary>
        /// Initialize the rebinding button
        /// </summary>
        /// <param name="controller"></param>
        public void Init(ControlsSettingController controller, InputActionAsset inputActionAsset, InputKeyDictionary keysDictionary)
        {
            // Get component
            targetButton = GetComponent<Button>();
            keyImage = GetComponent<Image>();
            animator = GetComponent<Animator>();
            multiElementSelector = GetComponent<MultiElementSelector>();

            validRebind = true;

            // Add a click listener
            targetButton.onClick.AddListener(() => controller.StartRebinding(bindingInfo.actionName, bindingInfo.bindingIndex, this));

            // Set image
            SetBindingImage(inputActionAsset, keysDictionary);

            // Check if the binding has been rebinded
            UpdateRebinded();
        }

        /// <summary>
        /// Set the binding image
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="keysDictionary"></param>
        private void SetBindingImage(InputActionAsset inputActionAsset, InputKeyDictionary keysDictionary)
        {
            // Get the input action
            InputAction action = inputActionAsset.FindAction(bindingInfo.actionName);

            // Set the default image
            SetImage(keysDictionary.GetKey(action.bindings[bindingInfo.bindingIndex].ToDisplayString()));
        }
        
        /// <summary>
        /// Check if the rebinding button is an overriden binding
        /// </summary>
        /// <returns></returns>
        public bool CheckIfRebinded()
        {
            // Get the targeted action
            InputAction action = bindingInfo.actionAsset.FindAction(bindingInfo.actionName);

            // Exit case - the action does not exist
            if (action == null) return false;

            // Get the current binding
            InputBinding currentBinding = action.bindings[bindingInfo.bindingIndex];

            // Check for duplicate composite bindings
            if (action.bindings[bindingInfo.bindingIndex].isPartOfComposite)
            {
                // Start the composite index at 1 (0 is the "head" or the describer)
                int compositeIndex = bindingInfo.bindingIndex;
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
            } else
            {
                return CheckForOverride(currentBinding.overridePath);
            }

            return false;
        }

        /// <summary>
        /// Check if a keybind's override path exists
        /// </summary>
        /// <param name="overridePath">a Keybind's override path</param>
        /// <returns>True if it exists, false if not</returns>
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
        /// Set whether or not the button has been rebinded or tried to been rebinded
        /// </summary>
        /// <param name="rebinded"></param>
        public void SetRebinded(bool rebinded) => this.rebinded = rebinded;

        /// <summary>
        /// Set the binding imagae
        /// </summary>
        /// <param name="sprite"></param>
        public void SetImage(Sprite sprite) => keyImage.sprite = sprite;

        /// <summary>
        /// Set whether or not the rebinding button has a valid rebind
        /// </summary>
        /// <param name="validRebind"></param>
        public void SetValidRebind(bool validRebind)
        {
            this.validRebind = validRebind;

            // Update colors
            float keyImageAlpha = keyImage.color.a;
            Color validationColor = (validRebind) ? new Color(1.0f, 1.0f, 1.0f, keyImageAlpha) : new Color(0.867f, 0.105f, 0.051f, keyImageAlpha);
            keyImage.color = validationColor;
            multiElementSelector.UpdateColors(validationColor);
        }

        /// <summary>
        /// Get whether or not the rebinding button has a valid rebind
        /// </summary>
        /// <returns></returns>
        public bool GetValidRebind() => validRebind;

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
            multiElementSelector.UpdateColors(keyImage.color);
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
            multiElementSelector.UpdateColors(validationColor);
        }

        /// <summary>
        /// Set the default values for the rebinding button
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="keysDictionary"></param>
        public void SetDefault(InputActionAsset inputActionAsset, InputKeyDictionary keysDictionary)
        {
            // Get the input action
            InputAction action = inputActionAsset.FindAction(bindingInfo.actionName);

            // Set the default image
            SetImage(keysDictionary.GetKey(action.bindings[bindingInfo.bindingIndex].ToDisplayString()));

            // Set valid rebind
            SetValidRebind(true);

            // Exit case - the button has not been rebinded yet
            if (!rebinded) return;

            // Nullify any ongoing glow coroutines
            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
                glowCoroutine = null;
            }

            // Start the glow coroutine
            glowCoroutine = StartCoroutine(GlowCoroutine(0.6f));

            // Set rebinded to false
            rebinded = false;
        }

        IEnumerator GlowCoroutine(float glowDuration)
        {
            float halfDuration = glowDuration / 2;
            Color originalColor = keyImage.color;

            // Fade to full opacity
            for (float t = 0; t < halfDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / halfDuration;
                Color color = keyImage.color;
                color.a = Mathf.Lerp(originalColor.a, 1f, normalizedTime);
                keyImage.color = color;
                yield return null;
            }

            // Ensure full opacity
            Color fullOpacityColor = keyImage.color;
            fullOpacityColor.a = 1f;
            keyImage.color = fullOpacityColor;

            // Fade back to original opacity
            for (float t = 0; t < halfDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / halfDuration;
                Color color = keyImage.color;
                color.a = Mathf.Lerp(1f, originalColor.a, normalizedTime);
                keyImage.color = color;
                yield return null;
            }

            // Ensure original opacity
            keyImage.color = originalColor;
        }
    }
}