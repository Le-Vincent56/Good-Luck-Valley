using GoodLuckValley.Player.Control;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.DevTools
{
    public class DevToolsUI : FadePanel
    {
        [SerializeField] private Text displayText;
        public bool Enabled { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            HideUIHard();
        }

        public void ToggleDevTools(Component sender, object data)
        {
            if (data is not bool) return;

            bool enabled = (bool)data;
            Enabled = enabled;

            if(Enabled)
            {
                ShowUI();
            } else
            {
                HideUI();
            }
        }

        public void UpdateUI(Component sender, object data)
        {
            if (data is not Player.Control.DevTools.Data) return;

            Player.Control.DevTools.Data devToolsData = (Player.Control.DevTools.Data)data;

            string display = "Dev Tools Enabled";

            if (devToolsData.NoClip)
                display += "\n NoClip On";

            if (devToolsData.PowersUnlocked)
                display += "\nMushroom Powers Unlocked";

            displayText.text = display;
        }
    }
}