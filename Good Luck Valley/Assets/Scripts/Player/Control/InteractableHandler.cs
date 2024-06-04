using GoodLuckValley.Entity;
using GoodLuckValley.Player.Input;
using GoodLuckValley.World.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.Handlers
{
    public class InteractableHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private DynamicCollisionHandler collisionHandler;

        [Header("Fields")]
        [SerializeField] private bool canInteract;
        private IInteractable interactable;

        private void Awake()
        {
            collisionHandler = GetComponent<DynamicCollisionHandler>();
        }

        private void OnEnable()
        {
            input.Interact += OnInteract;
        }

        private void OnDisable()
        {
            input.Interact -= OnInteract;
        }

        private void FixedUpdate()
        {
            interactable = collisionHandler.collisions.Interactable;

            canInteract = interactable != null;
        }

        public void OnInteract(bool started)
        {
            if(started)
            {
                interactable?.ExecuteCommand();
            }
        }
    }
}