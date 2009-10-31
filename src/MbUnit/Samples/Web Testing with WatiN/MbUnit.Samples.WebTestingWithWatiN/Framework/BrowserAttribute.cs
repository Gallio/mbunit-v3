// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using WatiN.Core;

namespace MbUnit.Samples.WebTestingWithWatiN.Framework
{
    /// <summary>
    /// Declares that a test requires the use of a WatiN browser.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When this attribute is applied to a test method, it transforms the test into
    /// a browser based test with additional set up and tear down logic to establish
    /// a <see cref="BrowserContext" /> and obtain <see cref="Browser" /> instances.
    /// </para>
    /// <para>
    /// This attribute may be applied multiple times to a test to run it repeatedly
    /// with different browser configurations.
    /// </para>
    /// </remarks>
    /// <example>
    /// </example>
    /// <seealso cref="BrowserTestFixture"/>
    [AttributeUsage(PatternAttributeTargets.TestMethod, AllowMultiple = true, Inherited = true)]
    public class BrowserAttribute : TestMethodDecoratorPatternAttribute, IBrowserConfiguration
    {
        private const string BrowserDataParameterName = "BrowserData";
        private static readonly Key<BrowserData> BrowserDataKey = new Key<BrowserData>("BrowserData");

        private readonly BrowserType browserType;

        /// <summary>
        /// Declares that a test requires the use of a WatiN browser.
        /// </summary>
        /// <param name="browserType">The browser type to use.</param>
        public BrowserAttribute(BrowserType browserType)
        {
            this.browserType = browserType;

            BrowserSnapshotTriggerEvent = TriggerEvent.Never;
            BrowserSnapshotZoom = 1.0;

            ScreenRecordingTriggerEvent = TriggerEvent.TestFailedOrInconclusive;
            ScreenRecordingZoom = 0.25;
            ScreenRecordingFramesPerSecond = 5;
        }

        /// <summary>
        /// Gets the browser type for the test.
        /// </summary>
        public BrowserType BrowserType
        {
            get { return browserType; }
        }

        /// <summary>
        /// Gets or sets when a browser snapshot should be captured and embedded.
        /// </summary>
        /// <remarks>
        /// The default setting is <see cref="TriggerEvent.Never"/> because by
        /// default we embed a screen recording on failure or inconclusive instead.
        /// </remarks>
        public TriggerEvent BrowserSnapshotTriggerEvent { get; set; }

        /// <summary>
        /// Gets or sets the zoom factor for browser snapshots.
        /// </summary>
        /// <remarks>
        /// The default setting is 1.0.
        /// </remarks>
        public double BrowserSnapshotZoom { get; set; }

        /// <summary>
        /// Gets or sets when a screen recording should be captured and embedded.
        /// </summary>
        /// <remarks>
        /// The default setting is <see cref="TriggerEvent.TestFailedOrInconclusive"/>.
        /// </remarks>
        public TriggerEvent ScreenRecordingTriggerEvent { get; set; }

        /// <summary>
        /// Gets or sets the zoom factor for screen recordings.
        /// </summary>
        /// <remarks>
        /// The default setting is 0.5.
        /// </remarks>
        public double ScreenRecordingZoom { get; set; }

        /// <summary>
        /// Gets or sets the number of frames per second for screen recordings.
        /// </summary>
        /// <remarks>
        /// The default setting is 0.25.
        /// </remarks>
        public double ScreenRecordingFramesPerSecond { get; set; }

        /// <inheritdoc />
        public virtual string Label
        {
            get { return browserType.ToString(); }
        }

        /// <inheritdoc />
        public virtual BrowserContext CreateBrowserContext()
        {
            return new BrowserContext(this);
        }

        /// <inheritdoc />
        protected override void DecorateMethodTest(IPatternScope methodScope, IMethodInfo method)
        {
            ITestParameterBuilder parameterBuilder = GetOrCreateBrowserDataTestParameter(methodScope);

            BrowserData browserData = new BrowserData() { BrowserConfiguration = this };
            EnlistBrowserData(parameterBuilder, browserData);
        }

        private static ITestParameterBuilder GetOrCreateBrowserDataTestParameter(IPatternScope methodScope)
        {
            ITestParameterBuilder parameterBuilder = methodScope.TestBuilder.GetParameter(BrowserDataParameterName);
            if (parameterBuilder == null)
            {
                // Add a new test parameter for some BrowserData.  This makes the test data-driven.
                var parameterDataContextBuilder = methodScope.TestDataContextBuilder.CreateChild();
                parameterBuilder = methodScope.TestBuilder.CreateParameter(BrowserDataParameterName, null, parameterDataContextBuilder);

                // When the BrowserData is bound to the parameter before the initialization phase
                // of the test, add it to the test instance state so we can access it later.
                parameterBuilder.TestParameterActions.BindTestParameterChain.After((state, obj) =>
                {
                    var browserData = (BrowserData)obj;
                    state.Data.SetValue(BrowserDataKey, browserData);

                    state.AddNameSuffix(browserData.BrowserConfiguration.Label);
                });

                // When the test is initialized (before other set up actions occur), set up the browser context.
                methodScope.TestBuilder.TestInstanceActions.InitializeTestInstanceChain.After(state =>
                {
                    BrowserData browserData = state.Data.GetValue(BrowserDataKey);

                    BrowserContext browserContext = new BrowserContext(browserData.BrowserConfiguration);
                    BrowserContext.SetBrowserContext(TestContext.CurrentContext, browserContext);

                    browserContext.SetUp();
                });

                // When the test is disposed (after other tear down actions occur), tear down the browser context.
                methodScope.TestBuilder.TestInstanceActions.DisposeTestInstanceChain.Before(state =>
                {
                    if (BrowserContext.HasCurrentBrowserContext)
                    {
                        try
                        {
                            BrowserContext.CurrentBrowserContext.TearDown();
                        }
                        finally
                        {
                            BrowserContext.SetBrowserContext(TestContext.CurrentContext, null);
                        }
                    }
                });
            }

            return parameterBuilder;
        }

        private static void EnlistBrowserData(ITestParameterBuilder parameterBuilder, BrowserData browserData)
        {
            DataSource dataSource = parameterBuilder.TestDataContextBuilder.DefineDataSource("");
            dataSource.AddDataSet(new ValueSequenceDataSet(new[] { browserData }, null, false));
        }

        /// <summary>
        /// The data object that is bound to the test parameter.
        /// </summary>
        private sealed class BrowserData
        {
            public IBrowserConfiguration BrowserConfiguration { get; set; }
        }
    }
}
