using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] protected TitleScreenController controller;

        protected virtual void Awake()
        {
            controller = GetComponentInParent<TitleScreenController>();
        }
    }
}