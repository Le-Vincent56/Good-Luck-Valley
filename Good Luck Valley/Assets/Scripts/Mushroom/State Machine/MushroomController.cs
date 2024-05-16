using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachine
{
    public class MushroomController : MonoBehaviour
    {
        #region REFERENCES
        #endregion

        #region FIELDS
        [SerializeField] string previousState;
        [SerializeField] string currentState;

        private bool isBouncing = false;
        #endregion

        #region PROPERTIES
        public Animator Anim { get; private set; }
        public MushroomStateMachine StateMachine { get; private set; }
        public RMIdleState RMIdleState { get; private set; }
        public RMBounceState RMBounceState { get; private set; }
        #endregion

        private void Awake()
        {
            StateMachine = new MushroomStateMachine();

            RMIdleState = new RMIdleState(this, StateMachine, "idle");
            RMBounceState = new RMBounceState(this, StateMachine, "bounce");
        }

        // Start is called before the first frame update
        void Start()
        {
            Anim = GetComponent<Animator>();

            StateMachine.Initialize(RMIdleState); // Eventually replace with grow
        }

        // Update is called once per frame
        void Update()
        {
            // Update the logic of the State Machine
            StateMachine.CurrentState.LogicUpdate();

            // Check and set states
            if (StateMachine.PreviousState != null)
                previousState = StateMachine.PreviousState.ToString().Substring(48);
            currentState = StateMachine.CurrentState.ToString().Substring(48);
        }

        private void FixedUpdate()
        {
            // Update the physics of the State Machine
            StateMachine.CurrentState.PhysicsUpdate();
        }

        #region CHECK FUNCTIONS
        /// <summary>
        /// Check if the Mushroom has applied a bounce
        /// </summary>
        /// <returns></returns>
        public bool CheckIfBouncing()
        {
            return isBouncing;
        }
        #endregion

        #region ANIMATION FUNCTIONS
        private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishedTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
        #endregion

        #region HELPER FUNCTIONS
        /// <summary>
        /// Set whether or not the Mushroom's bounce effects are triggered
        /// </summary>
        /// <param name="isBouncing"></param>
        public void SetIsBouncing(bool isBouncing)
        {
            this.isBouncing = isBouncing;
        }

        /// <summary>
        /// Handle what happens when the Player bounces on the Mushroom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OnBounce(Component sender, object data)
        {
            // Set isBouncing to true
            isBouncing = true;
        }
        #endregion
    }
}
