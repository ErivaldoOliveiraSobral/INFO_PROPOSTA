using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Iteris {
	public static class CollectionExtensions {

		public static void AddRange<T>(this Collection<T> collection, IEnumerable<T> values) {
			foreach (T item in values)
				collection.Add(item);
		}

	}
}
