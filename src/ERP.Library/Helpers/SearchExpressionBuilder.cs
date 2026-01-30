using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ERP.Library.Attributes;

namespace ERP.Library.Helpers
{
    
    /// <summary>
    /// 根據傳入的搜尋 ViewModel 自動生成對應 Entity 的 LINQ 過濾條件 (Expression)。
    /// </summary>
    /// --------------------------------------
    /// <remarks>
    /// 支援的型別與行為：
    /// - string: 使用 Contains 進行模糊比對
    /// - int / decimal / enum: 使用等於比較
    /// - DateTime: 可區間查詢
    /// - 導覽屬性: 支援 "A.B.C" 路徑格式
    /// </remarks>
    public static class SearchExpressionBuilder
    {
        public static Expression<Func<TEntity, bool>> Build<TEntity>(object searchVm)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression body = Expression.Constant(true);

            foreach (var prop in searchVm.GetType().GetProperties())
            {
                // 沒有值 => 跳過
                var value = prop.GetValue(searchVm);
                if (IsEmpty(value)) continue;

                // 欄位沒有加Attribute => 跳過
                var attr = prop.GetCustomAttribute<SearchFieldAttribute>();
                if (attr == null) continue;

                //組合 => ex: x.Name
                var member = BuildMemberExpression(parameter, attr.EntityProperty);
                //x.Name.Contains("白粉")
                var condition = BuildCondition(member, value!, attr.Compare);

                body = Expression.AndAlso(body, condition);
            }

            return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
        }

        private static bool IsEmpty(object? value)
        {
            if (value == null) return true;
            if (value is string s) return string.IsNullOrWhiteSpace(s);
            return false;
        }

        private static MemberExpression BuildMemberExpression(
            Expression parameter,
            string propertyPath)
        {
            Expression current = parameter;
            foreach (var prop in propertyPath.Split('.'))
            {
                current = Expression.Property(current, prop);
            }
            return (MemberExpression)current;
        }
        /// <summary>
        /// 建立對應的查詢條件 Expression
        /// </summary>
        /// <param name="member">比較的屬性成員（x.Name）</param>
        /// <param name="value">該屬性的值</param>
        /// <param name="compare">指較方式（等於、包含、大於等於、小於等於）</param>
        /// <returns></returns>
        private static Expression BuildCondition(
            MemberExpression member,
            object value,
            SearchCompare compare)
        {
            // 將C# 值轉成 Expression Tree 可理解的「常數節點」
            var constant = Expression.Constant(value);

            if (value is DateTime dt && compare == SearchCompare.LessThanOrEqual )
            {
                // 將 End 日期調整到當天最後一刻
                constant = Expression.Constant(dt.Date.AddDays(1).AddTicks(-1));
            }

            return compare switch
            {
                SearchCompare.Contains =>
                    Expression.Call(
                        member,
                        typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                        constant),

                SearchCompare.GreaterThanOrEqual =>
                    Expression.GreaterThanOrEqual(member, constant),

                SearchCompare.LessThanOrEqual =>
                    Expression.LessThanOrEqual(member, constant),

                _ => Expression.Equal(member, constant)
            };
        }
    }
}
