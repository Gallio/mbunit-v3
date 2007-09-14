// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// The invocation type used by <see cref="StaticInstrumentor" />.
    /// </summary>
    /// <remarks>
    /// This type is not intended to be used directly by clients.
    /// </remarks>
    public sealed class StaticInvocation : IInvocation
    {
        private readonly StaticInterceptorStub interceptorStub;
        private readonly object invocationTarget;
        private readonly object[] arguments;
        private object returnValue;

        private int interceptorIndex;

        /// <summary>
        /// An empty argument array.
        /// </summary>
        public static readonly object[] NoArguments = new object[0];

        /// <summary>
        /// Creates an invocation.
        /// </summary>
        /// <param name="interceptorStub">The stub</param>
        /// <param name="invocationTarget">The invocation target or null if intercepting a static method</param>
        /// <param name="arguments">The arguments</param>
        public StaticInvocation(StaticInterceptorStub interceptorStub, object invocationTarget, object[] arguments)
        {
            this.interceptorStub = interceptorStub;
            this.invocationTarget = invocationTarget;
            this.arguments = arguments;
        }

        /// <inheritdoc />
        public void SetArgumentValue(int index, object value)
        {
            arguments[index] = value;
        }

        /// <inheritdoc />
        public object GetArgumentValue(int index)
        {
            return arguments[index];
        }

        /// <inheritdoc />
        public void Proceed()
        {
            if (interceptorIndex == interceptorStub.Interceptors.Length)
            {
                InvokeMethodOnTarget();
            }
            else
            {
                IInterceptor interceptor = interceptorStub.Interceptors[interceptorIndex++];
                interceptor.Intercept(this);
            }
        }

        /// <inheritdoc />
        public object Proxy
        {
            get { return invocationTarget; }
        }

        /// <inheritdoc />
        public object InvocationTarget
        {
            get { return invocationTarget; }
        }

        /// <inheritdoc />
        public Type TargetType
        {
            get { return interceptorStub.TargetType; }
        }

        /// <inheritdoc />
        public object[] Arguments
        {
            get { return arguments; }
        }

        /// <inheritdoc />
        public MethodInfo Method
        {
            get { return interceptorStub.Method; }
        }

        /// <inheritdoc />
        public MethodInfo MethodInvocationTarget
        {
            get { return interceptorStub.MethodInvocationTarget; }
        }

        /// <inheritdoc />
        public object ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }

        /// <summary>
        /// Applies all interceptors to the invocation.
        /// </summary>
        /// <returns>The return value</returns>
        public object Execute()
        {
            Proceed();
            return returnValue;
        }

        private void InvokeMethodOnTarget()
        {
            returnValue = interceptorStub.MethodInvocationTarget.Invoke(invocationTarget, arguments);
        }
    }
}
