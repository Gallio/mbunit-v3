using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Interceptor;

namespace MbUnit.Instrumentation
{
    /// <summary>
    /// An <see cref="IInterceptor" /> based on a <see cref="Interceptor" /> delegate.
    /// </summary>
    public class DelegateInterceptor : IInterceptor
    {
        private readonly Interceptor interceptor;

        /// <summary>
        /// Creates the interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor delegate</param>
        public DelegateInterceptor(Interceptor interceptor)
        {
            this.interceptor = interceptor;
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            interceptor(invocation); 
        }
    }
}
