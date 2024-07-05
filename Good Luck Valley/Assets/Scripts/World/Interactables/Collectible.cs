using GoodLuckValley.Persistence;
using GoodLuckValley.Patterns.Commands;
using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Events;
using GoodLuckValley.Entities;

namespace GoodLuckValley.World.Interactables
{
    public class Collectible : MonoBehaviour, IInteractable, IBind<CollectibleSaveData>
    {
        [Header("Events")]
        [SerializeField] private GameEvent onSetInteractable;

        #region REFERENCES
        private AreaCollider areaCollider;
        [SerializeField] protected CollectibleSaveData data;
        #endregion

        #region FIELDS
        readonly Queue<ICommand<IInteractable>> commandQueue = new Queue<ICommand<IInteractable>>();
        [SerializeField] protected bool isCollected;
        #endregion

        #region PROPERTIES
        private bool AlwaysApply { get; set; } = true;
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        #endregion

        private void Awake()
        {
            areaCollider = GetComponent<AreaCollider>();
    }

        private void OnEnable()
        {
            areaCollider.OnTriggerEnter += EnterArea;
            areaCollider.OnTriggerExit += ExitArea;
        }

        private void OnDisable()
        {
            areaCollider.OnTriggerEnter -= EnterArea;
            areaCollider.OnTriggerExit -= ExitArea;
        }

        protected virtual void Start()
        {
            // Check if the Collectible should be active
            CheckActive();

            // Create and enqueue the interactable command
            InteractableCommand newCommand = new InteractableCommand.Builder(new List<IInteractable> { this })
                .WithAction(_ => Interact())
                .Build();
            QueueCommand(newCommand);
        }

        public void EnterArea(GameObject gameObject) => onSetInteractable.Raise(this, this);
        public void ExitArea(GameObject gameObject) => onSetInteractable.Raise(this, null);

        /// <summary>
        /// Queue a Command for the Collectible
        /// </summary>
        /// <param name="command"></param>
        public void QueueCommand(ICommand<IInteractable> command) => commandQueue.Enqueue(command);
        
        /// <summary>
        /// Execute the command for the interactable
        /// </summary>
        public void ExecuteCommand()
        {
            if(commandQueue.Count > 0)
            {
                commandQueue.Dequeue()?.Execute();
            }
        }

        /// <summary>
        /// Interactable with the collectible
        /// </summary>
        public virtual void Interact()
        {
            isCollected = true;
            UpdateSaveData();

            CheckActive();
        }

        /// <summary>
        /// Check if the Collectible should be active
        /// </summary>
        public void CheckActive()
        {
            if (isCollected) gameObject.SetActive(false);
            else gameObject.SetActive(true);
        }

        /// <summary>
        /// Update Collectible save data
        /// </summary>
        public void UpdateSaveData()
        {
            data.collected = isCollected;
        }

        /// <summary>
        /// Bind the Collectible for persistence
        /// </summary>
        /// <param name="data"></param>
        public void Bind(CollectibleSaveData data, bool applyData = true)
        {
            this.data = data;
            this.data.ID = ID;

            isCollected = data.collected;
        }
    }
}