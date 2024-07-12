using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Para.Business.Helpers
{
    public static class QueryHelper
    {
        // Dinamik sorgu oluşturmak için kullanılan yardımcı metod. çalışma mantığı sorgu sırasında propertName ile FirstName , LastName , MontlyIncome gibi where sorgusu yapacağımız koşulu yazıyoruz
        // comparison ile kullanacağımız koşulu yazıyoruz eq Equal yada gt GreaterThan gibi 
        // value ile sorgu verimizi yazıyoruz. örn. FirstName ise "Ferit" gibi .
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
                    throw new NotSupportedException($"Girdiğiniz karşılaştırma operatörü : {comparison} desteklenmemektedir. Lütfen uygun sorgu seçeneklerini kullanınız. ( eq,neq,gt,gte,lt,lte,contains )");
            }

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
