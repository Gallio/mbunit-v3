using System;
using System.Reflection;
using Castle.Core.Interceptor;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// Holds information about an intercepted method.
    /// </summary>
    /// <remarks>
    /// This type is not intended to be used directly by clients.
    /// </remarks>
    public sealed class StaticInterceptorStub
    {
        private static readonly object syncRoot = new object();

        private readonly MethodInfo method;
        private readonly IInterceptor[] interceptors;
        private readonly MethodInfo methodInvocationTarget;

        private StaticInterceptorStub(MethodInfo method, IInterceptor[] interceptors)
            : this(method, interceptors, GetTargetMethod(method))
        {
        }

        private StaticInterceptorStub(MethodInfo method, IInterceptor[] interceptors,
            MethodInfo methodInvocationTarget)
        {
            this.method = method;
            this.interceptors = interceptors;
            this.methodInvocationTarget = methodInvocationTarget;
        }

        /// <summary>
        /// Gets the intercepted method.
        /// </summary>
        public MethodInfo Method
        {
            get { return method; }
        }

        /// <summary>
        /// Gets the target method to call on Proceed.
        /// </summary>
        public MethodInfo MethodInvocationTarget
        {
            get { return methodInvocationTarget; }
        }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        public Type TargetType
        {
            get { return method.DeclaringType; }
        }

        /// <summary>
        /// Gets the array of interceptors.
        /// </summary>
        public IInterceptor[] Interceptors
        {
            get { return interceptors; }
        }

        /// <summary>
        /// Adds an interceptor.
        /// </summary>
        /// <param name="method">The method to intercept</param>
        /// <param name="interceptor">The interceptor to add</param>
        internal static void AddInterceptor(MethodInfo method, IInterceptor interceptor)
        {
            lock (syncRoot)
            {
                FieldInfo stubField = GetStubField(method);
                StaticInterceptorStub stub = (StaticInterceptorStub)stubField.GetValue(null);

                IInterceptor[] newInterceptors;
                if (stub == null)
                {
                    newInterceptors = new IInterceptor[] { interceptor };
                    stub = new StaticInterceptorStub(method, newInterceptors);
                }
                else
                {
                    newInterceptors = stub.interceptors;
                    Array.Resize(ref newInterceptors, newInterceptors.Length + 1);
                    newInterceptors[newInterceptors.Length - 1] = interceptor;
                    stub = new StaticInterceptorStub(method, newInterceptors, stub.methodInvocationTarget);
                }

                stubField.SetValue(null, stub);
            }
        }

        /// <summary>
        /// Removes an interceptor.
        /// </summary>
        /// <param name="method">The method to intercept</param>
        /// <param name="interceptor">The interceptor to remove</param>
        /// <returns>True if an interceptor was removed</returns>
        internal static bool RemoveInterceptor(MethodInfo method, IInterceptor interceptor)
        {
            lock (syncRoot)
            {
                FieldInfo stubField = GetStubField(method);
                StaticInterceptorStub stub = (StaticInterceptorStub)stubField.GetValue(null);

                if (stub == null)
                    return false;

                IInterceptor[] oldInterceptors = stub.interceptors;
                int index = Array.IndexOf(oldInterceptors, interceptor);
                if (index < 0)
                    return false;

                if (oldInterceptors.Length == 1)
                {
                    stub = null;
                }
                else
                {
                    IInterceptor[] newInterceptors = new IInterceptor[oldInterceptors.Length - 1];

                    int remainder = newInterceptors.Length - index;
                    if (index != 0)
                        Array.Copy(oldInterceptors, newInterceptors, index);
                    if (remainder != 0)
                        Array.Copy(oldInterceptors, index + 1, newInterceptors, index, remainder);

                    stub = new StaticInterceptorStub(method, newInterceptors, stub.methodInvocationTarget);
                }

                stubField.SetValue(null, stub);
                return true;
            }
        }

        internal static FieldInfo GetStubField(MethodInfo method)
        {
            string stubFieldName = GetStubFieldName(method.Name);
            FieldInfo stubField = method.DeclaringType.GetField(stubFieldName, BindingFlags.Static | BindingFlags.NonPublic);

            if (stubField == null)
                ThrowNotInterceptable(method);

            return stubField;
        }

        internal static MethodInfo GetTargetMethod(MethodInfo method)
        {
            string targetMethodName = GetTargetMethodName(method.Name);
            MethodInfo targetMethod = method.DeclaringType.GetMethod(targetMethodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);

            if (targetMethod == null)
                ThrowNotInterceptable(method);

            return targetMethod;
        }

        internal static void ThrowNotInterceptable(MethodInfo method)
        {
            throw new InvalidOperationException(string.Format("Method '{0}.{1}' cannot be intercepted.",
                method.DeclaringType.FullName, method.Name));
        }

        internal static string GetTargetMethodName(string methodName)
        {
            return @"<InterceptorTarget>_" + methodName;
        }

        internal static string GetStubFieldName(string methodName)
        {
            return @"<InterceptorStub>_" + methodName;
        }
    }
}
