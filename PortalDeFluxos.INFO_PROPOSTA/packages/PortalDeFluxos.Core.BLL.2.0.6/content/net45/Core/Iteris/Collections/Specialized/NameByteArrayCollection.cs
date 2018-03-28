using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Iteris.Collections.Generic;

namespace Iteris.Collections.Specialized {

	/// <summary>
	/// Represents a collection of associated System.String keys and System.Byte[] values
	/// that can be accessed either with the key or with the index.
	/// </summary>
	[Serializable]
	public class NameByteArrayCollection : NameObjectCollectionBase, ISerializable, ICollection<WritableKeyValuePair<String, Byte[]>> {

		// Creates an empty collection.
		public NameByteArrayCollection() {
		}

		// Adds elements from an IDictionary into the new collection.
		public NameByteArrayCollection(IDictionary dictionary) {
			if (dictionary == null)
				return;

			foreach (DictionaryEntry entry in dictionary)
				BaseAdd((String)entry.Key, entry.Value);
		}

		// Adds elements from an IDictionary into the new collection.
		public NameByteArrayCollection(IDictionary dictionary, Boolean readOnly)
			: this(dictionary) {
			IsReadOnly = readOnly;
		}

		// Gets a key-and-value pair (DictionaryEntry) using an index.
		public WritableKeyValuePair<String, Byte[]> this[Int32 index] {
			get {
				return new WritableKeyValuePair<String, Byte[]>(BaseGetKey(index), (Byte[])BaseGet(index));
			}
		}

		// Gets or sets the value associated with the specified key.
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Accordingly to the Framework Design Guidelines book, an exception can be made for byte arrays.")]
		public Byte[] this[String key] {
			get {
				return (Byte[])BaseGet(key);
			}
			set {
				BaseSet(key, value);
			}
		}

		// Gets a String array that contains all the keys in the collection.
		public ICollection<String> AllKeys {
			get { return BaseGetAllKeys(); }
		}

		// Gets an Byte[] array that contains all the values in the collection.
		public Array AllValues {
			get { return BaseGetAllValues(); }
		}

		// Gets a value indicating if the collection contains keys that are not null.
		public Boolean HasKeys {
			get {
				return (BaseHasKeys());
			}
		}

		// Adds an entry to the collection.
		public void Add(String key, Byte[] value) {
			BaseAdd(key, value);
		}

		// Adds a range of entries to the collection.
		public void AddRange(NameByteArrayCollection value) {
			if (value == null)
				return;

			foreach (WritableKeyValuePair<String, Byte[]> entry in value)
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

		public void CopyTo(NameByteArrayCollection array, int index) {
			if (array == null)
				throw new ArgumentNullException("array");

			for (Int32 i = index; i < array.Count && i < this.Count; ++i)
				array.Add(this[i].Key, this[i].Value);
		}

		public void Add(WritableKeyValuePair<String, Byte[]> item) {
			Add(item.Key, item.Value);
		}

		public Boolean Contains(WritableKeyValuePair<String, Byte[]> item) {
			return this.Any(keyValueItem => keyValueItem.Key == item.Key);
		}

		public void CopyTo(WritableKeyValuePair<String, Byte[]>[] array, Int32 arrayIndex) {
			if (array == null)
				throw new ArgumentNullException("array");

			for (Int32 i = 0; i < array.Length && (i + arrayIndex) < Count; ++i)
				array[i] = this[i + arrayIndex];
		}

		public new Boolean IsReadOnly {
			get { return base.IsReadOnly; }
			set { base.IsReadOnly = value; }
		}

		public Boolean Remove(WritableKeyValuePair<String, Byte[]> item) {
			Boolean contains = this.Contains(item);
			this.Remove(item.Key);
			return contains;
		}

		public new IEnumerator<WritableKeyValuePair<String, Byte[]>> GetEnumerator() {
			return new NameByteArrayCollection.Enumerator(this);
		}

		[Serializable]
		public struct Enumerator : IEnumerator<WritableKeyValuePair<String, Byte[]>>, IDisposable, IEnumerator {
			private Int32 _index;
			private NameByteArrayCollection _collection;

			public Enumerator(NameByteArrayCollection collection) {
				_index = -1;
				_collection = collection;
			}

			public WritableKeyValuePair<String, Byte[]> Current {
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

		protected NameByteArrayCollection(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			if (info == null)
				throw new ArgumentNullException("info");

			this.AddRange((NameByteArrayCollection)info.GetValue("Value", this.GetType()));
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			if (info == null)
				throw new ArgumentNullException("info");

			base.GetObjectData(info, context);

			info.AddValue("Value", this, this.GetType());
		}

	}

}
