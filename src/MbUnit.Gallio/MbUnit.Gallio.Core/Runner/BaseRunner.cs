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
using System.Text;
using Castle.Core.Logging;
using MbUnit.Core.Serialization;
using MbUnit.Core.Services.Runtime;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Abstract base implementation of <see cref="ITestRunner" />.
    /// </summary>
    public abstract class BaseRunner : ITestRunner
    {
        private EventDispatcher eventDispatcher;
        private ICoreRuntime runtime;
        private ITestDomainFactory domainFactory;

        private TemplateEnumerationOptions templateEnumerationOptions;
        private TestEnumerationOptions testEnumerationOptions;
        private TestExecutionOptions testExecutionOptions;

        private ILogger logger;
        private ITestDomain domain;

        /// <summary>
        /// Creates a runner with a specified test domain factory.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <param name="domainFactory">The test domain factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/>
        /// or <paramref name="domainFactory"/> is null</exception>
        public BaseRunner(ICoreRuntime runtime, ITestDomainFactory domainFactory)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (domainFactory == null)
                throw new ArgumentNullException("domainFactory");

            this.runtime = runtime;
            this.domainFactory = domainFactory;

            logger = NullLogger.Instance;
            eventDispatcher = new EventDispatcher();
            templateEnumerationOptions = new TemplateEnumerationOptions();
            testEnumerationOptions = new TestEnumerationOptions();
            testExecutionOptions = new TestExecutionOptions();
        }

        /// <summary>
        /// Gets or sets the logger for writing messages.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        /// <summary>
        /// Gets the runtime used by the test runner to obtain services.
        /// </summary>
        public ICoreRuntime Runtime
        {
            get { return runtime; }
            protected set { runtime = value; }
        }

        /// <summary>
        /// Gets the test domain used by the runner.
        /// </summary>
        public ITestDomain Domain
        {
            get
            {
                if (domain == null)
                {
                    domain = domainFactory.CreateDomain();
                    domain.SetEventListener(eventDispatcher);
                }

                return domain;
            }
            protected set { domain = value; }
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            if (domain != null)
            {
                domain.Dispose();
                domain = null;
            }
        }

        /// <inheritdoc />
        public EventDispatcher EventDispatcher
        {
            get { return eventDispatcher; }
        }

        /// <inheritdoc />
        public TemplateEnumerationOptions TemplateEnumerationOptions
        {
            get { return templateEnumerationOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                templateEnumerationOptions = value;
            }
        }

        /// <inheritdoc />
        public TestEnumerationOptions TestEnumerationOptions
        {
            get { return testEnumerationOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                testEnumerationOptions = value;
            }
        }

        /// <inheritdoc />
        public TestExecutionOptions TestExecutionOptions
        {
            get { return testExecutionOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                testExecutionOptions = value;
            }
        }

        /// <inheritdoc />
        public TemplateModel TemplateModel
        {
            get { return Domain.TemplateModel; }
        }

        /// <inheritdoc />
        public TestModel TestModel
        {
            get { return Domain.TestModel; }
        }

        /// <inheritdoc />
        public virtual void LoadProject(TestProject project)
        {
            logger.Debug("Loading project into test domain.");

            Domain.LoadProject(project);
        }

        /// <inheritdoc />
        public virtual void BuildTemplates()
        {
            logger.Debug("Building templates.");

            Domain.BuildTemplates(templateEnumerationOptions);
        }

        /// <inheritdoc />
        public virtual void BuildTests()
        {
            logger.Debug("Building tests.");

            Domain.BuildTests(testEnumerationOptions);
        }

        /// <inheritdoc />
        public virtual void Run()
        {
            logger.Debug("Running tests.");

            Domain.RunTests(testExecutionOptions);
        }

        /// <inheritdoc />
        public virtual void WriteReport()
        {
        }
    }
}
