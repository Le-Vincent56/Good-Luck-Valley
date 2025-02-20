using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Input;
using GoodLuckValley.Persistence;
using GoodLuckValley.UI.Menus.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.UI.Menus.Persistence
{
    public class ControlsSaveHandler : MonoBehaviour, IBind<ControlsData>
    {
        [SerializeField] private ControlsData data;
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private InputActionAsset inputActionAsset;
        private ControlsController controller;
        private SaveLoadSystem saveLoadSystem;

        [field: SerializeField] public SerializableGuid ID { get; set; } = new SerializableGuid(2453316011, 1181080316, 3984020625, 60208677);

        private void Awake()
        {
            // Get the controls menu controller
            controller = GetComponent<ControlsController>();

            // Get services
            saveLoadSystem = ServiceLocator.Global.Get<SaveLoadSystem>();
        }

        /// <summary>
        /// Save the Controls data
        /// </summary>
        public void SaveData()
        {
            // Save the bindings
            data.Bindings = inputActionAsset.SaveBindingOverridesAsJson();

            // Save the settings
            saveLoadSystem.SaveSettings();

            // Load the bindings
            inputReader.LoadBindings(data.Bindings);
        }

        /// <summary>
        /// Bind the Controls data
        /// </summary>
        public void Bind(ControlsData data)
        {
            // Bind the data
            this.data = data;
            this.data.ID = data.ID;

            // Load bindings
            inputActionAsset.LoadBindingOverridesFromJson(data.Bindings);
            inputReader.LoadBindings(data.Bindings);

            // Set the binding images
            controller.SetBindingImages();
        }

        /// <summary>
        /// Reset the Controls data
        /// </summary>
        public void ResetData()
        {
            // Iterate through each action map
            foreach (InputActionMap map in inputActionAsset.actionMaps)
            {
                // Remove all binding overrides
                map.RemoveAllBindingOverrides();
            }

            // Set the binding defaults for the Rebind Buttons
            controller.SetBindingDefaults();
        }
    }
}
