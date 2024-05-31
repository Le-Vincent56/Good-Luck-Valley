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
        [SerializeField] private GameEvent onCancelThrow;

        #region REFERENCES
        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private MushroomSaveData data;
        #endregion

        #region FIELDS
        private bool paused = false;
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
            input.Throw += OnThrow;
            input.QuickBounce += OnQuickBounce;
            input.RecallLast += OnRecallLast;
            input.RecallAll += OnRecallAll;
        }

        private void OnDisable()
        {
            input.Throw -= OnThrow;
            input.QuickBounce -= OnQuickBounce;
            input.RecallLast -= OnRecallLast;
            input.RecallAll -= OnRecallAll;
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

        /// <summary>
        /// Handle Throw input
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        /// <param name="canceled">If the button has been lifted</param>
        private void OnThrow(bool started, bool canceled)
        {
            // Return if paused
            if (paused) return;

            // Create data
            ContextData contextData = new ContextData(started, canceled);

            // Raise the throw event
            // Calls to:
            //  - MushroomThrow.OnThrow();
            onThrow.Raise(this, contextData);
        }

        /// <summary>
        /// Handle Quick Bounce input
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        private void OnQuickBounce(bool started)
        {
            // Only handle if pressed once and the game is not paused
            if (started && !paused)
            {
                // Quick bounce
                // Calls to:
                //  - MushroomQuickBounce.QuickBounce();
                onQuickBounce.Raise(this, null);
            }
        }

        /// <summary>
        /// Handle Recall Last input
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        private void OnRecallLast(bool started)
        {
            // Only handle if pressed once and the game is not paused
            if (started && !paused)
            {
                // Recall last shroom
                // Calls to:
                //  - MushroomTracker.RecallLast();
                onRecallLast.Raise(this, null);
            }
        }

        /// <summary>
        /// Handle Recall All input
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        private void OnRecallAll(bool started)
        {
            // Only handle if pressed once and the game is not paused
            if (started && !paused)
            {
                // Recall all shrooms
                // Calls to:
                //  - MushroomTracker.RecallAll();
                onRecallAll.Raise(this, null);
            }
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

        public void OnUnlockPowers(Component sender, object data)
        {
            // Verify that the correct data is being sent
            if (data is not bool) return;

            bool unlocked = (bool)data;

            // Set the powers
            unlockedThrow = unlocked;
            unlockedWallJump = unlocked;
        }

        /// <summary>
        /// Set paused for the Power Controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void SetPaused(Component sender, object data)
        {
            // Verify the correct data is being sent
            if (data is not bool) return;

            // Cast and set data
            bool paused = (bool)data;
            this.paused = paused;

            // Cancel throw
            onCancelThrow.Raise(this, null);
        }
    }
}