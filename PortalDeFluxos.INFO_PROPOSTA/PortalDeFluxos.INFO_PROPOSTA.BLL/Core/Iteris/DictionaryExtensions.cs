using System;
using System.Collections.Generic;

namespace Iteris {
	public static class DictionaryExtensions {

		public static T TryGetValue<TKey, T>(this Dictionary<TKey, T> dictionary, TKey key) {
			T obj;
			dictionary.TryGetValue(key, out obj);
			return obj;
		}

		public static TTarget TryGetValueAs<TKey, TTarget>(this Dictionary<TKey, Object> dictionary, TKey key) {
			Object obj;
			dictionary.TryGetValue(key, out obj);
			return (TTarget)obj;
		}
	}
}
