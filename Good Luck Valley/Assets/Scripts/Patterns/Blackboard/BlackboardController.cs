using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Patterns.ServiceLocator;
using System;

namespace GoodLuckValley.Patterns.Blackboard
{
    public class BlackboardController : MonoBehaviour
    {
        [SerializeField] BlackboardData blackboardData;
        readonly Blackboard blackboard = new Blackboard();
        readonly Arbiter arbiter = new Arbiter();

        private void Awake()
        {
            // Register to the global service locator
            ServiceLocator.ServiceLocator.Global.Register(this);

            // Set blackboard values
            blackboardData.SetValuesOnBlackboard(blackboard);

            // Debug the blackboard
            blackboard.Debug();
        }

        /// <summary>
        /// Get the Blackboard
        /// </summary>
        /// <returns>The Blackboard used by the Arbiter</returns>
        public Blackboard GetBlackboard() => blackboard;

        /// <summary>
        /// Register an Expert to the Arbiter
        /// </summary>
        /// <param name="expert">The Expert to register</param>
        public void RegisterExpert(IExpert expert) => arbiter.RegisterExpert(expert);


        // TODO Add deregister expert method

        private void Update()
        {
            // Execute all agreed actions from the current iteration
            foreach(Action action in arbiter.BlackboardIteration(blackboard))
            {
                // Execute the action
                action();
            }
        }
    }
}