using System;
using System.Collections.Generic;

namespace GoodLuckValley.Patterns.Blackboard
{
    public class Arbiter
    {
        private readonly List<IExpert> experts = new List<IExpert>();

        /// <summary>
        /// Register an Expert to the List
        /// </summary>
        /// <param name="expert">The Expert to register</param>
        public void RegisterExpert(IExpert expert)
        {
            // Check if the expert is null or not
            Preconditions.CheckNotNull(expert);

            // Add the expert to the list
            experts.Add(expert);
        }

        public List<Action> BlackboardIteration(Blackboard blackboard)
        {
            IExpert bestExpert = null;
            int highestInsistence = 0;

            // Iterate through each Expert
            foreach(IExpert expert in experts)
            {
                // Get the insistence of the Expert
                int insistence = expert.GetInsistence(blackboard);

                // Compare the Expert's insistence to the highest
                // seen insistence
                if(insistence > highestInsistence)
                {
                    // If higher, set the Expert's insistence
                    // as the highest seen insistence
                    // and set the Expert as the best Expert
                    highestInsistence = insistence;
                    bestExpert = expert;
                }
            }

            // Get the best Expert to execute (add Actions to the Blackboard)
            bestExpert?.Execute(blackboard);

            // Get the list of passed actions
            List<Action> actions = blackboard.PassedActions;

            // Clear the actions on the Blackboard
            blackboard.ClearActions();

            // Return the actions for handling
            return actions;
        }
    }
}
