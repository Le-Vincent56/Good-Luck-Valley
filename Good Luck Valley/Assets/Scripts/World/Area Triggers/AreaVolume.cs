using UnityEngine;
using AK.Wwise;
using System.Collections.Generic;

namespace GoodLuckValley.World.AreaTriggers
{
    [RequireComponent(typeof(AreaCollider))]
    public class AreaVolume : MonoBehaviour
    {
        public enum Type
        {
            Forest,
            Cave
        }

        AreaCollider areaCollider;

        [Header("Fields")]
        [SerializeField] Type areaType;
        Dictionary<Type, string> switchHash;

        private void Awake()
        {
            areaCollider = GetComponent<AreaCollider>();

            switchHash = new Dictionary<Type, string>()
            {
                { Type.Forest, "Forest" },
                { Type.Cave, "Cave" }
            };
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
            // Set the switch based on the area type
            string switchValue;
            if(switchHash.TryGetValue(areaType, out switchValue))
            {
                AkSoundEngine.SetSwitch("Ambience_SG", switchValue, gameObject);
                Debug.Log("Switch set to: " + switchValue);
            } else
            {
                Debug.LogError("Switch value not found for area type: " + areaType);
            }
        }
    }
}