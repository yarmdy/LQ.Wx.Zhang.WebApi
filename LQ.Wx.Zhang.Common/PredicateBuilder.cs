using System;
using System.CodeDom;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LQ.Wx.Zhang.Common
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            Expression right = new ExParameterVisitor(expr2.Parameters[0], expr1.Parameters[0]).Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, right), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            Expression right = new ExParameterVisitor(expr2.Parameters[0], expr1.Parameters[0]).Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, right), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> Equal<T>(this LambdaExpression expr,object? val) {
            return Expression.Lambda<Func<T, bool>>(Expression.Equal(expr.Body,Expression.Constant(val)),expr.Parameters);
        }
        public static Expression<Func<T, bool>> Equal<T>(this LambdaExpression expr, object? val,Type type)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Equal(expr.Body, Expression.Constant(val, type)), expr.Parameters);
        }

        public static Expression<Func<T, bool>> In<T, TType>(this Expression<Func<T, TType>> expr, TType[] vals)
        {
            var method = typeof(System.Linq.Enumerable).GetMethods().Where(a => a.Name == "Contains" && a.GetParameters().Length == 2).FirstOrDefault()!.MakeGenericMethod(typeof(TType));
            return Expression.Lambda<Func<T, bool>>(Expression.Call(null, method, Expression.Constant(vals), expr.Body),expr.Parameters);
        }
        public static Expression<Func<T, bool>> Like<T>(this Expression<Func<T, string>> expr, string val)
        {
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string)})!;
            return Expression.Lambda<Func<T, bool>>(Expression.Call(expr.Body, method, Expression.Constant(val)), expr.Parameters);
        }

        public static Expression<Func<T, TResult>> DotExpression<T, TResult>(string name)
        {
            return (Expression<Func<T, TResult>>)DotExpressionBase<T>(name);
        }
        public static LambdaExpression DotExpressionBase<T>(string name)
        {
            if (!TypeHelper.HasProperty<T>(name))
            {
                throw new Exception($"属性\"{name}\"不存在");
            }
            var paramExpr = Expression.Parameter(typeof(T), "a");
            var arr = name.Split('.');
            Expression expr = paramExpr;
            Type type = typeof(T);
            foreach (var param in arr)
            {
                var member = TypeHelper.GetPropertyBase(type, param)!;
                expr = Expression.MakeMemberAccess(expr, member);
                type = member.PropertyType;
            }
            return Expression.Lambda(expr, paramExpr);
        }


        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string property,bool desc=false)
        {
            if (desc)
            {
                return ThenByDescending(query, property);
            }
            if (!TypeHelper.HasProperty<T>(property))
            {
                return query;
            }
            var tprop = TypeHelper.GetProperty<T>(property)!.PropertyType;
            var isnullable = tprop.FullName!.StartsWith("System.Nullable");
            var prop = isnullable ? tprop.GenericTypeArguments?[0] : tprop;
            if (prop == null)
            {
                return query;
            }
            if(!isnullable)
            {
                switch (prop.Name)
                {
                    case "String":
                        {
                            return query.ThenBy(DotExpression<T, string>(property));
                        }
                    case "Int32":
                        {
                            return query.ThenBy(DotExpression<T, int>(property));
                        }
                    case "DateTime":
                        {
                            return query.ThenBy(DotExpression<T, DateTime>(property));
                        }
                    case "Decimal":
                        {
                            return query.ThenBy(DotExpression<T, decimal>(property));
                        }
                    case "Boolean":
                        {
                            return query.ThenBy(DotExpression<T, bool>(property));
                        }

                }
            }
            else
            {
                switch (prop.Name)
                {
                    case "Int32":
                        {
                            return query.ThenBy(DotExpression<T, int?>(property));
                        }
                    case "DateTime":
                        {
                            return query.ThenBy(DotExpression<T, DateTime?>(property));
                        }
                    case "Decimal":
                        {
                            return query.ThenBy(DotExpression<T, decimal?>(property));
                        }
                    case "Boolean":
                        {
                            return query.ThenBy(DotExpression<T, bool?>(property));
                        }

                }
            }
            
            return query;
        }
        public static IQueryable<T> EqualTo<T>(this IQueryable<T> query,string property,object? value) {
            if (!TypeHelper.HasProperty<T>(property))
            {
                return query;
            }
            var tprop = TypeHelper.GetProperty<T>(property)!.PropertyType;
            if (tprop.IsGenericType && tprop.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var ttprop = tprop.GenericTypeArguments[0];
                if (value + "" == "")
                {
                    value = Activator.CreateInstance(tprop);
                }
                else
                {
                    var tmpval = Convert.ChangeType(value, ttprop, null);
                    value = tmpval; 
                }
                
            }
            else
            {
                value = Convert.ChangeType(value, tprop, null);
            }

            return query.Where(DotExpressionBase<T>(property).Equal<T>(value, tprop));
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> query, string property, bool asc = false)
        {
            if (asc)
            {
                return ThenBy(query,property);
            }
            if (!TypeHelper.HasProperty<T>(property))
            {
                return query;
            }
            var tprop = TypeHelper.GetProperty<T>(property)!.PropertyType;
            var isnullable = tprop.FullName!.StartsWith("System.Nullable");
            var prop = isnullable ? tprop.GenericTypeArguments?[0] : tprop;
            if (prop == null)
            {
                return query;
            }
            if (!isnullable)
            {
                switch (prop.Name)
                {
                    case "String":
                        {
                            return query.ThenByDescending(DotExpression<T, string>(property));
                        }
                    case "Int32":
                        {
                            return query.ThenByDescending(DotExpression<T, int>(property));
                        }
                    case "DateTime":
                        {
                            return query.ThenByDescending(DotExpression<T, DateTime>(property));
                        }
                    case "Decimal":
                        {
                            return query.ThenByDescending(DotExpression<T, decimal>(property));
                        }
                    case "Boolean":
                        {
                            return query.ThenByDescending(DotExpression<T, bool>(property));
                        }

                }
            }
            else
            {
                switch (prop.Name)
                {
                    case "Int32":
                        {
                            return query.ThenByDescending(DotExpression<T, int?>(property));
                        }
                    case "DateTime":
                        {
                            return query.ThenByDescending(DotExpression<T, DateTime?>(property));
                        }
                    case "Decimal":
                        {
                            return query.ThenByDescending(DotExpression<T, decimal?>(property));
                        }
                    case "Boolean":
                        {
                            return query.ThenByDescending(DotExpression<T, bool?>(property));
                        }

                }
            }

            return query;
        }

        public static PropertyInfo[] GetProperties<T,TResult>(this Expression<Func<T, TResult>> expr) {
            if (expr == null)
            {
                return new PropertyInfo[0];
            }
            if (expr.Body.NodeType != ExpressionType.New && expr.Body.NodeType != ExpressionType.MemberAccess)
            {
                return new PropertyInfo[0];
            }
            if (expr.Body.NodeType == ExpressionType.MemberAccess)
            {
                if(!((expr.Body as MemberExpression)!.Member is PropertyInfo)) { 
                    return new PropertyInfo[0];
                }
                return new[] { ((expr.Body as MemberExpression)!.Member as PropertyInfo)! };
            }
            return ((NewExpression)expr.Body).Arguments.Where(a => a.NodeType == ExpressionType.MemberAccess && (a as MemberExpression)!.Member is PropertyInfo).Select(a => ((a as MemberExpression)!.Member as PropertyInfo)).ToArray()!;
        }

        private class ExParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;

            private readonly ParameterExpression _newParameter;

            public ExParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _oldParameter)
                {
                    return _newParameter;
                }

                return base.VisitParameter(node);
            }
        }

        private static object getNullableValue(Type type,object value)
        {
            switch (type.Name)
            {
                
                case "Int32":
                    {
                        return (int?)value;
                    }
                case "DateTime":
                    {
                        return (DateTime?)value;
                    }
                case "Decimal":
                    {
                        return (decimal?)value;
                    }
                case "Boolean":
                    {
                        return (bool?)value;
                    }
            }
            return value;
        }
    }
}