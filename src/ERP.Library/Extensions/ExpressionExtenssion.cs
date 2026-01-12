using System.Linq.Expressions;
using System.Reflection;
using ERP.Library.Attributes;

namespace ERP.Library.Extensions
{
    public static class ExpressionExtenssion
    {
        /// <summary>
        /// 將兩個表達式以AND串聯為一個表達式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">表達式1</param>
        /// <param name="expr2">表達式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ExpressionAnd<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(
                expr2.Parameters[0], expr1.Parameters[0]);

            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, secondBody), expr1.Parameters);
        }
        /// <summary>
        /// 將兩個表達式以or串聯為一個表達式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">表達式1</param>
        /// <param name="expr2">表達式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ExpressionOr<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(
                expr2.Parameters[0], expr1.Parameters[0]);

            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, secondBody), expr1.Parameters);
        }

        public static Expression? Replace(this Expression? expression, Expression searchEx, Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }


        /// <summary>
        /// 轉換表達式
        /// </summary>
        /// <typeparam name="TargetT">目標類型 泛型</typeparam>
        /// <param name="expr">待轉換表達式</param>
        /// <returns></returns>
        public static Expression<Func<TargetT, bool>> ConvertToExpression<TargetT>(this LambdaExpression expr)
        {
            var subType = expr.Parameters[0].Type;
            var parentType = typeof(TargetT);
            ParameterExpression parameter = Expression.Parameter(parentType, "e");
            var targetPropertyName = parentType.GetProperties().FirstOrDefault(x => x.PropertyType == subType)?.Name;
            Expression? property = null;
            if (targetPropertyName != null)
            {
                property = Expression.Property(parameter, targetPropertyName);
                // 如果屬性不是集合，則使用直接比較的方式
                var body = Expression.Invoke(expr, property);
                return Expression.Lambda<Func<TargetT, bool>>(body, parameter);
            }

            var Properties = parentType.GetProperties();
            foreach (var item in Properties)
            {
                if (item.PropertyType.GenericTypeArguments != null
                    && item.PropertyType.GenericTypeArguments.Any()
                    && item.PropertyType.GenericTypeArguments[0] == subType)
                {
                    targetPropertyName = item.Name;
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(targetPropertyName))
            {
                throw new InvalidOperationException($"No matching property found on type '{parentType}' for subtype '{subType}'.");
            }

            property = Expression.Property(parameter, targetPropertyName);
            var anyMethod = typeof(Enumerable).GetMethods()
                .Where(m => m.Name == "Any" && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(subType);
            var call = Expression.Call(anyMethod, property, expr);
            return Expression.Lambda<Func<TargetT, bool>>(call, parameter);
        }

        /// <summary>
        /// 轉換表達式
        /// </summary>
        /// <typeparam name="TargetT">目標類型 泛型</typeparam>
        /// <param name="expr">待轉換表達式</param>
        /// <param name="func">指定 目標泛型之 待轉換型別 屬性名稱</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Expression<Func<TargetT, bool>> ConvertToExpression<TargetT>(this LambdaExpression expr, Expression<Func<TargetT, object>> func)
        {
            var subType = typeof(object);

            if (func.Body is UnaryExpression unaryExpression)
            {
                // 處理轉型操作，例如 T => (object)T.SomeProperty
                subType = unaryExpression.Operand.Type;
            }
            else if (func.Body is MemberExpression memberExpression)
            {
                // 直接存取屬性，例如 T => T.SomeProperty
                subType = memberExpression.Type;
            }
            else if (func.Body is MethodCallExpression methodCallExpression)
            {
                // 處理方法呼叫，例如 T => T.SomeMethod()
                subType = methodCallExpression.Method.ReturnType;
            }

            var targetType = typeof(TargetT);
            var targetPropertyName = targetType.GetProperties().FirstOrDefault(x => x.PropertyType == subType)?.Name;
            if (string.IsNullOrWhiteSpace(targetPropertyName))
            {
                throw new InvalidOperationException($"No matching property found on type '{targetType}' for subtype '{subType}'.");
            }

            ParameterExpression parameter = Expression.Parameter(targetType, "e");
            Expression? property = null;

            if (subType.GenericTypeArguments != null
                && subType.GenericTypeArguments.Any())
            {
                property = Expression.Property(parameter, targetPropertyName);
                var anyMethod = typeof(Enumerable).GetMethods()
                    .Where(m => m.Name == "Any" && m.GetParameters().Length == 2)
                    .Single()
                    .MakeGenericMethod(subType.GenericTypeArguments[0]);
                var call = Expression.Call(anyMethod, property, expr);
                return Expression.Lambda<Func<TargetT, bool>>(call, parameter);
            }
            else
            {
                property = Expression.Property(parameter, targetPropertyName);
                // 如果屬性不是集合，則使用直接比較的方式
                var body = Expression.Invoke(expr, property);
                return Expression.Lambda<Func<TargetT, bool>>(body, parameter);
            }
        }

        /// <summary>
        /// 建立屬性表達式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        public static Expression BuildNestedPropertyExpression<T>(this Expression param, string propertyPath)
        {
            var properties = propertyPath.Split('.');
            Expression currentExpression = param;

            foreach (var property in properties)
            {
                currentExpression = Expression.Property(currentExpression, property);
            }

            return currentExpression;
        }
        /// <summary>
        /// 取得屬性名稱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetPropertyName<T>(this Expression<Func<T, object?>> expression)
        {
            // 處理 UnaryExpression（例如 x => x.SignOffDocument.DocumentNo）
            if (expression.Body is UnaryExpression unary)
            {
                expression = Expression.Lambda<Func<T, object>>(unary.Operand, expression.Parameters);
            }

            // 確保 body 是 MemberExpression
            if (expression.Body is MemberExpression memberExpression)
            {
                return GetMemberName(memberExpression);
            }

            throw new ArgumentException("Expression is not a member access", nameof(expression));
        }
        /// <summary>
        /// 取得成員名稱
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        private static string GetMemberName(MemberExpression memberExpression)
        {
            // 遞迴處理多層嵌套的屬性
            if (memberExpression.Expression is MemberExpression parentMember)
            {
                return GetMemberName(parentMember) + "." + memberExpression.Member.Name;
            }

            return memberExpression.Member.Name;
        }

        internal class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression from, to;
            public ReplaceVisitor(Expression from, Expression to)
            {
                this.from = from;
                this.to = to;
            }
            public override Expression? Visit(Expression? node)
            {
                return node == from ? to : base.Visit(node);
            }
        }
        internal class ParameterReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression _oldParameter;
            private readonly Expression _newParameter;

            public ParameterReplaceVisitor(Expression oldParameter, Expression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _oldParameter)
                    return _newParameter;
                return base.VisitParameter(node);
            }
        }
    }
}
