﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Task1
{
    public class ExpressionTreeTransformator : ExpressionVisitor
    {
        private readonly Dictionary<string, int> _mapperList;
        public ExpressionTreeTransformator(Dictionary<string, int> mapperList)
        {
            _mapperList = mapperList;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var parameters = node.Parameters
                                 .Where(p => _mapperList.ContainsKey(p.Name));

            return Expression.Lambda<T>(Visit(node.Body), parameters);
        }


        protected override Expression VisitParameter(ParameterExpression node)
        {
            var constantToReplace = _mapperList.FirstOrDefault(m => m.Key == node.Name).Value;
            return Expression.Constant(constantToReplace);
        }


        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Subtract || node.NodeType == ExpressionType.Add)
            {
                ParameterExpression param = null;
                ConstantExpression constant = null;

                if (node.Left.NodeType == ExpressionType.Parameter)
                {
                    param = (ParameterExpression)node.Left;
                }
                else if (node.Left.NodeType == ExpressionType.Constant)
                {
                    constant = (ConstantExpression)node.Left;
                }

                if (node.Right.NodeType == ExpressionType.Parameter)
                {
                    param = (ParameterExpression)node.Right;
                }
                else if (node.Right.NodeType == ExpressionType.Constant)
                {
                    constant = (ConstantExpression)node.Right;
                }

                if (param != null && constant != null && constant.Type == typeof(int) && (int)constant.Value == 1)
                {
                    if (node.NodeType == ExpressionType.Add)
                    {
                        return Expression.Increment(param);
                    }

                    if (node.NodeType == ExpressionType.Subtract)
                    {
                        return Expression.Decrement(param);
                    }
                }
            }

            return base.VisitBinary(node);
        }


    }



}
