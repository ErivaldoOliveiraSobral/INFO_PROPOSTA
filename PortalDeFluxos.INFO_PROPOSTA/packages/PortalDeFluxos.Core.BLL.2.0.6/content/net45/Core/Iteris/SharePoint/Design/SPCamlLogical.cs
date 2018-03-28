
namespace Iteris.SharePoint.Design {

	public class SPCamlLogical : SPCamlCondition {
		private SPCamlLogicalOperatorType _logicalJoinType = SPCamlLogicalOperatorType.And;
		public SPCamlLogicalOperatorType LogicalJoinType {
			get { return _logicalJoinType; }
			internal set { _logicalJoinType = value; }
		}

		private SPCamlCondition _firstLogicalJoinOrComparison = null;
		public SPCamlCondition FirstLogicalJoinOrComparison {
			get { return _firstLogicalJoinOrComparison; }
			internal set { _firstLogicalJoinOrComparison = value; }
		}

		private SPCamlCondition _secondLogicalJoinOrComparison = null;
		public SPCamlCondition SecondLogicalJoinOrComparison {
			get { return _secondLogicalJoinOrComparison; }
			internal set { _secondLogicalJoinOrComparison = value; }
		}

		public SPCamlLogical(SPCamlLogicalOperatorType type, SPCamlCondition firstLogicalJoinOrComparison, SPCamlCondition secondLogicalJoinOrComparison) {
			LogicalJoinType = type;
			FirstLogicalJoinOrComparison = firstLogicalJoinOrComparison;
			SecondLogicalJoinOrComparison = secondLogicalJoinOrComparison;
		}
	}

}
