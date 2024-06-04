using GoodLuckValley.Patterns.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies
{
    public class FireflyController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform currentTarget;
        [SerializeField] private Transform followTarget;
        [SerializeField] private Transform retreatTarget;

        [Header("Fields - Movement")]
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float xVelSmoothing;

        [Header("Fields - Checks")]
        [SerializeField] private bool isRetreating;

        private StateMachine stateMachine;

        private void Awake()
        {
            // Get components
            animator = GetComponent<Animator>();

            // Declare states
            stateMachine = new StateMachine();
            IdleState idleState = new IdleState(this, animator);
            FollowState followState = new FollowState(this, animator);
            RetreatState retreatState = new RetreatState(this, animator);

            // Define strict transitions
            At(idleState, followState, new FuncPredicate(() => currentTarget != null));
            At(followState, retreatState, new FuncPredicate(() => isRetreating));
            At(retreatState, idleState, new FuncPredicate(() => currentTarget == null && !isRetreating));

            stateMachine.SetState(idleState);
        }

        /// <summary>
        /// Add a transition from one State to another given a certain condition
        /// </summary>
        /// <param name="from">The State to define the transition from</param>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    }
}