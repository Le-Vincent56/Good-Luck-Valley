using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Patterns.ServiceLocator;
using System;

namespace GoodLuckValley.Patterns.Blackboard
{
    public class BlackboardController : MonoBehaviour
    {
        private Dictionary<string, int> blackboardIndices = new Dictionary<string, int>();
        [SerializeField] private BlackboardData[] blackboardDatas;
        [SerializeField] private Blackboard[] blackboards;

        private void Awake()
        {
            blackboardIndices = new Dictionary<string, int>()
            {
                {"Tutorial", 0 },
                {"Player", 1 },
            };

            // Register to the global service locator
            ServiceLocator.ServiceLocator.ForSceneOf(this).Register(this);

            blackboards = new Blackboard[blackboardDatas.Length];

            // Set blackboard values
            for(int i = 0; i < blackboardDatas.Length; i++)
            {
                blackboards[i] = new Blackboard();
                blackboardDatas[i].SetValuesOnBlackboard(blackboards[i]);
            }
        }

        private void Update()
        {
            // Iterate through each blackboard
            for(int i = 0; i < blackboards.Length; i++)
            {
                // Check if the blackboard is dirty
                if (!blackboards[i].Dirty) continue;

                // If it is, reflect the data back to the data object
                blackboards[i].ReflectData(blackboardDatas[i]);

                // Clean the blackboard
                blackboards[i].Dirty = false;
            }
        }

        /// <summary>
        /// Get a Blackboard from its name
        /// </summary>
        /// <returns>The Blackboard associated with the given name</returns>
        public Blackboard GetBlackboard(string blackboardName)
        {
            // Try to get an index from a string name
            if(blackboardIndices.TryGetValue(blackboardName, out int index))
            {
                // Return the blackboard at the given index
                return blackboards[index];
            }

            Debug.Log("BlackboardController.GetBlackboard: Blackboard name does not exist/not implemented");
            return null;
        }
    }
}