using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Start
{
    public class StartMenuController : MonoBehaviour
    {
        [SerializeField] private TitleScreenController controller;

        private void Awake()
        {
            controller = GetComponentInParent<TitleScreenController>();
        }

        /// <summary>
        /// Return to the Main Menu
        /// </summary>
        public void ReturnToMain() => controller.SetState(controller.MAIN);
    }
}