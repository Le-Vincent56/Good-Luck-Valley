using UnityEngine;
using AK.Wwise;
using GoodLuckValley.Audio.Ambience;

namespace GoodLuckValley.World.AreaTriggers
{
    [RequireComponent(typeof(AreaCollider))]
    public class BiomeVolume : MonoBehaviour
    {
        AreaCollider areaCollider;

        [Header("Fields")]
        [SerializeField] private Switch biomeSwitch;
        [SerializeField] private bool startAmbience;

        private void Awake()
        {
            areaCollider = GetComponent<AreaCollider>();
        }

        private void OnEnable()
        {
            areaCollider.OnTriggerEnter += ChangeAmbience;
        }

        private void OnDisable()
        {
            areaCollider.OnTriggerEnter -= ChangeAmbience;
        }

        private void ChangeAmbience(GameObject other)
        {
            biomeSwitch.SetValue(AmbienceManager.Instance.gameObject);

            if (startAmbience)
                AmbienceManager.Instance.StartAmbience();
        }
    }
}