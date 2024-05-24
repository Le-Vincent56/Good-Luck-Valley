using GoodLuckValley.Persistence;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Interactables
{
    public class Collectible : MonoBehaviour, IInteractable, IBind<CollectibleSaveData>
    {
        #region REFERENCES
        [SerializeField] protected CollectibleSaveData data;
        #endregion

        #region FIELDS        
        [SerializeField] protected bool isCollected;
        #endregion

        #region PROPERTIES
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        #endregion

        private void Start()
        {
            // Check if the Collectible should be active
            CheckActive();
        }

        /// <summary>
        /// Bind the Collectible for persistence
        /// </summary>
        /// <param name="data"></param>
        public void Bind(CollectibleSaveData data)
        {
            this.data = data;
            this.data.ID = ID;

            // Set collected data
            isCollected = data.collected;
        }

        /// <summary>
        /// Interactable with the collectible
        /// </summary>
        public virtual void Interact()
        {
            isCollected = true;
            data.collected = true;

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
    }
}