using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace mrs.Application.Common.ExpressionExtension
{
    public static class OrderByExtension
    {
        public static IQueryable<TEntity> OrderByCustom<TEntity>(this IQueryable<TEntity> source, string sortExpression) where TEntity : class
        {
            if (string.IsNullOrEmpty(sortExpression))
                return source;

            var entityType = typeof(TEntity);
            string ascSortMethodName = "OrderBy";
            string descSortMethodName = "OrderByDescending";
            string[] sortExpressionParts = sortExpression.Split(' ');
            string sortProperty = sortExpressionParts[0];
            string sortMethod = ascSortMethodName;

            if (sortExpressionParts.Length > 1 && sortExpressionParts[1] == "DESC")
                sortMethod = descSortMethodName;

            var property = entityType.GetProperty(sortProperty);
            if (property == null) return source;

            var parameter = Expression.Parameter(entityType, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            MethodCallExpression resultExp = Expression.Call(
                                                typeof(Queryable),
                                                sortMethod,
                                                new Type[] { entityType, property.PropertyType },
                                                source.Expression,
                                                Expression.Quote(orderByExp));

            return source.Provider.CreateQuery<TEntity>(resultExp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortExpression">Format "PropertyName ASC" or "PropertyName DESC"</param>
        /// <returns></returns>
        public static IEnumerable<TEntity> OrderByCustom<TEntity>(this IEnumerable<TEntity> source, params string[] sortExpression) where TEntity : class
        {
            if (sortExpression == null || !sortExpression.Any()) return source;

            var entityType = typeof(TEntity);

            IOrderedEnumerable<TEntity> sorted = null;
            foreach (string sortString in sortExpression)
            {
                if (string.IsNullOrWhiteSpace(sortString)) continue;

                string[] sortExpressionParts = sortString.Split(' ');
                string sortProperty = sortExpressionParts[0];
                bool isDescending = false;

                if (sortExpressionParts.Length > 1 && sortExpressionParts[1] == "DESC")
                    isDescending = true;

                var property = entityType.GetProperty(sortProperty);
                if (property == null) return source;

                if (isDescending)
                {
                    sorted = (sorted == null) ? source.OrderByDescending(x => property.GetValue(x, null)) : sorted.ThenByDescending(x => property.GetValue(x, null));
                }
                else
                {
                    sorted = (sorted == null) ? source.OrderBy(x => property.GetValue(x, null)) : sorted.ThenBy(x => property.GetValue(x, null));
                }
            }

            return sorted;
        }
    }
}
