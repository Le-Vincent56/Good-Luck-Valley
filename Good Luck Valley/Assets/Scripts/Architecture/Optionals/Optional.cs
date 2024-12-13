using System;
using System.Collections.Generic;

namespace GoodLuckValley.Architecture.Optionals
{
    public struct Optional<T>
    {
        public static readonly Optional<T> NoValue = new Optional<T>();

        private readonly bool hasValue;
        private readonly T value;

        public Optional(T value)
        {
            this.value = value;
            hasValue = true;
        }

        public bool HasValue => hasValue;

        public T Value => hasValue ? value : throw new InvalidOperationException("No Value");

        /// <summary>
        /// Get the value or default of the Optional
        /// </summary>
        public T GetValueOrDefault() => value;

        /// <summary>
        /// Get the Optional's value or the passed in default value
        /// </summary>
        public T GetValueOrDefault(T defaultValue) => hasValue ? value : defaultValue;

        /// <summary>
        /// Match the Optional to a corresponding function depending on whether or not it has a value or not
        /// </summary>
        public TResult Match<TResult>(Func<T, TResult> onValue, Func<TResult> onNoValue)
        {
            return hasValue ? onValue(value) : onNoValue();
        }

        /// <summary>
        /// Chain another Optional-producing function and flatten the result
        /// </summary>
        public Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> bind)
        {
            return hasValue ? bind(value) : Optional<TResult>.NoValue;
        }

        /// <summary>
        /// Transform the Optional to another typeof Optional using a mapping function
        /// </summary>
        public Optional<TResult> Select<TResult>(Func<T, TResult> map)
        {
            return hasValue ? new Optional<TResult>(map(value)) : Optional<TResult>.NoValue;
        }

        /// <summary>
        /// Combine two Optionals using a combining function
        /// </summary>
        public static Optional<TResult> Combine<T1, T2, TResult>(Optional<T1> first, Optional<T2> second, Func<T1, T2, TResult> combiner)
        {
            if(first.HasValue && second.HasValue)
            {
                return new Optional<TResult>(combiner(first.Value, second.Value));
            }

            return Optional<TResult>.NoValue;
        }

        /// <summary>
        /// Create an Optional with some value
        /// </summary>
        public static Optional<T> Some(T value) => new Optional<T>(value);

        /// <summary>
        /// Create an Optional with no value
        /// </summary>
        public static Optional<T> None() => NoValue;

        /// <summary>
        /// Equality override
        /// </summary>
        public override bool Equals(object obj) => obj is Optional<T> other && Equals(other);

        /// <summary>
        /// Strongly-typed equality method for runtime type checking
        /// </summary>
        public bool Equals(Optional<T> other) => !hasValue ? other.hasValue : EqualityComparer<T>.Default.Equals(value, other.value);

        /// <summary>
        /// Get the Optional's hash code
        /// </summary>
        public override int GetHashCode() => (hasValue.GetHashCode() * 397) ^  EqualityComparer<T>.Default.GetHashCode(value);

        /// <summary>
        /// Override the string conversion
        /// </summary>
        public override string ToString() => hasValue ? $"Some({value})" : "None";

        public static implicit operator Optional<T>(T value) => new Optional<T>(value);
        public static implicit operator bool(Optional<T> value) => value.hasValue;
        public static explicit operator T(Optional<T> value) => value.Value;
    }
}
