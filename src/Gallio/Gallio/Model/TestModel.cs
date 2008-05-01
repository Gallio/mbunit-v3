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
using System.Collections.Generic;
using Gallio.Collections;

namespace Gallio.Model
{
    /// <summary>
    /// The test model provides access to the contents of the test tree
    /// generated from a test package by the test enumeration process.
    /// </summary>
    public sealed class TestModel
    {
        private readonly TestPackage testPackage;
        private readonly RootTest rootTest;
        private readonly List<Annotation> annotations;

        /// <summary>
        /// Creates a test model with a new empty root test.
        /// </summary>
        /// <param name="testPackage">The test package from which the model was created</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackage"/> is null</exception>
        public TestModel(TestPackage testPackage)
            : this(testPackage, new RootTest())
        {
        }

        /// <summary>
        /// Creates a test model.
        /// </summary>
        /// <param name="testPackage">The test package from which the model was created</param>
        /// <param name="rootTest">The root test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackage" /> or <paramref name="rootTest"/> is null</exception>
        public TestModel(TestPackage testPackage, RootTest rootTest)
        {
            if (testPackage == null)
                throw new ArgumentNullException("testPackage");
            if (rootTest == null)
                throw new ArgumentNullException(@"rootTest");

            this.testPackage = testPackage;
            this.rootTest = rootTest;
            annotations = new List<Annotation>();
        }

        /// <summary>
        /// Gets the test package.
        /// </summary>
        public TestPackage TestPackage
        {
            get { return testPackage; }
        }

        /// <summary>
        /// Gets the root test in the model.
        /// </summary>
        public RootTest RootTest
        {
            get { return rootTest; }
        }

        /// <summary>
        /// Recursively enumerates all tests including the root test.
        /// </summary>
        public IEnumerable<ITest> AllTests
        {
            get
            {
                if (rootTest == null)
                    return EmptyArray<ITest>.Instance;

                return TreeUtils.GetPreOrderTraversal<ITest>(rootTest, GetChildren);
            }
        }

        /// <summary>
        /// <para>
        /// Gets the read-only list of annotations.
        /// </para>
        /// <para>
        /// An annotation is an informational, warning or error message associated with
        /// a code element in the test model.
        /// </para>
        /// <para>
        /// Test frameworks publish annotations on the test model that describe usage errors
        /// or warnings about problems that may prevent tests from running, such as using a
        /// custom attribute incorrectly.  They may also emit informational annotations to
        /// draw the user's attention, such as by flagging ignored or pending tests.
        /// </para>
        /// <para>
        /// The presentation of annotations is undefined.  A command-line test runner might
        /// simply log them whereas an IDE plugin could generate new task items to incorporate
        /// them into the UI.
        /// </para>
        /// </summary>
        public IList<Annotation> Annotations
        {
            get { return annotations.AsReadOnly(); }
        }

        /// <summary>
        /// Adds an annotation.
        /// </summary>
        /// <param name="annotation">The annotation to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="annotation"/> is null</exception>
        /// <seealso cref="Annotations"/>
        public void AddAnnotation(Annotation annotation)
        {
            if (annotation == null)
                throw new ArgumentNullException("annotation");

            annotations.Add(annotation);
        }

        private static IEnumerable<ITest> GetChildren(ITest node)
        {
            return node.Children;
        }
    }
}