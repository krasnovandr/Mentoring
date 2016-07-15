using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Task3
{
    public class ExpressionToFTSRequestTranslator : ExpressionVisitor
    {
        StringBuilder resultString;

        public string Translate(Expression exp)
        {
            resultString = new StringBuilder();
            Visit(exp);

            return resultString.ToString();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            if (node.Method.Name == "StartsWith")
            {
                Visit(node.Object);
                resultString.Append("(");
                var predicate = node.Arguments[0];
                resultString.Append("*");
                Visit(predicate);
      
                resultString.Append(")");
                return node;
            }

            if (node.Method.Name == "Contains")
            {
                Visit(node.Object);
                resultString.Append("(");
                var predicate = node.Arguments[0];
                resultString.Append("*");
                Visit(predicate);
                resultString.Append("*");
                resultString.Append(")");
                return node;
            }

            if (node.Method.Name == "EndsWith")
            {
                Visit(node.Object);
                resultString.Append("(");
                var predicate = node.Arguments[0];
                resultString.Append("*");
                Visit(predicate);
                resultString.Append(")");
                return node;
            }



            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    ProcessBinary(node);

                    break;
                case ExpressionType.AndAlso:
                    ProcessBinary((BinaryExpression)node.Left);
                    resultString.Append(" AND ");
                    ProcessBinary((BinaryExpression)node.Right);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
            };

            return node;
        }

        private void ProcessBinary(BinaryExpression node)
        {
            if ((node.Left.NodeType == ExpressionType.MemberAccess
                 && node.Right.NodeType == ExpressionType.Constant))
            {
                Visit(node.Left);
                resultString.Append("(");
                Visit(node.Right);
                resultString.Append(")");
            }


            if ((node.Left.NodeType == ExpressionType.Constant
                 && node.Right.NodeType == ExpressionType.MemberAccess))
            {
                Visit(node.Right);
                resultString.Append("(");
                Visit(node.Left);
                resultString.Append(")");
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            resultString.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            resultString.Append(node.Value);

            return node;
        }
    }
}
