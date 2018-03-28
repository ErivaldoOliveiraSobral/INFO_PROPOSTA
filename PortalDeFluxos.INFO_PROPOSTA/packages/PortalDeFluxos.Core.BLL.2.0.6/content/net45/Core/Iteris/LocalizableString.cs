using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Iteris.Collections.Generic;

namespace Iteris {

	/// <summary>
	/// The LocalizedStringCollection class is commonly used to abstract strings of different cultures as a plain regular string.
	/// </summary>
	/// <example>
	/// <code>
	/// </code>
	/// </example>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "From MSDN: It is safe to suppress a warning to use the 'Collection' suffix if the type is a generalized data structure that might be extended or that will hold an arbitrary set of diverse items. (which is the case here) - http://msdn.microsoft.com/en-us/library/ms182244.aspx")]
	[Serializable]
	public sealed class LocalizableString : ICollection<WritableKeyValuePair<CultureLCID, String>>, IComparable, IDictionary<CultureLCID, String>, ICloneable {

		private Dictionary<CultureLCID, String> _internalDictionary = new Dictionary<CultureLCID, String>();

		public event EventHandler CollectionChanged = null;

		public Int32 Count {
			get { return _internalDictionary.Count; }
		}

		public Boolean IsReadOnly {
			get { return false; }
		}

		public String this[CultureLCID lcid] {
			get {
				try {
					if (_internalDictionary.Count == 0)
						return null;
					return _internalDictionary[lcid];
				}
				catch (KeyNotFoundException) {
					if (_internalDictionary.Keys.Count > 0)
						return this[_internalDictionary.Keys.First()];
					return null;
				}
			}
			set {
				_internalDictionary[lcid] = value;
				if (CollectionChanged != null)
					CollectionChanged(this, new EventArgs());
			}
		}

		public String this[String cultureInfoName] {
			get {
				return this[(CultureLCID)CultureInfo.GetCultureInfo(cultureInfoName).LCID];
			}
			set {
				this[(CultureLCID)CultureInfo.GetCultureInfo(cultureInfoName).LCID] = value;
			}
		}

		public ICollection<CultureLCID> Keys {
			get { return _internalDictionary.Keys; }
		}

		public ICollection<String> Values {
			get { return _internalDictionary.Values; }
		}

		public static implicit operator LocalizableString(String value) {
			return new LocalizableString() { { (CultureLCID)CultureInfo.CurrentUICulture.LCID, value } };
		}

		public static implicit operator String(LocalizableString value) {
			if (value == null)
				return null;

			return value.ToString();
		}

		public override string ToString() {
			return this[(CultureLCID)CultureInfo.CurrentUICulture.LCID];
		}

