using System;
using System.Collections.Generic;

namespace Iteris {
	public class LambdaEqualityComparer<T> : IEqualityComparer<T> {
		private Func<T, T, Boolean> _equalsDelegate = null;
		private Func<T, Int32> _getHashCodeDelegate = null;

		public bool Equals(T x, T y) {
			if (_equalsDelegate == null)
				return false;
			return _equalsDelegate(x, y);
		}

		public int GetHashCode(T obj) {
			if (_getHashCodeDelegate == null)
				return 0;
			return _getHashCodeDelegate(obj);
		}

		public LambdaEqualityComparer(Func<T, T, Boolean> equalsDelegate, Func<T, Int32> getHashCodeDelegate) {
			_equalsDelegate = equalsDelegate;
			_getHashCodeDelegate = getHashCodeDelegate;
		}
	}
}
