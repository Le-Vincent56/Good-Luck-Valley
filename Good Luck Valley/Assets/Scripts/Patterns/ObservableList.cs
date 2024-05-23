using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Patterns
{
    public interface IObservableList<T>
    {
        /// <summary>
        /// Adds an item to the Observable List
        /// </summary>
        /// <param name="item">The item to add</param>
        void Add(T item);

        /// <summary>
        /// Removes all items from the Observable List
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether the Observable List contains a specific item
        /// </summary>
        /// <param name="item">The item to locate</param>
        /// <returns>True if the Observable List contains the item, False if not</returns>
        bool Contains(T item);

        /// <summary>
        /// Copies the elements of the Observable List to an array, starting at a particular index
        /// </summary>
        /// <param name="aray">The destination array of the elements</param>
        /// <param name="arrayIndex">The zero-based index in the array at which to begin the copy</param>
        void CopyTo(T[] aray, int arrayIndex);

        /// <summary>
        /// Get a generic enumerator that iterates through the Observable List
        /// </summary>
        /// <returns>A generic enumerator that iterates through the Observable List</returns>
        IEnumerator<T> GetEnumerator();

        /// <summary>
        /// Determines the index of a specific item in the Observable List
        /// </summary>
        /// <param name="item">The item to locate in the Observable List</param>
        /// <returns>The zero-based index of the item in the Observable List</returns>
        int IndexOf(T item);

        /// <summary>
        /// Inserts and item into the Observable List
        /// </summary>
        /// <param name="index">The zero-based index at which the item should be inserted</param>
        /// <param name="item">The item to insert into the Observable List</param>
        void Insert(int index, T item);

        /// <summary>
        /// Removes the first occurrence of a specific item from the Observable List
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item was found and removed, false if not</returns>
        bool Remove(T item);

        /// <summary>
        /// Removes the item at the specified index of the Observable List
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove</param>
        void RemoveAt(int index);
    }

    [Serializable]
    public class ObservableList<T> : IList<T>, IObservableList<T>
    {
        private readonly IList<T> list;
        public event Action<IList<T>> AnyValueChanged;

        public ObservableList(IList<T> initialList = null)
        {
            list = initialList ?? new List<T>();
        }

        public T this[int index]
        {
            get => list[index];
            set
            {
                list[index] = value;
                Invoke();
            }
        }

        public int Count => list.Count;
        public void Invoke() => AnyValueChanged?.Invoke(list);
        public bool IsReadOnly => list.IsReadOnly;

        public void Add(T item)
        {
            list.Add(item);
            Invoke();
        }

        public void Clear()
        {
            list.Clear();
            Invoke();
        }

        public bool Contains(T item) => list.Contains(item);

        public void CopyTo(T[] aray, int arrayIndex) => list.CopyTo(aray, arrayIndex);

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        public int IndexOf(T item) => list.IndexOf(item);

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            Invoke();
        }

        public bool Remove(T item)
        {
            bool result = list.Remove(item);
            if(result) Invoke();

            return result;
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}