using GoodLuckValley.Entities;
using GoodLuckValley.Persistence;
using UnityEngine;

namespace GoodLuckValley.World.AreaTriggers
{
    [RequireComponent(typeof(AreaCollider))]
    public class SaveTrigger : MonoBehaviour
    {
        private AreaCollider areaCollider;

        private void Awake()
        {
            areaCollider = GetComponent<AreaCollider>();
        }

        private void OnEnable()
        {
            areaCollider.OnTriggerEnter += TriggerEnter;
        }

        private void OnDisable()
        {
            areaCollider.OnTriggerEnter -= TriggerEnter;
        }

        private void TriggerEnter(GameObject other)
        {
            SaveLoadSystem.Instance.SaveGame();
        }
    }
}
