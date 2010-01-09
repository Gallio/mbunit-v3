// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using Gallio.Model;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;
using Gallio.Common.Security;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Gallio.Framework;

namespace MbUnit.Framework
{
    /// <summary>
    /// Run a test or a test fixture under different cultures.
    /// </summary>
    /// <example>
    /// The following example demonstrates a simple test run under multiple cultures. 
    /// It will pass under en-US culture but fail under en-GB.
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     [MultipleCulture("en-US", "en-GB")]
    ///     public void CheckCurrencySymbol()
    ///     {
    ///         Assert.AreEqual("$4.50", String.Format("{0:C}", 4.5d);
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    [CLSCompliant(false)]
    public class MultipleCultureAttribute : TestDecoratorPatternAttribute
    {
        private readonly string[] cultures;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleCultureAttribute"/> class.
        /// </summary>
        /// <param name="cultures">An array of culture names to run the test under.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="cultures"/> is a null reference.</exception>
        public MultipleCultureAttribute(params string[] cultures)
        {
            if (cultures == null)
                throw new ArgumentNullException("cultures");

            this.cultures = cultures;
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.TestInstanceActions.RunTestInstanceBodyChain.Around((state, inner) =>
            {
			    CultureInfo current = Thread.CurrentThread.CurrentCulture;
                TestOutcome outcome = TestOutcome.Passed;
                int passedCount = 0;

                try
                {
                    foreach (string culture in cultures)
                    {
                        string name = String.Format("Culture {0}", culture);
                        
                        TestContext context = TestStep.RunStep(name, () =>
                        {
                            CultureInfo cultureInfo;

                            try
                            {
                                cultureInfo = new CultureInfo(culture);
                            }
                            catch (Exception exception)
                            {
                                TestContext.CurrentContext.LogWriter.Default.WriteException(exception, 
                                    String.Format("An exception occurred while setting the culture to '{0}'.", culture));
                                throw new SilentTestException(TestOutcome.Error);
                            }

                            Thread.CurrentThread.CurrentCulture = cultureInfo;
                            TestOutcome innerOutcome = inner(state);

                            if (innerOutcome.Status != TestStatus.Passed)
                                throw new SilentTestException(innerOutcome);
                        }, null, false, codeElement);

                        outcome = outcome.CombineWith(context.Outcome);

                        if (context.Outcome.Status == TestStatus.Passed)
                            passedCount ++;
                    }
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = current;
                }

                TestLog.WriteLine(String.Format("{0} of {1} cultures passed.", passedCount, cultures.Length));
                return outcome;
            });
        }
    }
}
			
