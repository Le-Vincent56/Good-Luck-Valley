using UnityEngine;

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
            Debug.Log("Change Ambience: " + areaType);
        }
    }
}