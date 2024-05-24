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

        public void Bind(MushroomSaveData data)
        {
            this.data = data;
            this.data.ID = ID;

            unlockedThrow = data.unlockedThrow;
            unlockedWallJump = data.unlockedWallJump;
        }

        public void OnRequestThrowUnlock(Component sender, object data)
        {
            // Set data for correct sender types
            if (sender is MushroomThrow) ((MushroomThrow)sender).SetThrowUnlocked(unlockedThrow);
        }

        public void OnRequestWallJumpUnlock(Component sender, object data)
        {
            if (sender is MushroomWallJump) ((MushroomWallJump)sender).SetWallJumpUnlocked(unlockedWallJump);
        }
    }
}