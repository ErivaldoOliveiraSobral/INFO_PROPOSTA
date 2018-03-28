using System;

namespace Iteris.Collections.Generic {

	[Serializable]
	public struct WritableKeyValuePair<TKey, TValue> {
		private TKey _key;

		public TKey Key {
			get { return _key; }
			set { _key = value; }
		}

		private TValue _value;

		public TValue Value {
			get { return _value; }
			set { _value = value; }
		}

		public static Boolean operator ==(WritableKeyValuePair<TKey, TValue> left, WritableKeyValuePair<TKey, TValue> right) {
			return left.Equals(right);
		}

		public static Boolean operator !=(WritableKeyValuePair<TKey, TValue> left, WritableKeyValuePair<TKey, TValue> right) {
			return !left.Equals(right);
		}

		public WritableKeyValuePair(TKey key, TValue value) {
			_key = key;
			_value = value;
		}

		public override bool Equals(Object obj) {
			WritableKeyValuePair<TKey, TValue> convertedValue = default(WritableKeyValuePair<TKey, TValue>);

			if (!(obj is WritableKeyValuePair<TKey, TValue>))
				return false;

			convertedValue = (WritableKeyValuePair<TKey, TValue>)obj;

			return (Key.Equals(Key) && Value.Equals(convertedValue.Value));
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

	}

}
