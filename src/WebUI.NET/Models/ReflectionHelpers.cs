using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WebUI.Models
{
    public static class ReflectionHelpers
    {
        /// <summary>Given a lambda expression that calls a method, returns the method info</summary>
		/// <param name="expression">The lambda expression using the method</param>
		/// <returns>The method in the lambda expression</returns>
		///
		public static MethodInfo GetMethodInfo(LambdaExpression expression, out object instance)
        {
            instance = null;
            var outermostExpression = expression.Body as MethodCallExpression;
            if (outermostExpression != null)
            {
                Expression<Func<Object>> getCallerExpression = Expression<Func<Object>>.Lambda<Func<Object>>(outermostExpression.Object);
                instance = getCallerExpression.Compile()();//need to verify this working

            }
            if (outermostExpression is null)
            {
                if (expression.Body is UnaryExpression ue && ue.Operand is MethodCallExpression me && me.Object is System.Linq.Expressions.ConstantExpression ce && ce.Value is MethodInfo mi)
                {
                    if (me.Arguments.Count > 1)
                    {
                        var inst = me.Arguments.ElementAt(1);
                        if (inst is ConstantExpression ce2)
                            instance = ce2.Value;

                    }
                    return mi;
                }
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            var method = outermostExpression.Method;
            if (method is null)
                throw new Exception($"Cannot find method for expression {expression}");

            return method;
        }

        public static (object[] defaultVals, Type[] argTypes) GetArgsForMethod(MethodInfo method)
        {
            var infoArgs = method.GetParameters();
            var defaultCallArgs = new object[infoArgs.Length];
            var callArgTypes = infoArgs.Select(a => a.ParameterType).ToArray();
            for (var x = 0; x < infoArgs.Length; x++)
            {
                var infoArg = infoArgs[x];
                if (infoArg.HasDefaultValue)
                    defaultCallArgs[x] = infoArg.DefaultValue;
                else
                {
                    if (!infoArg.ParameterType.IsValueType)
                        defaultCallArgs[x] = null;
                    else
                        defaultCallArgs[x] = Activator.CreateInstance(infoArg.ParameterType);
                }
            }
            return (defaultCallArgs, callArgTypes);
        }
    }
}
