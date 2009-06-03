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
using System.Collections.Generic;
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// The pattern test framework is a built-in framework that Gallio provides based on
    /// reflection over attributes that implement <see cref="IPattern" />.
    /// </para>
    /// <para>
    /// The pattern test framework does not provide many attributes that end-users would use
    /// to write tests.  The framework is intended to be extended by 
    /// components and libraries that define the test syntax and other facilities.
    /// </para>
    /// <para>
    /// For example, a Test-Driven framework would augment the base framework with a syntax based
    /// around test fixtures, test methods and assertions.  A Behavior-Driven framework
    /// would instead use a syntax based around contexts and specifications.  Both frameworks
    /// would share the common attribute-based model provided by the pattern test framework.
    /// They may even interoperate to a large extent.
    /// </para>
    /// <para>
    /// Other add-on libraries and tools may further contribute functionality to the pattern
    /// test framework by registering components with the runtime or by subclassing
    /// pattern attributes.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Gallio supports the use of multiple test frameworks.  The pattern test framework
    /// model defined here may not be appropriate for all purposes.  Therefore you may
    /// consider creating a new test framework from scratch by implementing <see cref="ITestFramework" />
    /// and <see cref="ITestExplorer" /> appropriately to obtain the desired semantics.
    /// </para>
    /// <para>
    /// The name of the Pattern Test Framework derives from the similarity between the recursive
    /// construction process performed by <see cref="IPattern" /> and the kind of pattern matching
    /// that is performed by a recursive descent parser.  Each primary <see cref="IPattern" /> can be
    /// thought of as a grammar rule that matches a given code element and describes how further
    /// matching of nested code elements is intended to occur.  The actual matching performed is
    /// somewhat simplistic (largely based on attribute lookup) but on the whole the framework
    /// provides a quite flexible implementation pattern for a wide variety of testing features.
    /// </para>
    /// </remarks>
    public abstract class PatternTestFramework : BaseTestFramework
    {
        /// <inheritdoc />
        public override void RegisterTestExplorers(IList<ITestExplorer> explorers)
        {
            var explorer = (PatternTestExplorer)GenericCollectionUtils.Find(explorers, x => x is PatternTestExplorer);
            if (explorer == null)
            {
                explorer = new PatternTestExplorer();
                explorers.Add(explorer);
            }

            explorer.RegisterExtensionProvider(GetExtensions);
        }

        /// <summary>
        /// Gets information about a particular pattern test framework extension.
        /// The information will be included in the report as part of the framework
        /// node that contains the assembly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there are not framework extensions applied to a particular assembly then
        /// the assembly will not be explored for tests.
        /// </para>
        /// </remarks>
        /// <param name="assembly">The test assembly.</param>
        /// <returns>The pattern test framework extension information.</returns>
        protected abstract IEnumerable<PatternTestFrameworkExtensionInfo> GetExtensions(IAssemblyInfo assembly);
    }
}