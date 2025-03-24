using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;

namespace GoodLuckValley.Utilities.PlayerLoop
{
    public static class PlayerLoopUtils
    {
        /// <summary>
        /// Insert a system into the player loop
        /// </summary>
        public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
        {
            // Exit case - the loop type does not match
            if (loop.type != typeof(T)) return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);

            // Create a list to hold all the subsystems of type T
            List<PlayerLoopSystem> playerLoopSystemList = new();

            // If there are subsystems, add them to the list
            if (loop.subSystemList != null) playerLoopSystemList.AddRange(loop.subSystemList);

            // Insert the given system at the provided index
            playerLoopSystemList.Insert(index, systemToInsert);

            // Replace the old subsystem with the new one
            loop.subSystemList = playerLoopSystemList.ToArray();

            // Notify that the function was successful
            return true;
        }

        /// <summary>
        /// Recursively check the type of each subsystem
        /// </summary>
        private static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
        {
            // Exit case - if there's no more subsystems to recurse
            if (loop.subSystemList == null) return false;

            // Loop through each subsystem that belongs to this node
            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                // If the subsystem type is not correct, continue
                if (!InsertSystem<T>(ref loop.subSystemList[i], in systemToInsert, index)) continue;

                // Otherwise, return true
                return true;
            }

            // If reached the end, there is no type, so return false
            return false;
        }

        /// <summary>
        /// Remove a system from the player loop
        /// </summary>
        public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
        {
            // Exit case - if there are no subsystems for this node
            if (loop.subSystemList == null) return;

            // Create a list and copy the current subsystems over
            List<PlayerLoopSystem> playerLoopSystemList = new(loop.subSystemList);

            // Iterate over each system
            for (int i = 0; i < playerLoopSystemList.Count; i++)
            {
                // Check if the current subsystem is the system to remove
                if (playerLoopSystemList[i].type == systemToRemove.type
                    && playerLoopSystemList[i].updateDelegate == systemToRemove.updateDelegate)
                {
                    // If so, remove it from the list
                    playerLoopSystemList.RemoveAt(i);

                    // Set the subsystem list equal to the copy
                    loop.subSystemList = playerLoopSystemList.ToArray();
                }
            }

            // Handle removal in other subsystems
            HandleSubSystemLoopForRemoval<T>(ref loop, systemToRemove);
        }

        /// <summary>
        /// Handle removing a whole subsystem
        /// </summary>
        private static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop, PlayerLoopSystem systemToRemove)
        {
            // Exit case - if there are no subsystems for this node
            if (loop.subSystemList == null) return;

            // Recursively remove the system from each subsystem
            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                RemoveSystem<T>(ref loop.subSystemList[i], systemToRemove);
            }
        }

        /// <summary>
        /// Print the player loop
        /// </summary>
        public static void PrintPlayerLoop(PlayerLoopSystem loop)
        {
            // Create a new string builder
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Unity Player Loop");

            // Iterate through each subsystem
            foreach (PlayerLoopSystem subSystem in loop.subSystemList)
            {
                // Start the recursive print
                PrintSubsystem(subSystem, sb, 0);
            }

            // Print the string
            Debug.Log(sb.ToString());
        }

        /// <summary>
        /// Print subsystems recursively
        /// </summary>
        private static void PrintSubsystem(PlayerLoopSystem system, StringBuilder sb, int level)
        {
            // Append the system at the current level of recursion
            sb.Append(' ', level * 2).AppendLine(system.type.ToString());

            // Exit case - if we have reached a leaf (nowhere left to go)
            if (system.subSystemList == null || system.subSystemList.Length == 0) return;

            // Iterate through each subsystem
            foreach (PlayerLoopSystem subSystem in system.subSystemList)
            {
                // Recursively print
                PrintSubsystem(subSystem, sb, level + 1);
            }
        }
    }
}