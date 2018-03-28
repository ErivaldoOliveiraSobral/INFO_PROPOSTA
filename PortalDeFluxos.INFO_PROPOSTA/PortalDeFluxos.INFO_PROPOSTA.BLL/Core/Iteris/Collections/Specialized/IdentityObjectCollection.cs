using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Iteris.Collections.Generic;

namespace Iteris.Collections.Specialized {

	/// <summary>
	/// Represents a collection of associated System.Int32 keys and System.Object values
	/// that can be accessed either with the key or with the index.
	/// </summary>
	[Serializable]
	public class IdentityObjectCollection : ISerializable, IEnumerable<WritableKeyValuePair<Int32, Object>> {

		private Dictionary<Int32, Object> _internalDictionary = new Dictionary<Int32, Object>();

		// Creates an empty collection.
		public IdentityObjectCollection() {
		}

		// Adds elements from an IDictionary into the new collection.
		public IdentityObjectCollection(IDictionary dictionary) {
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			foreach (DictionaryEntry entry in dictionary)
				_internalDictionary.Add((Int32)entry.Key, entry.Value);
		}

		// Gets or sets the value associated with the specified key.
		public Object this[Int32 key] {
			get {
				return _internalDictionary[key];
			}
			set {
				_internalDictionary[key] = value;
			}
		}

		// Gets a String array that contains all the keys in the collection.
		public ICollection<Int32> AllKeys {
			get { return _internalDictionary.Keys; }
		}

		// Gets an Object array that contains all the values in the collection.
		public Array AllValues {
			get { return _internalDictionary.Values.ToArray(); }
		}

		// Adds an entry to the collection.
		public void Add(Int32 key, Object value) {
			_internalDictionary.Add(key, value);
		}

		public void Add(WritableKeyValuePair<Int32, Object> item) {
			_internalDictionary.Add(item.Key, item.Value);
		}

		// Adds a range of entries to the collection.
		public void AddRange(IdentityObjectCollection value) {
			if (value == null)
				return;

			foreach (WritableKeyValuePair<Int32, Object> entry in value)
				_internalDictionary.Add(entry.Key, entry.Value);
		}

		// Removes an entry with the specified key from the collection.
		public void Remove(Int32 key) {
			_internalDictionary.Remove(key);
		}

		// Clears all the elements in the collection.
		public void Clear() {
			_internalDictionary.Clear();
		}

		public void CopyTo(IdentityObjectCollection array, Int32 index) {
			if (array == null)
				throw new ArgumentNullException("array");

			for (Int32 i = index; i < array.Count() && i < this.Count(); ++i)
				array.Add(_internalDictionary.ElementAt(i).ToWritableKeyValuePair());
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
			_internalDictionary.GetObjectData(info, context);
		}

		protected IdentityObjectCollection(SerializationInfo info, StreamingContext context) {
			if (info == null)
				throw new ArgumentNullException("info");

			AddRange((IdentityObjectCollection)info.GetValue("Value", this.GetType()));
		}

		public IEnumerator<WritableKeyValuePair<Int32, Object>> GetEnumerator() {
			return _internalDictionary.ToList().ConvertAll(item => new WritableKeyValuePair<Int32, Object>(item.Key, item.Value)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _internalDictionary.GetEnumerator();
		}

	}

}
