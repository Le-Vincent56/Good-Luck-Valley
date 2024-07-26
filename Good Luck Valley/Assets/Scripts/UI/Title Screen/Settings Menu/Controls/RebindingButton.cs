using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    public class RebindingButton : MonoBehaviour
    {
        public InputBindingInfo bindingInfo;
        [SerializeField] private Button targetButton;
        [SerializeField] private Image keyImage;

        /// <summary>
        /// Initialize the rebinding button
        /// </summary>
        /// <param name="controller"></param>
        public void Init(ControlsSettingController controller)
        {
            // Get component
            targetButton = GetComponent<Button>();
            keyImage = GetComponent<Image>();

            // Add a click listener
            targetButton.onClick.AddListener(() => controller.StartRebinding(bindingInfo.actionName, bindingInfo.bindingIndex, this));
        }

        /// <summary>
        /// Set the binding imagae
        /// </summary>
        /// <param name="sprite"></param>
        public void SetImage(Sprite sprite) => keyImage.sprite = sprite;
    }
}