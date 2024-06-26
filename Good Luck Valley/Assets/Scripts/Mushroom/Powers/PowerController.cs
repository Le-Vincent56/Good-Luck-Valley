using GoodLuckValley.Events;
using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Input;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class PowerController : MonoBehaviour
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
        #endregion

        #region PROPERTIES
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();
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