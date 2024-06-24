using GoodLuckValley.Patterns.Blackboard;
using GoodLuckValley.Patterns.ServiceLocator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.Tutorial
{
    public class TutorialChecker : MonoBehaviour, IExpert
    {
        Blackboard blackboard;

        // Start is called before the first frame update
        void Start()
        {
            // Get the blackboard and register this as an expert
            blackboard = ServiceLocator.For(this).Get<BlackboardController>().GetBlackboard();
            ServiceLocator.For(this).Get<BlackboardController>().RegisterExpert(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public int GetInsistence(Blackboard blackboard)
        {
            return 100;
        }

        public void Execute(Blackboard blackboard)
        {
            blackboard.AddAction(() =>
            {

            }
            );
        }
    }
}