		public override Boolean Equals(Object obj) {
			if (!(obj is String))
				return false;

			foreach (String value in _internalDictionary.Values)
				if (value.Equals((String)obj, StringComparison.OrdinalIgnoreCase))
					return true;
			return false;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public void Add(WritableKeyValuePair<CultureLCID, String> item) {
			Add(item.Key, item.Value);
		}

		public void Add(CultureLCID lcid, String value) {
			_internalDictionary.Add(lcid, value);
			if (CollectionChanged != null)
				CollectionChanged(this, new EventArgs());
		}

		public void Add(KeyValuePair<CultureLCID, String> item) {
			_internalDictionary.Add(item.Key, item.Value);
			if (CollectionChanged != null)
				CollectionChanged(this, new EventArgs());
		}

		public void Clear() {
			_internalDictionary.Clear();
			if (CollectionChanged != null)
				CollectionChanged(this, new EventArgs());
		}

		public Boolean Contains(WritableKeyValuePair<CultureLCID, String> item) {
			return _internalDictionary.ContainsKey(item.Key) && _internalDictionary[item.Key] == item.Value;
		}

		public Boolean Contains(KeyValuePair<CultureLCID, String> item) {
			return _internalDictionary.ContainsKey(item.Key) && _internalDictionary[item.Key] == item.Value;
		}

		public Boolean ContainsKey(CultureLCID lcid) {
			return _internalDictionary.ContainsKey(lcid);
		}

		public Boolean ContainsValue(String value) {
			return _internalDictionary.ContainsValue(value);
		}

		public void CopyTo(WritableKeyValuePair<CultureLCID, String>[] array, Int32 arrayIndex) {
			_internalDictionary.ToList().ConvertAll(item => new WritableKeyValuePair<CultureLCID, String>(item.Key, item.Value)).CopyTo(array, arrayIndex);
		}

		public void CopyTo(KeyValuePair<CultureLCID, String>[] array, Int32 arrayIndex) {
			_internalDictionary.ToList().ConvertAll(item => new KeyValuePair<CultureLCID, String>(item.Key, item.Value)).CopyTo(array, arrayIndex);
		}

		public Boolean Remove(WritableKeyValuePair<CultureLCID, String> item) {
			Boolean result = _internalDictionary.Remove(item.Key);
			if (CollectionChanged != null)
				CollectionChanged(this, new EventArgs());
			return result;
		}

		public Boolean Remove(CultureLCID key) {
			Boolean result = _internalDictionary.Remove(key);
			if (CollectionChanged != null)
				CollectionChanged(this, new EventArgs());
			return result;
		}

		public Boolean Remove(KeyValuePair<CultureLCID, String> item) {
			Boolean result = _internalDictionary.Remove(item.Key);
			if (CollectionChanged != null)
				CollectionChanged(this, new EventArgs());
			return result;
		}

		public Boolean TryGetValue(CultureLCID key, out String value) {
			return _internalDictionary.TryGetValue(key, out value);
		}

		public List<WritableKeyValuePair<CultureLCID, String>> ToList() {
			return _internalDictionary.ToList().ConvertAll(item => new WritableKeyValuePair<CultureLCID, String>(item.Key, item.Value));
		}

		public IEnumerator<WritableKeyValuePair<CultureLCID, String>> GetEnumerator() {
			return new LocalizableStringEnumerator(this);
		}

		IEnumerator<KeyValuePair<CultureLCID, String>> IEnumerable<KeyValuePair<CultureLCID, String>>.GetEnumerator() {
			return _internalDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new LocalizableStringEnumerator(this);
		}

		public Int32 CompareTo(Object obj) {
			LocalizableString localizableString = null;

			if (obj == null)
				throw new ArgumentNullException("obj");

			localizableString = obj as LocalizableString;

			if (localizableString == null)
				throw new ArgumentOutOfRangeException("obj");

			return ToString().CompareTo(localizableString.ToString());
		}

		public Object Clone() {
			LocalizableString newLocalizableString = new LocalizableString();
			foreach (KeyValuePair<CultureLCID, String> item in _internalDictionary)
				newLocalizableString.Add(item);
			return newLocalizableString;
		}

		public static LocalizableString Create(Hashtable values) {
			LocalizableString localizableString = new LocalizableString();
			foreach (DictionaryEntry value in values)
				localizableString.Add((CultureLCID)value.Key, (String)value.Value);
			return localizableString;
		}

		public class LocalizableStringEnumerator : IEnumerator<WritableKeyValuePair<CultureLCID, String>> {

			protected List<WritableKeyValuePair<CultureLCID, String>> _collection = null;
			protected Int32 _index = -1;
			protected WritableKeyValuePair<CultureLCID, String> _current = new WritableKeyValuePair<CultureLCID, String>();

			public WritableKeyValuePair<CultureLCID, String> Current {
				get { return _current; }
			}

			object IEnumerator.Current {
				get { return _current; }
			}

			public LocalizableStringEnumerator(LocalizableString collection) {
				_collection = collection.ToList();
			}

			public Boolean MoveNext() {
				if (++_index >= _collection.Count)
					return false;
				_current = _collection[_index];
				return true;
			}

			public void Reset() {
				_current = new WritableKeyValuePair<CultureLCID, String>();
				_index = -1;
			}

			public void Dispose() {
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(Boolean disposing) {
			}
		}

	}

}
