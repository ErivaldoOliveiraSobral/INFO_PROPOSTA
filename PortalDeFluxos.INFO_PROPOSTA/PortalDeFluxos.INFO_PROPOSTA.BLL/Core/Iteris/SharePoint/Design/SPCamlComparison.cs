using System;

namespace Iteris.SharePoint.Design {

	public class SPCamlComparison : SPCamlCondition {
		private SPCamlComparisonOperatorType _comparisonType = SPCamlComparisonOperatorType.Eq;
		public SPCamlComparisonOperatorType ComparisonType {
			get { return _comparisonType; }
			private set { _comparisonType = value; }
		}

		private String _fieldName = null;
		public String FieldName {
			get { return _fieldName; }
			private set { _fieldName = value; }
		}

		private Object _value = null;
		public Object Value {
			get { return _value; }
			private set { _value = value; }
		}

		private SPCamlFieldOptions _options = SPCamlFieldOptions.None;
		public SPCamlFieldOptions Options {
			get { return _options; }
			private set { _options = value; }
		}

		public SPCamlComparison(SPCamlComparisonOperatorType type, String fieldName, Object value, SPCamlFieldOptions options) {
			ComparisonType = type;
			FieldName = fieldName;
			Value = value;
			Options = options;
		}

		public SPCamlComparison(SPCamlComparisonOperatorType type, String fieldName, Object value) :
			this(type, fieldName, value, SPCamlFieldOptions.None) {
		}
	}

}
