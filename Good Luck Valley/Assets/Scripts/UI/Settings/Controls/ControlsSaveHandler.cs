using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.UI.Settings.Controls
{
    public class ControlsSaveHandler : MonoBehaviour, IBind<ControlsData>
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private ControlsData data;

        [field: SerializeField] public SerializableGuid ID { get; set; } = new SerializableGuid(2453316011, 1181080316, 3984020625, 60208677);

        public void SaveData()
        {
            data.bindings = inputActionAsset.SaveBindingOverridesAsJson();
            SaveLoadSystem.Instance.SaveSettings();

            inputReader.LoadBindings(data.bindings);
        }

        public void Bind(ControlsData data, bool applyData = true)
        {
            this.data = data;
            this.data.ID = data.ID;

            inputActionAsset.LoadBindingOverridesFromJson(data.bindings);
            inputReader.LoadBindings(data.bindings);
        }
    }
}