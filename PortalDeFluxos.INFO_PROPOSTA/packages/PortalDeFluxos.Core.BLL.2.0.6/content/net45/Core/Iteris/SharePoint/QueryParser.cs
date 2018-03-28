using PortalDeFluxos.Core.BLL.Atributos;
using System;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace PortalDeFluxos.Core.BLL.Iteris.SharePoint
{
    public class QueryParser
    {
        private XElement query = new XElement("Where");

        public string Parse(Expression expression, int rowLimit = 0)
        {
            query.Add(Visit(expression));
            String retorno = String.Empty;

            if (rowLimit == 0)
                retorno = String.Concat("<View><Query>", query.ToString(SaveOptions.DisableFormatting), "</Query></View>");
            retorno = String.Concat("<View><Query>", query.ToString(SaveOptions.DisableFormatting), String.Format("</Query><RowLimit>{0}</RowLimit></View>", rowLimit));

            return retorno.Replace("true", "1").Replace("false", "0");
        }

        protected XElement VisitBinary(BinaryExpression binary)
        {
            XElement node = ParseNodeType(binary.NodeType);
            XElement left = Visit(binary.Left);
            XElement right = Visit(binary.Right);

			if ((binary.Right as ConstantExpression) != null && 
				(binary.Right as ConstantExpression).Value == null && left != null
				&& (binary.NodeType == ExpressionType.Equal || binary.NodeType == ExpressionType.NotEqual))
			{
				node = binary.NodeType == ExpressionType.Equal ? new XElement("IsNull") : new XElement("IsNotNull");
				node.Add(left);

			}else if (left != null && right != null)
                node.Add(left, right);

            return node;
        }

        protected XElement Visit(Expression expression)
        {
            if (expression == null)
                return null;

            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    return VisitMethodCall(expression as MethodCallExpression);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess(expression as MemberExpression);
                case ExpressionType.Constant:
                    return VisitConstant(expression as ConstantExpression);
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return VisitBinary(expression as BinaryExpression);
                default:
                    return null;
            }
        }

        protected XElement ParseNodeType(ExpressionType type)
        {
            XElement node;

            switch (type)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    node = new XElement("And");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    node = new XElement("Or");
                    break;
                case ExpressionType.Equal:
                    node = new XElement("Eq");
                    break;
                case ExpressionType.GreaterThan:
                    node = new XElement("Gt");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    node = new XElement("Geq");
                    break;
                case ExpressionType.LessThan:
                    node = new XElement("Lt");
                    break;
                case ExpressionType.LessThanOrEqual:
                    node = new XElement("Leq");
                    break;
				case ExpressionType.NotEqual:
					node = new XElement("Neq");
					break;
                default:
                    throw new Exception(string.Format("Comparação ainda não implementada: '{0}'", type));
            }
            return node;
        }

        protected XElement VisitMethodCall(MethodCallExpression methodcall)
        {
            XElement node;
            XElement left = Visit(methodcall.Object);
            XElement right = Visit(methodcall.Arguments[0]);

            switch (methodcall.Method.Name)
            {
                case "Contains":
                    node = new XElement("Contains");
                    break;
                case "StartsWith":
                    node = new XElement("BeginsWith");
                    break;
                default:
                    throw new Exception(string.Format("Tipo ainda não implementado: '{0}'", methodcall.Method.Name));
            }

            if (left != null && right != null)
                node.Add(left, right);

            return node;

        }

        protected XElement VisitConstant(ConstantExpression constant)
        {
            return new XElement("Value", ParseValueType(constant.Type), constant.Value);
        }

        protected XElement VisitMemberAccess(MemberExpression member)
        {

            var expr = member.Expression;
            if (expr.NodeType == ExpressionType.Constant)
            {
                LambdaExpression lambda = Expression.Lambda(member);
                Delegate fn = lambda.Compile();
                return VisitConstant(Expression.Constant(fn.DynamicInvoke(null), member.Type));

            }
            else
            {
                //Busca pelo internal name do campo
                object[] internalName = member.Member.GetCustomAttributes(typeof(InternalNameAttribute), true);
                if (internalName == null || internalName.Length == 0)
                    return new XElement("FieldRef", new XAttribute("Name", member.Member.Name));

                return new XElement("FieldRef", new XAttribute("Name", ((InternalNameAttribute)internalName.GetValue(0)).Name));
            }
        }

        protected XAttribute ParseValueType(Type type)
        {
            string name = "Text";

            switch (type.Name)
            {
                case "String":
                    name = "Text";
                    break;
                case "Int64":
                case "Int32":
                case "Int16":
                    name = "Number";
                    break;
                default:
                    name = type.Name;
                    break;
            }
            return new XAttribute("Type", name);
        }
    }
}
