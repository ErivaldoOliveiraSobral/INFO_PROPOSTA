using System.Collections.Generic;
using Iteris.Collections.Generic;

namespace Iteris {
	public static class KeyValuePairExtensions {

		public static WritableKeyValuePair<TKey, TValue> ToWritableKeyValuePair<TKey, TValue>(this KeyValuePair<TKey, TValue> value) {
			return new WritableKeyValuePair<TKey, TValue>(value.Key, value.Value);
		}

		public static KeyValuePair<TKey, TValue> FromWritableKeyValuePair<TKey, TValue>(this WritableKeyValuePair<TKey, TValue> value) {
			return new KeyValuePair<TKey, TValue>(value.Key, value.Value);
		}

	}
}
