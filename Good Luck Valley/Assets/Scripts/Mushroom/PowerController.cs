using GoodLuckValley.Events;
using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Input;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class PowerController : MonoBehaviour, IBind<MushroomSaveData>
    {
        [Header("Events")]
        [SerializeField] private GameEvent onThrow;
        [SerializeField] private GameEvent onQuickBounce;
        [SerializeField] private GameEvent onRecallLast;
        [SerializeField] private GameEvent onRecallAll;

        #region REFERENCES
        [Header("References")]
        [SerializeField] private InputReader input;
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

        private void OnEnable()
        {
            input.Throw += Throw;
        }

        private void OnDisable()
        {
            input.Throw -= Throw;
        }

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

        private void Throw(bool started, bool canceled)
        {
            ContextData contextData = new ContextData(started, canceled);

            // Raise the throw event
            // Calls to:
            //  - MushroomThrow.OnThrow();
            onThrow.Raise(this, contextData);
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
    }
}