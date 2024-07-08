using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Scenes.Data
{
    [CreateAssetMenu(fileName = "Level Position Data")]
    public class LevelPositionData : ScriptableObject
    {
        [SerializeField] private List<LevelDataContainer> levelDataList;
        public HashSet<LevelDataContainer> LevelPositions { get; private set; }

        private void OnEnable()
        {
            LevelPositions = new HashSet<LevelDataContainer>(levelDataList);
        }

        /// <summary>
        /// Get a LevelDataContainer from a level name
        /// </summary>
        /// <param name="levelName">The level name to derive a LevelDataContainer</param>
        /// <returns>A LevelDataContainer with a name equal to the given level name</returns>
        public LevelDataContainer GetLevelData(string levelName)
        {
            // Iterate through each level position container
            foreach(LevelDataContainer level in LevelPositions)
            {
                // If the level name is found, then return the LevelDataContainer
                if (level.LevelName == levelName) return level;
            }

            // If the iteration completed and nothing was returned, return null
            return null;
        }
    }

    [Serializable]
    public class LevelDataContainer : IEquatable<LevelDataContainer>
    {
        public string LevelName;
        public List<Vector2> Entrances;
        public List<Vector2> Exits;

        /// <summary>
        /// Compare the LevelDataContainer with another to check if they are equal
        /// </summary>
        /// <param name="other">The LevelDataContainer to compare to</param>
        /// <returns>True if the LevelDataContainers have the same name, false if not</returns>
        public bool Equals(LevelDataContainer other)
        {
            // If an empty LevelDataContainer is given, return false
            if (other == null) return false;

            // Return whether or not the names are equal
            return LevelName == other.LevelName;
        }

        /// <summary>
        /// Compare the LevelDataContainer with an object to check if they are equal
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if the LevelDataContainer equals the object, false if not</returns>
        public override bool Equals(object obj)
        {
            // Convert the object to a LevelDataContainer
            if(obj is LevelDataContainer other)
            {
                // Check whether or not this LevelDataContainer equals the other
                return Equals(other);
            }

            // If the other object cannot be converted/casted, return false
            return false;
        }

        /// <summary>
        /// Get the hash code for the LevelDataContainer
        /// </summary>
        /// <returns>The hashcode for the LevelDataContainer based on the level name</returns>
        public override int GetHashCode()
        {
            return LevelName != null ? LevelName.GetHashCode() : 0;
        }
    }
}