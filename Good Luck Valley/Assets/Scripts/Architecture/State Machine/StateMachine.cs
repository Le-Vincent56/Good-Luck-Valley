using System;
using System.Collections.Generic;

namespace GoodLuckValley.Architecture.StateMachine
{
    public class StateMachine
    {
        StateNode current;
        StateNode previous;
        Dictionary<Type, StateNode> nodes = new Dictionary<Type, StateNode>();
        HashSet<ITransition> anyTransitions = new HashSet<ITransition>();

        public void Update()
        {
            // Try to find any transitions
            ITransition transition = GetTransition();
            if (transition != null) ChangeState(transition.To);

            // If a State exists, update it
            current.State?.Update();
        }

        public void FixedUpdate()
        {
            // If a State exists, run it's FixedUpdate
            current.State?.FixedUpdate();
        }

        /// <summary>
        /// Set the current State of the StateMachine
        /// </summary>
        /// <param name="state">The State to set</param>
        public void SetState(IState state)
        {
            // Change the State
            current = nodes[state.GetType()];

            // Enter the State, if it exists
            current.State?.OnEnter();
        }

        /// <summary>
        /// Get the current State of the StateMachine
        /// </summary>
        public IState GetState() => current.State;

        /// <summary>
        /// Get the previous State of the StateMachine
        /// </summary>
        public IState GetPreviousState() => previous.State;

        /// <summary>
        /// Change States within the StateMachine
        /// </summary>
        /// <param name="state">The State to change to</param>
        private void ChangeState(IState state)
        {
            // Return if changing to the same State
            if (state == current.State) return;

            // Store the current State and get the next State
            IState previousState = current.State;
            IState nextState = nodes[state.GetType()].State;

            // Set the previous State
            previous = nodes[previousState.GetType()];

            // Exit the previous State
            previousState?.OnExit();

            // Enter the next State
            nextState?.OnEnter();

            // Set the current state
            current = nodes[state.GetType()];
        }

        /// <summary>
        /// Attempt to get a Transition
        /// </summary>
        /// <returns>The first Evaluated Transition</returns>
        private ITransition GetTransition()
        {
            // First, Loop through each Transition
            foreach (ITransition transition in anyTransitions)
            {
                // If any of the conditions of the Transition evaluate
                // to true, return it
                if (transition.Condition.Evaluate())
                    return transition;
            }

            // Loop through the Transitions of the current State
            foreach (ITransition transition in current.Transitions)
            {
                // If any of the conditions of the Transition evaluate
                // to true, return it
                if (transition.Condition.Evaluate())
                    return transition;
            }

            // If no Transition were found, return null
            return null;
        }
        /// <summary>
        /// Add a Transition from a StateNode to another StateNode given a certain condition
        /// </summary>
        /// <param name="from">The State to add a transition from</param>
        /// <param name="to">The State to transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        /// <summary>
        /// Add a transition from one State to another given a certain condition
        /// </summary>
        /// <param name="from">The State to define the transition from</param>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        public void At(IState from, IState to, IPredicate condition) => AddTransition(from, to, condition);

        /// <summary>
        /// Add a transition from any State to another one given a certain condition
        /// </summary>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the transition</param>
        public void Any(IState to, IPredicate condition) => AddAnyTransition(to, condition);

        /// <summary>
        /// Add a Transition to the StateMachine's Transitions List
        /// </summary>
        /// <param name="to">The State to transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }

        /// <summary>
        /// Get a StateNode within the StateMachine's dictionary.
        /// If it doesn't exist, add the StateNode to the StateMachine's dictionary
        /// </summary>
        /// <param name="state">The State to extract a StateNode from</param>
        /// <returns>The StateNode that was retrieved/added</returns>
        private StateNode GetOrAddNode(IState state)
        {
            // Attempt to extract the StateNode from the dictionary
            StateNode node = nodes.GetValueOrDefault(state.GetType());

            // Check if the StateNode exists in the dictionary
            if (node == null)
            {
                // If not, then add the StateNode to the dictionary
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            // Return the StateNode
            return node;
        }

        class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            /// <summary>
            /// Add a Transition to the StateNode
            /// </summary>
            /// <param name="to">The State to transition to</param>
            /// <param name="condition">The condition of the Transition</param>
            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }
}