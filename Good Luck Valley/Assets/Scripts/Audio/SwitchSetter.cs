using UnityEngine;

namespace GoodLuckValley.Audio
{
    public class SwitchSetter : MonoBehaviour
    {
        [Header("Wwise Fields")]
        [SerializeField] private AK.Wwise.Switch switchToSet;

        /// <summary>
        /// Set the Switch
        /// </summary>
        public void Switch() => switchToSet.SetValue(gameObject);
    }
}
