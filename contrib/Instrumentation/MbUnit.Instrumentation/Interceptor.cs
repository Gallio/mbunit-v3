using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Interceptor;

namespace MbUnit.Instrumentation
{
    /// <summary>
    /// A representation of <see cref="IInterceptor" /> as a delegate.
    /// </summary>
    /// <param name="invocation">The intercepted invocation</param>
    public delegate void Interceptor(IInvocation invocation);
}
