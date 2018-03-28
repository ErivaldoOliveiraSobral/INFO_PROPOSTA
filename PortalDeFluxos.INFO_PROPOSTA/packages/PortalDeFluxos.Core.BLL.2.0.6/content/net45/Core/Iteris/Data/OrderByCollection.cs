using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace Iteris.Data {

	/// <summary>
	/// Represents a collection of associated System.String keys and OrderByDirection values
	/// that can be accessed either with the key or with the index.
	/// </summary>
	[Serializable]
	public class OrderByCollection : NameObjectCollectionBase, ICollection<KeyValuePair<String, OrderByDirection>> {

		// Creates an empty collection.
		public OrderByCollection() {
		}

		// Adds elements from an IDictionary into the new collection.
		public OrderByCollection(IDictionary dictionary) {
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			foreach (DictionaryEntry entry in dictionary)
				BaseAdd((String)entry.Key, (OrderByDirection)entry.Value);
		}

		// Adds elements from an IDictionary into the new collection.
		public OrderByCollection(IDictionary dictionary, Boolean readOnly)
			: this(dictionary) {
			IsReadOnly = readOnly;
		}

		protected OrderByCollection(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}

		// Gets a key-and-value pair (DictionaryEntry) using an index.
		public KeyValuePair<String, OrderByDirection> this[Int32 index] {
			get {
				return new KeyValuePair<String, OrderByDirection>(BaseGetKey(index), (OrderByDirection)BaseGet(index));
			}
		}

		// Gets or sets the value associated with the specified key.
		public OrderByDirection this[String key] {
			get {
				return (OrderByDirection)BaseGet(key);
			}
			set {
				BaseSet(key, value);
			}
		}

		// Gets a String array that contains all the keys in the collection.
		public ICollection<String> AllKeys {
			get {
				return BaseGetAllKeys();
			}
		}

		// Gets an Byte[] array that contains all the values in the collection.
		public Array AllValues {
			get {
				return BaseGetAllValues();
			}
		}

		// Gets a value indicating if the collection contains keys that are not null.
		public Boolean HasKeys {
			get {
				return (BaseHasKeys());
			}
		}

		// Adds an entry to the collection.
		public void Add(String key, OrderByDirection value) {
			BaseAdd(key, value);
		}

		public void Add(KeyValuePair<String, OrderByDirection> item) {
			Add(item.Key, item.Value);
		}

		// Adds a range of entries to the collection.
		public void AddRange(OrderByCollection value) {
			if (value == null)
				return;

			foreach (KeyValuePair<String, OrderByDirection> entry in value)
				BaseAdd(entry.Key, entry.Value);
		}

		// Removes an entry with the specified key from the collection.
		public void Remove(String key) {
			BaseRemove(key);
		}

		// Removes an entry in the specified index from the collection.
		public void Remove(Int32 index) {
			BaseRemoveAt(index);
		}

		// Clears all the elements in the collection.
		public void Clear() {
			BaseClear();
		}

		public void CopyTo(OrderByCollection collection, Int32 index) {
			if (collection == null)
				throw new ArgumentNullException("collection");

			for (Int32 i = index; i < Count; ++i)
				collection.Add(this[i]);
		}

		public bool Contains(KeyValuePair<String, OrderByDirection> item) {
			return this.Any(keyValueItem => keyValueItem.Key == item.Key);
		}

		public void CopyTo(KeyValuePair<String, OrderByDirection>[] array, int arrayIndex) {
			if (array == null)
				throw new ArgumentNullException("array");

			for (Int32 i = 0; i < array.Length && (i + arrayIndex) < Count; ++i)
				array[i] = this[i + arrayIndex];
		}

		public new bool IsReadOnly {
			get { return base.IsReadOnly; }
			set { base.IsReadOnly = value; }
		}

		public bool Remove(KeyValuePair<String, OrderByDirection> item) {
			Boolean contains = Contains(item);
			Remove(item.Key);
			return contains;
		}

		public new IEnumerator<KeyValuePair<String, OrderByDirection>> GetEnumerator() {
			return new OrderByCollection.Enumerator(this);
		}

		[Serializable]
		public struct Enumerator : IEnumerator<KeyValuePair<String, OrderByDirection>>, IDisposable, IEnumerator {
			private Int32 _index;
			private OrderByCollection _collection;

			public Enumerator(OrderByCollection collection) {
				_index = -1;
				_collection = collection;
			}

			public KeyValuePair<String, OrderByDirection> Current {
				get { return _collection[_index]; }
			}

			public bool MoveNext() {
				if (++_index < _collection.Count)
					return true;
				--_index;
				return false;
			}

			public void Reset() {
				_index = 0;
			}

			object IEnumerator.Current {
				get {
					return _collection[_index];
				}
			}

			public void Dispose() {
			}

		}

	}

}
