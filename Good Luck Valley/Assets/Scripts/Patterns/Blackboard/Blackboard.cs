using GoodLuckValley.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GoodLuckValley.Patterns.Blackboard
{
    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        readonly string name;
        readonly int hashedKey;

        public BlackboardKey(string name)
        {
            this.name = name;
            hashedKey = name.ComputeFNV1aHash();
        }

        public bool Equals(BlackboardKey other) => hashedKey == other.hashedKey;
        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => hashedKey;
        public override string ToString() => name;

        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs.hashedKey == rhs.hashedKey;
        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => !(lhs == rhs);
    }

    [Serializable]
    public class BlackboardEntry<T>
    {
        public BlackboardKey Key { get; }
        public T Value { get; }
        public Type ValueType { get; }

        public BlackboardEntry(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }

        public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;
        public override int GetHashCode() => Key.GetHashCode();
    }

    [Serializable]
    public class Blackboard
    {
        Dictionary<string, BlackboardKey> keyRegistry = new Dictionary<string, BlackboardKey>();
        Dictionary<BlackboardKey, object> entries = new Dictionary<BlackboardKey, object>();

        public bool Dirty { get; set; }

        /// <summary>
        /// Attempt to get a value out of the Blackboard
        /// </summary>
        /// <typeparam name="T">The type of the value trying to be retrieved</typeparam>
        /// <param name="key">The key within the Blackboard</param>
        /// <param name="value">The value associated with the given key</param>
        /// <returns>True if the value was retrieved successfully, false if otherwise</returns>
        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            // Attempt to retrieve the value associated with the given key,
            // then check if the value is of the correct type before casting it
            if(entries.TryGetValue(key, out object entry) && entry is BlackboardEntry<T> castedEntry)
            {
                // Return the casted value and that the value was successfully retrieved
                value = castedEntry.Value;
                return true;
            }

            // Set the value to default and return false
            value = default;
            return false;
        }

        /// <summary>
        /// Set a value within the Blackboard
        /// </summary>
        /// <typeparam name="T">The type of the value being set</typeparam>
        /// <param name="key">The key within the Blackboard</param>
        /// <param name="value">The new value</param>
        public void SetValue<T>(BlackboardKey key, T value, bool fromData = false)
        {
            entries[key] = new BlackboardEntry<T>(key, value);

            // If setting values from BlackboardData, return
            if (fromData) return;

            // Otherwise, notify dirty
            Dirty = true;
        }

        /// <summary>
        /// Get a key from the Blackboard or register a new one if one is not already found
        /// </summary>
        /// <param name="keyName">The name of the key</param>
        /// <returns>The BlackboardKey that was either retrieved or created</returns>
        public BlackboardKey GetOrRegisterKey(string keyName)
        {
            // Check if the key is null or if it is a blank key
            Preconditions.CheckNotNull(keyName);

            // Try to get the value out of the key registry
            if(!keyRegistry.TryGetValue(keyName, out BlackboardKey key))
            {
                // If there's no key, create a new one and assign
                // the key and value
                key = new BlackboardKey(keyName);
                keyRegistry[keyName] = key;
            }

            // Return the key
            return key;
        }

        /// <summary>
        /// Check if the Blackboard contains a key
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>True if the Blackboard contains they key, false otherwise</returns>
        public bool ContainsKey(BlackboardKey key) => entries.ContainsKey(key);

        /// <summary>
        /// Remove a key from the Blackboard
        /// </summary>
        /// <param name="key">The key to remove</param>
        public void Remove(BlackboardKey key) => entries.Remove(key);

        /// <summary>
        /// Reflect data back into the ScriptableObject
        /// </summary>
        /// <param name="data"></param>
        public void ReflectData(BlackboardData data)
        {
            foreach(KeyValuePair<BlackboardKey, object> entry in entries)
            {
                // Get the key and entry value
                BlackboardKey key = entry.Key;
                object entryValue = entry.Value;

                // Find the corresponding BlackboardEntryData in the BlackboardData
                BlackboardEntryData blackboardEntryData = data.entries.Find(e => e.keyName == key.ToString());

                // Check if a BlackboardEntryData has been retrieved
                if(blackboardEntryData != null)
                {
                    // Update the value based on the entry type
                    Type entryType = entryValue.GetType();

                    // Check if the type is equal to the generic type of the type of the Blackboard Entry
                    if (entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>))
                    {
                        // Get the property of the value
                        PropertyInfo valueProperty = entryType.GetProperty("Value");

                        // If it is null, continue
                        if (valueProperty == null) continue;

                        // Get the value of the entry
                        object value = valueProperty.GetValue(entryValue);

                        switch (blackboardEntryData.valueType)
                        {
                            case AnyValue.ValueType.Int:
                                blackboardEntryData.value.intValue = (int)value;
                                break;

                            case AnyValue.ValueType.Float:
                                blackboardEntryData.value.floatValue = (float)value;
                                break;

                            case AnyValue.ValueType.Bool:
                                blackboardEntryData.value.boolValue = (bool)value;
                                break;

                            case AnyValue.ValueType.String:
                                blackboardEntryData.value.stringValue = (string)value;
                                break;

                            case AnyValue.ValueType.Vector2:
                                blackboardEntryData.value.vector2Value = (Vector2)value;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Debug the Blackboard
        /// </summary>
        public void Debug()
        {
            // Loop through each blackboard entry
            foreach(KeyValuePair<BlackboardKey, object> entry in entries)
            {
                // Get the type of the entry
                Type entryType = entry.Value.GetType();

                // Check if the type is equal to the generic type of the type of the Blackboard Entry
                if(entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>))
                {
                    // Get the property of the value
                    PropertyInfo valueProperty = entryType.GetProperty("Value");

                    // If it is null, continue
                    if (valueProperty == null) continue;

                    // Otherwise, get the value of the property
                    object value = valueProperty.GetValue(entry.Value);

                    // Debug the key and the value
                    UnityEngine.Debug.Log($"Key: {entry.Key}, Value: {value}");
                }
            }
        }
    }
}