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
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

#if false // not implemented yet

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares a mixin which can be linked into another test fixture or mixin to incorporate
    /// additional functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A mixin specifies common testing behaviors that can subsequently be
    /// linked into test fixtures as a unit of reuse.
    /// </para>
    /// <para>
    /// The linking mechanism works by defining a field or property of the mixin's
    /// type on the test fixture which is to acquire new functionality.
    /// Unlike abstract test fixture base class, multiple mixins can be linked into
    /// a single test fixture.  It is also possible to create composite mixins
    /// that express the behavior of multiple mixins taken together.
    /// </para>
    /// <para>
    /// A mixin typically expresses a single test infrastructure concern.  For example,
    /// preparing an empty database, starting / stopping a server, checking common
    /// interface contracts, creating a mock object repository, or providing access to
    /// a web browser for integration testing.
    /// </para>
    /// <example>
    /// <para>
    /// The following code sample code defines a mixin class that encapsulates a
    /// browser initialization concern.  During the setup phase the browser is
    /// initialized and during teardown it is disposed.  If an error occurred during
    /// test execution, the mixin also captures a screenshot that it writes to
    /// the log.  Finally, all test fixtures that use the BrowserMixin are
    /// automatically included in the "Web Tests" category.
    /// </para>
    /// <code>
    /// // A test fixture that uses a BrowserMixin will be placed in the "Web Tests" category.
    /// [Mixin]
    /// [Category("Web Tests")]
    /// public class BrowserMixin
    /// {
    ///     private Browser browser;
    ///     
    ///     // Gets the web browser instance.
    ///     public Browser Browser
    ///     {
    ///         get { return browser; }
    ///     }
    /// 
    ///     // This setup action will create a new instance of the browser before each test runs.
    ///     [SetUp]
    ///     public void SetUp()
    ///     {
    ///         browser = new Browser();
    ///     }
    ///     
    ///     // This teardown action will take a screenshot if the test failed then
    ///     // dispose them browser after each test runs.
    ///     [TearDown]
    ///     public void TearDown()
    ///     {
    ///         if (browser != null)
    ///         {
    ///             if (Context.CurrentContext.Outcome != TestOutcome.Success)
    ///             {
    ///                 using (TestLog.BeginSection("Last screen prior to test failure.");
    ///                     TestLog.EmbedImage(browser.TakeScreenshot());
    ///             }
    /// 
    ///             browser.Dispose();
    ///             browser = null;
    ///         }
    ///     }
    /// }
    /// </code>
    /// <para>
    /// The following test fixture class uses the example BrowserMixin define above as
    /// part of its tests.  The mixin is linked in via a field.
    /// </para>
    /// <code>
    /// [TestFixture]
    /// public class HomePageTests
    /// {
    ///     // Specifies that we want to include the browser mixin into the fixture.
    ///     // The field assignment ensures that the a BrowserMixin object will be created
    ///     // and ready for use when the fixture itself is instantiated.
    ///     [LinkMixin]
    ///     public BrowserMixin browserMixin = new BrowserMixin();
    ///     
    ///     // Convenience propoerty to access the web browser provided by the mixin.
    ///     public Browser Browser
    ///     {
    ///         get { return browserMixin.Browser; }
    ///     }
    ///     
    ///     [Test]
    ///     public void Search()
    ///     {
    ///         Browser.GoTo("http://www.google.com");
    ///         Browser.TextField(Find.ByName("q")).TypeText("MbUnit");
    ///         Browser.Button(Find.ByName("btnI")).Click();
    ///         
    ///         Assert.IsTrue(Browser.ContainsText("MbUnit"));
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class MixinAttribute : TestTypePatternAttribute
    {
        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            ITypeInfo type = codeElement as ITypeInfo;
            Validate(containingScope, type);

            ConsumeMembers(containingScope, type);
            ConsumeNestedTypes(containingScope, type);
        }
    }
}

#endif