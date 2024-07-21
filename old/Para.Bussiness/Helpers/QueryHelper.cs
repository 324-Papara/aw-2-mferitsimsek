using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Para.Bussiness.Helpers
{
    public static class QueryHelper
    {
        // Dinamik sorgu oluşturmak için kullanılan yardımcı metod
        public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var left = Expression.Property(parameter, propertyName);
            var right = Expression.Constant(value);

            Expression body;
            switch (comparison.ToLower())
            {
                // Eşitlik kontrolü
                case "eq":
                    body = Expression.Equal(left, right);
                    break;
                // Eşit değil kontrolü
                case "neq":
                    body = Expression.NotEqual(left, right);
                    break;
                // Büyüktür kontrolü
                case "gt":
                    body = Expression.GreaterThan(left, right);
                    break;
                // Büyük eşittir kontrolü
                case "gte":
                    body = Expression.GreaterThanOrEqual(left, right);
                    break;
                // Küçüktür kontrolü
                case "lt":
                    body = Expression.LessThan(left, right);
                    break;
                // Küçük eşittir kontrolü
                case "lte":
                    body = Expression.LessThanOrEqual(left, right);
                    break;
                // İçerir kontrolü
                case "contains":
                    body = Expression.Call(left, typeof(string).GetMethod("Contains", new[] { typeof(string) }), right);
                    break;
                // Desteklenmeyen karşılaştırma operatörü
                default:
                    throw new NotSupportedException($"Comparison {comparison} is not supported.");
            }

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
