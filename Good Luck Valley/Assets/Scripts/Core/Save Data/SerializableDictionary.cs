using System.Collections.Generic;
using UnityEngine;

namespace HiveMind.SaveData
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        #region FIELDS
        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();
        #endregion

        /// <summary>
        /// Prepare the dictionary for serializing
        /// </summary>
        public void OnBeforeSerialize()
        {
            // Clear the keys and values
            keys.Clear();
            values.Clear();

            // Add the keys and values
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        /// <summary>
        /// Clean up the dictionary after serializing
        /// </summary>
        public void OnAfterDeserialize()
        {
            // Clear the SerializableDictionary
            Clear();

            // Check if there are any mismatches between keys and values
            if (keys.Count != values.Count)
            {
                Debug.LogError("Tried to deserialize a SerializableDictionary, but the amount of keys (" + keys.Count + ") do not equal the amount of values (" + values.Count + ")");
            }
            else
            {
                // If there are not, then add the keys and values
                for (int i = 0; i < keys.Count; i++)
                {
                    Add(keys[i], values[i]);
                }
            }
        }
    }
}
