using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MbUnit.Core.Services;
using MbUnit.Core.Services.Assert;
using MbUnit.Core.Services.Context;
using MbUnit.Core.Services.Report;
using MbUnit.Framework;

namespace MbUnit.Core.Runtime
{
    /// <summary>
    /// The runtime class is instantiated within the test domain to provide a suitable
    /// hosting environment for test enumeration and execution.
    /// 
    /// The runtime provides a number of services to the MbUnit core libraries.
    /// These services may be customized or mocked during initialization by providing
    /// a suitable <see cref="IServiceProvider" /> allowing for diverse hosting scenarios.
    /// Moreover, additional services might be offered without requiring any change to the
    /// runtime.
    /// </summary>
    public sealed class Runtime : IGenericServiceProvider
    {
        private static readonly object syncRoot = new object();
        private static Runtime instance;

        private TextReader oldConsoleIn;
        private TextWriter oldConsoleOut;
        private TextWriter oldConsoleError;

        private ContextualReportTraceListener debugListener;
        private ContextualReportTraceListener traceListener;

        private IServiceProvider serviceProvider;
        private IContextManager contextManager;
        private IAssertionService assertionService;
        private IReportService reportService;

        private Runtime(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the runtime singleton or null if it has not been initialized yet.
        /// </summary>
        public static Runtime Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Gets the context manager service.
        /// </summary>
        public IContextManager ContextManager
        {
            get { return contextManager; }
        }

        /// <summary>
        /// Gets the assertion manager service.
        /// </summary>
        public IAssertionService AssertionService
        {
            get { return assertionService; }
        }

        /// <summary>
        /// Gets the report manager service.
        /// </summary>
        public IReportService ReportingService
        {
            get { return reportService; }
        }

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <remarks>
        /// This is functionally equivalent to calling <see cref="IServiceProvider.GetService" />
        /// with the specified type then casting the resulting object.
        /// </remarks>
        /// <typeparam name="T">The type of service to obtain</typeparam>
        /// <returns>An instance of a component that implements that service</returns>
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>An instance of a component that implements that service</returns>
        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Sets up the runtime environment.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <exception cref="InvalidOperationException">Thrown if the runtime has already been set up</exception>
        public static void SetUp(IServiceProvider serviceProvider)
        {
            lock (syncRoot)
            {
                if (instance != null)
                    throw new InvalidOperationException("The runtime has already been set up.");

                instance = new Runtime(serviceProvider);
                instance.InternalSetUp();
            }
        }

        /// <summary>
        /// Tears down the runtime environment.
        /// Does nothing if the runtime has not been set up.
        /// </summary>
        public static void TearDown()
        {
            lock (syncRoot)
            {
                if (instance != null)
                {
                    instance.InternalTearDown();
                    instance = null;
                }
            }
        }

        private void InternalSetUp()
        {
            // Obtain services.
            contextManager = GetService<IContextManager>();
            assertionService = GetService<IAssertionService>();
            reportService = GetService<IReportService>();

            // Save the old console streams.
            oldConsoleIn = Console.In;
            oldConsoleOut = Console.Out;
            oldConsoleError = Console.Error;

            // Inject debug and trace listeners.
            debugListener = new ContextualReportTraceListener(Report.DebugStreamName);
            traceListener = new ContextualReportTraceListener(Report.TraceStreamName);
            Debug.Listeners.Add(debugListener);
            Debug.AutoFlush = true;

            Trace.Listeners.Add(traceListener);
            Trace.AutoFlush = true;
            
            // Inject console streams.

        }

        private void InternalTearDown()
        {
            // Remove debug and trace listeners.
            Debug.Listeners.Remove(debugListener);
            Trace.Listeners.Remove(traceListener);

            // Restore the old console streams.
            Console.SetIn(oldConsoleIn);
            Console.SetOut(oldConsoleOut);
            Console.SetError(oldConsoleError);
        }
    }
}
