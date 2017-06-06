using System;

namespace UruIT.Utilities
{
    /// <summary>
    /// Plain representation of a LambdaExpression
    /// </summary>
    public class FlatExpression
    {
        /// <summary>
        /// Name of the expression's field.
        /// </summary>
        public string ExpFieldName { get; set; }

        /// <summary>
        /// Type of the expression.
        /// </summary>
        public Type ExpFieldType { get; set; }

        /// <summary>
        /// Type of the object that declares the type.
        /// </summary>
        public Type ExpFieldDeclaringType { get; set; }
    }
}

namespace System.Linq.Expressions
{
    using System.Reflection;
    using UruIT.Utilities;

    public static class ExpressionUtilities
    {
        /// <summary>
        /// Flattens a member access expression by obtaining the name of the field, its type and the declaring type.
        /// </summary>
        private static FlatExpression Flatten(this MemberExpression exp, Type declaringType)
        {
            var propInfo = exp.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException("The member should be a property", "exp");

            return new FlatExpression()
            {
                ExpFieldName = propInfo.Name,
                ExpFieldType = propInfo.PropertyType,
                ExpFieldDeclaringType = declaringType
            };
        }

        /// <summary>
        /// Flattens a lambda expression by obtaining the name of the field, its type and the declaring type.
        /// </summary>
        public static FlatExpression Flatten(this LambdaExpression exp)
        {
            var memExp = exp.Body as MemberExpression;
            if (memExp == null)
                throw new ArgumentException("The expression should be a member accesor", "exp");

            // Since it's a MemberExpression, it contains a parameter
            return memExp.Flatten(exp.Parameters[0].Type);
        }

        /// <summary>
        /// Obtains a lambda expression of the form "x => x.ExpFieldName" given its flattened form.
        /// </summary>
        public static LambdaExpression Unflatten(this FlatExpression flatExp)
        {
            // We create a single parameter "x" expression
            var paramExp = Expression.Parameter(flatExp.ExpFieldDeclaringType, "x");

            // We obtain the property of the original object, and we create an expression of form "x.ExpFieldName"
            var property = flatExp.ExpFieldDeclaringType.GetProperty(flatExp.ExpFieldName).DeclaringType.GetProperty(flatExp.ExpFieldName);
            var memberExp = Expression.MakeMemberAccess(paramExp, property);

            // We create a function Func<ObjectType, PropertyType> to be used in the lambda expression
            var funcType = typeof(Func<,>).MakeGenericType(new Type[] { flatExp.ExpFieldDeclaringType, flatExp.ExpFieldType });

            // The final lambda expression is created
            return Expression.Lambda(funcType, memberExp, paramExp);
        }
    }
}