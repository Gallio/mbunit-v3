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
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Abstract base implementation of <see cref="ITestRunner" />.
    /// </summary>
    public abstract class BaseRunner : ITestRunner
    {
        private ICoreRuntime runtime;
        private ITestDomainFactory domainFactory;

        private ILogger logger;
        private ITestDomain domain;
        private bool isTemplateTreeReady;
        private bool isTestTreeReady;

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
                    domain = domainFactory.CreateDomain();
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
        public virtual void LoadProject(TestProject project)
        {
            logger.Debug("Loading project into test domain.");

            Domain.LoadProject(project);
        }

        /// <inheritdoc />
        public virtual TemplateInfo GetTemplateTreeRoot()
        {
            BuildTemplatesIfNeeded();
            return Domain.TemplateTreeRoot;
        }

        /// <inheritdoc />
        public virtual TestInfo GetTestTreeRoot()
        {
            BuildTestsIfNeeded();
            return Domain.TestTreeRoot;
        }

        /// <inheritdoc />
        public virtual void Run()
        {
            BuildTestsIfNeeded();
            Domain.RunTests();
        }

        /// <inheritdoc />
        public virtual void WriteReport()
        {
        }

        private void BuildTemplatesIfNeeded()
        {
            if (! isTemplateTreeReady)
            {
                Domain.BuildTemplates();
                isTemplateTreeReady = true;
            }
        }

        private void BuildTestsIfNeeded()
        {
            BuildTemplatesIfNeeded();

            if (!isTestTreeReady)
            {
                Domain.BuildTests();
                isTestTreeReady = true;
            }
        }
    }
}
