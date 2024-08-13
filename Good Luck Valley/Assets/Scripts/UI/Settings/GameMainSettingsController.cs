using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.Settings
{
    public class GameMainSettingsController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameSettingsMenu controller;
        private const int stateNum = 3;

        private void Awake()
        {
            controller = GetComponentInParent<GameSettingsMenu>();
        }

        public void EnterAudio() => controller.SetState(controller.AUDIO);
        public void EnterVideo() => controller.SetState(controller.VIDEO);
        public void EnterControls() => controller.SetState(controller.CONTROLS);

        public void BackInput(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not int) return;

            // Cast and compare data
            if ((int)data == stateNum)
            {
                ReturnToMain();
            }
        }

        public void ReturnToMain() => controller.CloseSettings();
    }
}