using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using Iteris.SharePoint.Design;
using Microsoft.SharePoint;
using Iteris.Data;
using Microsoft.SharePoint.Client;
using PortalDeFluxos.Core.BLL;

namespace Iteris.SharePoint {

	[Flags]
	public enum SPCamlFieldOptions {
		None,
		LookupFieldById,
		IncludeTimeValue
	}
    
	public static class SPCamlComparisonOperator {
		/// <summary>
		/// Searches for a string at the start of a column that holds Text or Note field type values.
		/// </summary>
		public static SPCamlComparison BeginsWith(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.BeginsWith, title, value, options);
		}

		/// <summary>
		/// Searches for a string at the start of a column that holds Text or Note field type values.
		/// </summary>
		public static SPCamlComparison BeginsWith(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.BeginsWith, title, value);
		}

		/// <summary>
		/// Searches for a string anywhere within a column that holds Text or Note field type values. 
		/// </summary>
		public static SPCamlComparison Contains(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Contains, title, value, options);
		}

		/// <summary>
		/// Searches for a string anywhere within a column that holds Text or Note field type values. 
		/// </summary>
		public static SPCamlComparison Contains(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Contains, title, value);
		}

		/// <summary>
		/// Used in queries to compare the dates in a recurring event with a specified DateTime value, to determine whether they overlap.
		/// </summary>
		public static SPCamlComparison DateRangesOverlap(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.DateRangesOverlap, title, value, options);
		}

		/// <summary>
		/// Used in queries to compare the dates in a recurring event with a specified DateTime value, to determine whether they overlap.
		/// </summary>
		public static SPCamlComparison DateRangesOverlap(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.DateRangesOverlap, title, value);
		}

		/// <summary>
		/// Arithmetic operator that means "equal to".
		/// </summary>
		public static SPCamlComparison Equal(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Eq, title, value, options);
		}

		/// <summary>
		/// Arithmetic operator that means "equal to".
		/// </summary>
		public static SPCamlComparison Equal(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Eq, title, value);
		}

		/// <summary>
		/// Arithmetic operator that means "greater than or equal to.".
		/// </summary>
		public static SPCamlComparison GreaterOrEqual(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Geq, title, value, options);
		}

		/// <summary>
		/// Arithmetic operator that means "greater than or equal to.".
		/// </summary>
		public static SPCamlComparison GreaterOrEqual(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Geq, title, value);
		}

		/// <summary>
		/// Arithmetic operator that means "greater than.".
		/// </summary>
		public static SPCamlComparison Greater(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Gt, title, value, options);
		}

		/// <summary>
		/// Arithmetic operator that means "greater than.".
		/// </summary>
		public static SPCamlComparison Greater(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Gt, title, value);
		}

		/// <summary>
		/// Specifies whether the value of a list item for the field specified by the FieldRef element is equal to one of the values specified by the Values element.
		/// </summary>
		public static SPCamlComparison In(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.In, title, value, options);
		}

		/// <summary>
		/// Specifies whether the value of a list item for the field specified by the FieldRef element is equal to one of the values specified by the Values element.
		/// </summary>
		public static SPCamlComparison In(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.In, title, value);
		}

		/// <summary>
		/// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is included in the list item for the field that is specified by the FieldRef element.
		/// </summary>
		public static SPCamlComparison Includes(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Includes, title, value, options);
		}

		/// <summary>
		/// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is included in the list item for the field that is specified by the FieldRef element.
		/// </summary>
		public static SPCamlComparison Includes(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Includes, title, value);
		}

		/// <summary>
		/// Used within a query to return items that are not empty (Null).
		/// </summary>
		public static SPCamlComparison IsNotNull(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.IsNotNull, title, value, options);
		}

		/// <summary>
		/// Used within a query to return items that are not empty (Null).
		/// </summary>
		public static SPCamlComparison IsNotNull(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.IsNotNull, title, value);
		}

		/// <summary>
		/// Used within a query to return items that are empty (Null).
		/// </summary>
		public static SPCamlComparison IsNull(String title, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.IsNull, title, null, options);
		}

		/// <summary>
		/// Used within a query to return items that are empty (Null).
		/// </summary>
		public static SPCamlComparison IsNull(String title) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.IsNull, title, null);
		}

		/// <summary>
		/// Arithmetic operator that means "less than or equal to.".
		/// </summary>
		public static SPCamlComparison LessOrEqual(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Leq, title, value, options);
		}

		/// <summary>
		/// Arithmetic operator that means "less than or equal to.".
		/// </summary>
		public static SPCamlComparison LessOrEqual(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Leq, title, value);
		}

		/// <summary>
		/// Arithmetic operator that means "less than.".
		/// </summary>
		public static SPCamlComparison Less(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Lt, title, value, options);
		}

		/// <summary>
		/// Arithmetic operator that means "less than.".
		/// </summary>
		public static SPCamlComparison Less(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Lt, title, value);
		}

		/// <summary>
		/// Defines a filter based on the type of membership for the user.
		/// </summary>
		public static SPCamlComparison Membership(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Membership, title, value, options);
		}

		/// <summary>
		/// Defines a filter based on the type of membership for the user.
		/// </summary>
		public static SPCamlComparison Membership(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Membership, title, value);
		}

		/// <summary>
		/// Arithmetic operator that means "not equal to.".
		/// </summary>
		public static SPCamlComparison NotEqual(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Neq, title, value, options);
		}

		/// <summary>
		/// Arithmetic operator that means "not equal to.".
		/// </summary>
		public static SPCamlComparison NotEqual(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.Neq, title, value);
		}

		/// <summary>
		/// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is excluded from the list item for the field that is specified by the FieldRef element.
		/// </summary>
		public static SPCamlComparison NotIncludes(String title, Object value, SPCamlFieldOptions options) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.NotIncludes, title, value, options);
		}

		/// <summary>
		/// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is excluded from the list item for the field that is specified by the FieldRef element.
		/// </summary>
		public static SPCamlComparison NotIncludes(String title, Object value) {
			return new SPCamlComparison(SPCamlComparisonOperatorType.NotIncludes, title, value);
		}
	}

	public static class SPCamlLogicalOperator {
		/// <summary>
		/// Used to group fields in a query.
		/// </summary>
		public static SPCamlLogical And(SPCamlCondition firstCondition, SPCamlCondition secondCondition) {
			return new SPCamlLogical(SPCamlLogicalOperatorType.And, firstCondition, secondCondition);
		}

		/// <summary>
		/// Used to group fields in a query.
		/// </summary>
		public static SPCamlLogical Or(SPCamlCondition firstCondition, SPCamlCondition secondCondition) {
			return new SPCamlLogical(SPCamlLogicalOperatorType.Or, firstCondition, secondCondition);
		}
	}

	public static class SPCamlBuilder {

		[SecurityPermission(SecurityAction.LinkDemand)]
        private static String CreateCamlFromComparison(List<Field> fields, SPCamlComparison comparison)
        {
			StringBuilder camlBuilder = new StringBuilder();
			Field field = fields.FirstOrDefault(i => String.Equals(i.InternalName, comparison.FieldName, StringComparison.InvariantCultureIgnoreCase));
			Array values = null;

			camlBuilder.AppendFormat(@"<{0}>", comparison.ComparisonType.ToString());

			// FieldRef element
			camlBuilder.AppendFormat(@"<FieldRef Name=""{0}""", field.InternalName);

			// Check for custom properties defined to this field.
            if (field.FieldTypeKind == FieldType.Lookup || field.FieldTypeKind == FieldType.User)
				camlBuilder.AppendFormat(@" LookupId=""{0}""", ((comparison.Options & SPCamlFieldOptions.LookupFieldById) > 0).ToString().ToUpper(CultureInfo.InvariantCulture));

			camlBuilder.Append(@"/>");

			// Check whether we should deal with multiple values.
			if (comparison.ComparisonType == SPCamlComparisonOperatorType.In)
				camlBuilder.Append(@"<Values>");

			// The value can be null for some operators.
			if (comparison.Value != null) {
				// Create an array of values.
				if (comparison.Value is Array)
					values = (Array)comparison.Value;
				else
					values = new Object[1] { comparison.Value };

				foreach (Object value in values) {
					camlBuilder.AppendFormat(@"<Value Type=""{0}""", field.TypeAsString);

					// Check for custom properties defined to this field.
                    if (field.FieldTypeKind == FieldType.DateTime)
						camlBuilder.AppendFormat(@" IncludeTimeValue=""{0}""", ((comparison.Options & SPCamlFieldOptions.IncludeTimeValue) > 0).ToString().ToUpper(CultureInfo.InvariantCulture));

					camlBuilder.Append(@">");

                    if (field.FieldTypeKind == FieldType.DateTime)
                        camlBuilder.Append(((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ssZ"));
					else
						camlBuilder.Append(value.ToString());

					camlBuilder.Append("</Value>");
				} // foreach
			}

			// Check whether we should deal with multiple values.
			if (comparison.ComparisonType == SPCamlComparisonOperatorType.In)
				camlBuilder.Append(@"</Values>");

			camlBuilder.AppendFormat(@"</{0}>", comparison.ComparisonType.ToString());

			return camlBuilder.ToString();
		}

        private static String CreateCamlFromLogicalJoin(List<Field> fields, SPCamlLogical logicalJoin)
        {
			StringBuilder camlBuilder = new StringBuilder();
			camlBuilder.AppendFormat(@"<{0}>", logicalJoin.LogicalJoinType.ToString());

			if (logicalJoin.FirstLogicalJoinOrComparison != null)
				if (logicalJoin.FirstLogicalJoinOrComparison is SPCamlComparison)
					camlBuilder.Append(CreateCamlFromComparison(fields, (SPCamlComparison)logicalJoin.FirstLogicalJoinOrComparison));
				else
					camlBuilder.Append(CreateCamlFromLogicalJoin(fields, (SPCamlLogical)logicalJoin.FirstLogicalJoinOrComparison));

			if (logicalJoin.SecondLogicalJoinOrComparison != null)
				if (logicalJoin.SecondLogicalJoinOrComparison is SPCamlComparison)
					camlBuilder.Append(CreateCamlFromComparison(fields, (SPCamlComparison)logicalJoin.SecondLogicalJoinOrComparison));
				else
					camlBuilder.Append(CreateCamlFromLogicalJoin(fields, (SPCamlLogical)logicalJoin.SecondLogicalJoinOrComparison));

			camlBuilder.AppendFormat(@"</{0}>", logicalJoin.LogicalJoinType.ToString());

			return camlBuilder.ToString();
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
        private static String CreateCamlFromOrderByCollection(List<Field> fields, OrderByCollection orderByDefinition)
        {
			Field field = null;
			StringBuilder camlBuilder = new StringBuilder();

			camlBuilder.Append(@"<OrderBy>");

			foreach (KeyValuePair<String, OrderByDirection> orderByItem in orderByDefinition) {
                field = fields.FirstOrDefault(i => String.Equals(i.InternalName, orderByItem.Key, StringComparison.InvariantCultureIgnoreCase));
				camlBuilder.AppendFormat(
					@"<FieldRef Name=""{0}"" Ascending=""{1}""/>",
					field.InternalName,
					(orderByItem.Value == OrderByDirection.Ascending).ToString().ToUpper(CultureInfo.InvariantCulture)
				);
			}

			camlBuilder.Append(@"</OrderBy>");

			return camlBuilder.ToString();
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
        private static String CreateCamlFromViewFieldsCollection(List<Field> fields, StringCollection viewFields)
        {
			StringBuilder camlBuilder = new StringBuilder();

			foreach (String viewField in viewFields)
				camlBuilder.AppendFormat(@"<FieldRef Name=""{0}""/>", fields.FirstOrDefault(i => String.Equals(i.InternalName, viewField, StringComparison.InvariantCultureIgnoreCase)));

			return camlBuilder.ToString();
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		public static CamlQuery Build(PortalWeb contexto, List list, SPCamlCondition condition, StringCollection viewFields, OrderByCollection orderByDefinition) {
            CamlQuery query = new CamlQuery();
			SPCamlComparison comparison = condition as SPCamlComparison;
			SPCamlLogical logicalJoin = condition as SPCamlLogical;

            //Carrega a lista de campos
            IEnumerable<Field> fields = contexto.SPClient.LoadQuery(list.Fields.Include(i => i.InternalName,
                                                                                                    i => i.TypeAsString,
                                                                                                    i => i.FieldTypeKind));
            contexto.SPClient.ExecuteQuery();

            //Busca a lista de campos
            List<Field> fieldList = fields.ToList();

			if (comparison != null)
                query.ViewXml = CreateCamlFromComparison(fieldList, comparison);

			if (logicalJoin != null)
                query.ViewXml = CreateCamlFromLogicalJoin(fieldList, logicalJoin);

            if (!String.IsNullOrEmpty(query.ViewXml))
                query.ViewXml = String.Concat(@"<Where>", query.ViewXml, @"</Where>");

			if (orderByDefinition != null)
                query.ViewXml = String.Concat(query.ViewXml, CreateCamlFromOrderByCollection(fieldList, orderByDefinition));

			if (viewFields != null)
                query.ViewXml = String.Concat(@"<ViewFields>", CreateCamlFromViewFieldsCollection(fieldList, viewFields), @"</ViewFields>");

            if (!String.IsNullOrWhiteSpace(query.ViewXml))
                query.ViewXml = String.Concat(@"<View><Query>", query.ViewXml, @"</Query></View>");

            return query;
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
        public static CamlQuery Build(PortalWeb contexto, List list, SPCamlCondition condition, StringCollection viewFields)
        {
            return Build(contexto, list, condition, viewFields, null);
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
        public static CamlQuery Build(PortalWeb contexto, List list, SPCamlCondition condition)
        {
            return Build(contexto, list, condition, null);
		}
    }
}
