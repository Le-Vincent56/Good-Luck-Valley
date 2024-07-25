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

        public void Init(ControlsSettingController controller)
        {
            // Get component
            targetButton = GetComponent<Button>();

            // Add a click listener
            targetButton.onClick.AddListener(() => controller.StartRebinding(bindingInfo.actionName, bindingInfo.bindingIndex));

            Debug.Log(targetButton);
        }
    }
}