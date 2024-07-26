using GoodLuckValley.Player.Control;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GoodLuckValley.World.AreaTriggers
{
    [RequireComponent(typeof(AreaCollider))]
    public class PlayerInputTrigger : MonoBehaviour
    {
        private AreaCollider areaCollider;
        [SerializeField] private bool enableInput;

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
            PlayerController player = other.GetComponent<PlayerController>();
            player.SetPlayerInput(enableInput);
        }
    }
}