// MIT License
// Copyright Eraware

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Eraware.Modules.MyModule
{
    /// <summary>
    /// Provides various entension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Checks if the query contains any sorting expressions
        /// such as OrderBy, OrderByDescending, ThenBy or ThenByDescending.
        /// </summary>
        /// <typeparam name="T">The type of entities in the query.</typeparam>
        /// <param name="queryable">The original queryable.</param>
        /// <returns>A value indicating whether the query is already ordered.</returns>
        internal static bool IsOrdered<T>(this IQueryable<T> queryable)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException("queryable");
            }

            return OrderingMethodFinder.OrderMethodExists(queryable.Expression);
        }

        private class OrderingMethodFinder : ExpressionVisitor
        {
            private bool orderingMethodFound = false;

            internal static bool OrderMethodExists(Expression expression)
            {
                var visitor = new OrderingMethodFinder();
                visitor.Visit(expression);
                return visitor.orderingMethodFound;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var name = node.Method.Name;

                if (node.Method.DeclaringType == typeof(Queryable) && (
                    name.StartsWith("OrderBy", StringComparison.Ordinal) ||
                    name.StartsWith("ThenBy", StringComparison.Ordinal)))
                {
                    this.orderingMethodFound = true;
                }

                return base.VisitMethodCall(node);
            }
        }
    }
}