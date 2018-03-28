
namespace Iteris.SharePoint.Design {

	public enum SPCamlComparisonOperatorType {
		/// <summary>
		/// Searches for a string at the start of a column that holds Text or Note field type values.
		/// </summary>
		BeginsWith,
		/// <summary>
		/// Searches for a string anywhere within a column that holds Text or Note field type values. 
		/// </summary>
		Contains,
		/// <summary>
		/// Used in queries to compare the dates in a recurring event with a specified DateTime value, to determine whether they overlap.
		/// </summary>
		DateRangesOverlap,
		/// <summary>
		/// Arithmetic operator that means "equal to".
		/// </summary>
		Eq,
		/// <summary>
		/// Arithmetic operator that means "greater than or equal to.".
		/// </summary>
		Geq,
		/// <summary>
		/// Arithmetic operator that means "greater than.".
		/// </summary>
		Gt,
		/// <summary>
		/// Specifies whether the value of a list item for the field specified by the FieldRef element is equal to one of the values specified by the Values element.
		/// </summary>
		In,
		/// <summary>
		/// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is included in the list item for the field that is specified by the FieldRef element.
		/// </summary>
		Includes,
		/// <summary>
		/// Used within a query to return items that are not empty (Null).
		/// </summary>
		IsNotNull,
		/// <summary>
		/// Used within a query to return items that are empty (Null).
		/// </summary>
		IsNull,
		/// <summary>
		/// Arithmetic operator that means "less than or equal to.".
		/// </summary>
		Leq,
		/// <summary>
		/// Arithmetic operator that means "less than.".
		/// </summary>
		Lt,
		/// <summary>
		/// Defines a filter based on the type of membership for the user.
		/// </summary>
		Membership,
		/// <summary>
		/// Arithmetic operator that means "not equal to.".
		/// </summary>
		Neq,
		/// <summary>
		/// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is excluded from the list item for the field that is specified by the FieldRef element.
		/// </summary>
		NotIncludes
	}

	public enum SPCamlLogicalOperatorType {
		/// <summary>
		/// Used to group fields in a query.
		/// </summary>
		And,
		/// <summary>
		/// Used to group fields in a query.
		/// </summary>
		Or
	}

	public abstract class SPCamlCondition {
	}

}
