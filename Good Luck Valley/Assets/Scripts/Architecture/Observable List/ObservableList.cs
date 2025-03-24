using System;
using System.Collections;
using System.Collections.Generic;

namespace GoodLuckValley.Architecture.ObservableList
{
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

        /// <summary>
        /// Invoke the AnyValueChanged event
        /// </summary>
        public void Invoke() => AnyValueChanged?.Invoke(list);

        /// <summary>
        /// Get the Count of the Observable List
        /// </summary>
        public int Count => list.Count;

        /// <summary>
        /// Check if the Observable List is read-only
        /// </summary>
        public bool IsReadOnly => list.IsReadOnly;

        /// <summary>
        /// Add an item to the Observable List
        /// </summary>
        public void Add(T item)
        {
            list.Add(item);
            Invoke();
        }

        /// <summary>
        /// Clear the Observable List
        /// </summary>
        public void Clear()
        {
            list.Clear();
            Invoke();
        }

        /// <summary>
        /// Check if the Observable List contains an item
        /// </summary>
        public bool Contains(T item) => list.Contains(item);


        /// <summary>
        /// Copy an array of items into an index within the Observable List
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        /// <summary>
        /// Remove an item from the Observable List
        /// </summary>
        public bool Remove(T item)
        {
            var result = list.Remove(item);
            if (result)
            {
                Invoke();
            }

            return result;
        }

        /// <summary>
        /// Get the Observable List's typed enumerator
        /// </summary>
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        /// <summary>
        /// Get the Observable List's generic enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        /// <summary>
        /// Get the index of an item within the Observable List
        /// </summary>
        public int IndexOf(T item) => list.IndexOf(item);

        /// <summary>
        /// Insert an item at a certain index in the Observable List
        /// </summary>
        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            Invoke();
        }

        /// <summary>
        /// Remove an item at a specified index within the Observable List
        /// </summary>
        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
            Invoke();
        }
    }
}