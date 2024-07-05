using GoodLuckValley.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX
{
    public class TreeRustleTrigger : MonoBehaviour
    {
        // References
        private AreaCollider areaCollider;
        [SerializeField] private VisualEffect treeRustleVFX;

        private void Awake()
        {
            areaCollider = GetComponent<AreaCollider>();
        }

        private void OnEnable()
        {
            areaCollider.OnTriggerEnter += PlayEffect;
        }

        private void OnDisable()
        {
            areaCollider.OnTriggerEnter -= PlayEffect;
        }

        private void PlayEffect(GameObject gameObject)
        {
            treeRustleVFX.Play();
        }
    }
}