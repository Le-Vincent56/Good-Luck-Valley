using GoodLuckValley.Events;
using GoodLuckValley.Persistence;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class PowerController : MonoBehaviour, IBind<MushroomSaveData>
    {
        #region REFERENCES
        [SerializeField] private MushroomSaveData data;
        #endregion

        #region FIELDS
        [SerializeField] private bool unlockedThrow;
        [SerializeField] private bool unlockedWallJump;
        #endregion

        #region PROPERTIES
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
        public bool UnlockedThrow { get { return unlockedThrow;} }
        public bool UnlockedWallJump { get {  return unlockedWallJump;} }
        #endregion

        /// <summary>
        /// Update Mushroom Power save data
        /// </summary>
        public void UpdateData()
        {
            data.unlockedThrow = unlockedThrow;
            data.unlockedWallJump = unlockedWallJump;
        }

        public void Bind(MushroomSaveData data)
        {
            // Initialize data
            this.data = data;
            this.data.ID = ID;

            // Set data
            unlockedThrow = data.unlockedThrow;
            unlockedWallJump = data.unlockedWallJump;
        }

        /// <summary>
        /// Request to see if the Mushroom Throw is unlocked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OnRequestThrowUnlock(Component sender, object data)
        {
            // Set data for correct sender types
            if (sender is MushroomThrow) ((MushroomThrow)sender).SetThrowUnlocked(unlockedThrow);
            if(sender is MushroomQuickBounce) ((MushroomQuickBounce)sender).SetThrowUnlocked(unlockedThrow);
        }

        /// <summary>
        /// Request to see if the Wall Jump is unlocked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OnRequestWallJumpUnlock(Component sender, object data)
        {
            if (sender is MushroomWallJump) ((MushroomWallJump)sender).SetWallJumpUnlocked(unlockedWallJump);
        }

        /// <summary>
        /// Unlock the Mushroom Throw ability
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void UnlockThrow(Component sender, object data)
        {
            // Unlock the mushroom throw
            unlockedThrow = true;

            // Update data
            UpdateData();
        }

        /// <summary>
        /// Unlock the Wall Jump ability
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void UnlockWallJump(Component sender, object data)
        {
            // Unlock the wall jump
            unlockedWallJump = true;

            // Update save data
            UpdateData();
        }

        public void SetUnlockMushroom(Component sender, object data)
        {
            if (data is not bool) return;

            bool unlock = (bool)data;

            unlockedThrow = unlock;
            unlockedWallJump = unlock;
        }
    }
}