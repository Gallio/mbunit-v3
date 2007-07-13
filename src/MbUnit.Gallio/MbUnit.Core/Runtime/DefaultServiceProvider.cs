using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Services;
using MbUnit.Core.Services.Assert;
using MbUnit.Core.Services.Context;
using MbUnit.Core.Services.Report;

namespace MbUnit.Core.Runtime
{
    /// <summary>
    /// The default service provider simply provides instances of the default
    /// implementations of various services.
    /// </summary>
    /// <remarks>
    /// It is anticipated that this will be replaced with a more configurable solution someday.
    /// </remarks>
    public class DefaultServiceProvider : IServiceProvider
    {
        private Dictionary<Type, object> services;

        public DefaultServiceProvider()
        {
            services = new Dictionary<Type, object>();
            services.Add(typeof(IAssertionService), new DefaultAssertionService());
            services.Add(typeof(IContextManager), new DefaultContextManager());
            services.Add(typeof(IReportService), new DefaultReportService());
        }

        public object GetService(Type serviceType)
        {
            return services[serviceType];
        }
    }
}
