using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Patterns.Blackboard
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Blackboard Data", menuName = "Blackboard/Blackboard Data")]
    public class BlackboardData : ScriptableObject
    {
        public List<BlackboardEntryData> entries = new List<BlackboardEntryData>();

        /// <summary>
        /// Set all entry values in this object into a Blackboard
        /// </summary>
        /// <param name="blackboard">The Blackboard to set values in</param>
        public void SetValuesOnBlackboard(Blackboard blackboard)
        {
            // Iterate through each entry
            foreach(BlackboardEntryData entry in entries)
            {
                // Set the value on the blackboard
                entry.SetValueOnBlackboard(blackboard);
            }
        }
    }

    [Serializable]
    public class BlackboardEntryData : ISerializationCallbackReceiver
    {
        public string keyName;
        public AnyValue.ValueType valueType;
        public AnyValue value;

        // Dispatch table to set different types of value on the Blackboard
        static Dictionary<AnyValue.ValueType, Action<Blackboard, BlackboardKey, AnyValue>> setValueDispatchTable 
            = new Dictionary<AnyValue.ValueType, Action<Blackboard, BlackboardKey, AnyValue>>()
        {
                { AnyValue.ValueType.Int, (blackboard, key, anyValue) => blackboard.SetValue<int>(key, anyValue) },
                { AnyValue.ValueType.Float, (blackboard, key, anyValue) => blackboard.SetValue<float>(key, anyValue) },
                { AnyValue.ValueType.Bool, (blackboard, key, anyValue) => blackboard.SetValue<bool>(key, anyValue) },
                { AnyValue.ValueType.String, (blackboard, key, anyValue) => blackboard.SetValue<string>(key, anyValue) },
                { AnyValue.ValueType.Vector2, (blackboard, key, anyValue) => blackboard.SetValue<Vector2>(key, anyValue) },

        };

        /// <summary>
        /// Set a value on the dispatch table to a value on the Blackboard
        /// </summary>
        /// <param name="blackboard">The Blackboard to set a value on</param>
        public void SetValueOnBlackboard(Blackboard blackboard)
        {
            // Get the key on th Blackboard
            BlackboardKey key = blackboard.GetOrRegisterKey(keyName);

            // Set the value on the dispatch table
            setValueDispatchTable[value.type](blackboard, key, value);
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => value.type = valueType;
    }

    [Serializable]
    public struct AnyValue
    {
        public enum ValueType { Int, Float, Bool, String, Vector2 }
        public ValueType type;

        // Storage for different types of values
        public int intValue;
        public float floatValue;
        public bool boolValue;
        public string stringValue;
        public Vector2 vector2Value;

        // Implicit conversion operators to convert AnyValue to different types
        public static implicit operator int(AnyValue value) => value.ConvertValue<int>();
        public static implicit operator float(AnyValue value) => value.ConvertValue<float>();
        public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();
        public static implicit operator string(AnyValue value) => value.ConvertValue<string>();
        public static implicit operator Vector2(AnyValue value) => value.ConvertValue<Vector2>();

        T ConvertValue<T>()
        {
            return type switch
            {
                ValueType.Int => AsInt<T>(intValue),
                ValueType.Float => AsFloat<T>(floatValue),
                ValueType.Bool => AsBool<T>(boolValue),
                ValueType.String => (T)(object)stringValue,
                ValueType.Vector2 => AsVector2<T>(vector2Value),
                _ => throw new NotSupportedException($"Not supported value type: {typeof(T)}")
            };
        }

        // Methods to convert primitive types to generic types with type safety and without boxing
        private T AsInt<T>(int value) => typeof(T) == typeof(int) && value is T correctType ? correctType : default;
        private T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default;
        private T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
        private T AsVector2<T>(Vector2 value) => typeof(T) == typeof(Vector2) && value is T correctType ? correctType : default;
    }
}
