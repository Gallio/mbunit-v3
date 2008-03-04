// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The mixin attribute combines tests, test parameters, setup/teardown methods,
    /// data sources and other test elements declared by a target mixin class into a
    /// host test.  The target mixin class is incorporated as a surrogate
    /// of the host test and participates in all phases of the test lifecycle
    /// including data binding, setup/teardown and test execution.
    /// </para>
    /// <para>
    /// At runtime, an instance of the target class is created and injected
    /// into the host by way of the property, field, constructor parameter or method
    /// parameter to which the mixin attribute was applied.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Mixins provide an reuse mechanism for common test behaviors in a similar
    /// but more general manner than abstract test fixtures.
    /// </para>
    /// <para>
    /// In many cases, it may be simpler to create an abstract class and to define
    /// your fixtures as concrete subclasses.  However, using abstract classes has
    /// the undesirable effect of constraining the composition of separately-defined
    /// orthogonal behaviors because of the lack of multiple inheritance.
    /// </para>
    /// <para>
    /// Mixins make it easy to create a library of reusable test components that
    /// can be combined at will by end-users.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// This code defines a mixin class that encapsulates a browser initialization
    /// concern.  During the setup phase, the browser is initialized, during teardown
    /// it is disposed.  If an error occurred during test execution, the mixin also
    /// captures a screenshot that it incorporates into the log.  Finally, all tests
    /// that use the BrowserMixin are included in the "Web Tests" category.
    /// </para>
    /// <code>
    /// [Category("Web Tests")]
    /// public class BrowserMixin
    /// {
    ///     private Browser browser;
    ///     
    ///     public Browser Browser
    ///     {
    ///         get { return browser; }
    ///     }
    /// 
    ///     [SetUp]
    ///     public void SetUp()
    ///     {
    ///         browser = new Browser();
    ///     }
    ///     
    ///     [TearDown]
    ///     public void TearDown()
    ///     {
    ///         if (browser != null)
    ///         {
    ///             if (Context.CurrentContext.Outcome != TestOutcome.Success)
    ///             {
    ///                 using (Log.BeginSection("Last screen prior to test failure.");
    ///                     Log.EmbedImage(browser.TakeScreenshot());
    ///             }
    /// 
    ///             browser.Dispose();
    ///             browser = null;
    ///         }
    ///     }
    /// }
    /// </code>
    /// <para>
    /// The following test fixture class uses the BrowserMixin as part of its tests
    /// by injecting it as a field.
    /// </para>
    /// <code>
    /// [TestFixture]
    /// public class HostTestFixture
    /// {
    ///     [Mixin]
    ///     public BrowserMixin mixin;
    ///     
    ///     public Browser Browser
    ///     {
    ///         get { return mixin.Browser; }
    ///     }
    ///     
    ///     [Test]
    ///     public void ATest()
    ///     {
    ///         Browser.GoTo("http://www.google.com");
    ///         Browser.TextField(Find.ByName("q")).TypeText("MbUnit");
    ///         Browser.Button(Find.ByName("btnI")).Click();
    ///         
    ///         Assert.IsTrue(Browser.ContainsText("MbUnit"));
    ///     }
    /// }
    /// </code>
    /// <para>
    /// Other injection possibilities are:
    /// </para>
    /// <code>
    /// // Inject as a field.
    /// public BrowserMixin mixin;
    /// 
    /// // Inject as a property.
    /// [Mixin]
    /// public BrowserMixin Mixin { get; set; }
    /// 
    /// // Inject as a constructor parameter.
    /// public HostTestFixture([Mixin] BrowserMixin mixin) { ... }
    /// 
    /// // Inject as a test method parameter.
    /// [Test]
    /// public void ATest([Mixin] BrowserMixin mixin) { ... }
    /// </code>
    /// </example>
    public class MixinAttribute
    {
        private readonly Type mixinType;

        /// <summary>
        /// Injects a mixin to use as the value of a fixture property, field, constructor
        /// parameter or test method parameter.
        /// </summary>
        /// <param name="mixinType">The mixin type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mixinType"/> is null</exception>
        public MixinAttribute(Type mixinType)
        {
            if (mixinType == null)
                throw new ArgumentNullException("mixinType");

            this.mixinType = mixinType;
        }

        /// <summary>
        /// Gets the mixin type.
        /// </summary>
        public Type MixinType
        {
            get { return mixinType; }
        }
    }
}
