using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace MiniTool.Util
{

    internal class SubstituteParameterVisitor : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _dic;

        private SubstituteParameterVisitor(Dictionary<ParameterExpression, ParameterExpression> dic)
        {
            _dic = dic ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }
        public static Expression Replace(Dictionary<ParameterExpression, ParameterExpression> dic, Expression exp)
        {
            return new  SubstituteParameterVisitor(dic).Visit(exp);
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            ParameterExpression replacement;
            if (_dic.TryGetValue(node, out replacement))
                node = replacement;
            return base.VisitParameter(node);
        }
    }

    public static class LinqExtension
    {

        private static Expression<T> CombineLambdas<T>(this Expression<T> first, Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = SubstituteParameterVisitor.Replace(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// 或连接
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="left">左条件</param>
        /// <param name="right">右条件</param>
        /// <returns>新表达式</returns>
       public static Expression<Func<T,bool>> Or<T>(this Expression<Func<T,bool>> left,Expression<Func<T,bool>> right)
        {
            return left.CombineLambdas(right, Expression.OrElse);
        }

       /// <summary>
       /// 与连接
       /// </summary>
       /// <typeparam name="T">类型</typeparam>
       /// <param name="left">左条件</param>
       /// <param name="right">右条件</param>
       /// <returns>新表达式</returns>
       public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
       {
           return left.CombineLambdas(right, Expression.AndAlso);
       }
    }
}
