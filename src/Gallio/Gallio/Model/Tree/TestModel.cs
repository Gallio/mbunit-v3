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

namespace Gallio.Model.Tree
{
    /// <summary>
    /// The test model provides access to the contents of the test tree
    /// generated from a test package by the test enumeration process.
    /// </summary>
    public class TestModel
    {
        private Test rootTest;
        private readonly List<Annotation> annotations;

        /// <summary>
        /// Creates a test model with a new empty root test.
        /// </summary>
        public TestModel()
        {
            annotations = new List<Annotation>();
        }

        /// <summary>
        /// Gets or sets the root test in the model.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public Test RootTest
        {
            get
            {
                if (rootTest == null)
                    rootTest = new RootTest();
                return rootTest;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                rootTest = value;
            }
        }

        /// <summary>
        /// Recursively enumerates all tests including the root test.
        /// </summary>
        public IEnumerable<Test> AllTests
        {
            get { return TreeUtils.GetPreOrderTraversal(RootTest, GetChildren); }
        }

        /// <summary>
        /// Gets the read-only list of annotations.
        /// </summary>
        /// <remarks>
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
        /// </remarks>
        public IList<Annotation> Annotations
        {
            get { return annotations.AsReadOnly(); }
        }

        /// <summary>
        /// Clears the list of annotations.
        /// </summary>
        public void ClearAnnotations()
        {
            annotations.Clear();
        }

        /// <summary>
        /// Adds an annotation.
        /// </summary>
        /// <param name="annotation">The annotation to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="annotation"/> is null.</exception>
        /// <seealso cref="Annotations"/>
        public void AddAnnotation(Annotation annotation)
        {
            if (annotation == null)
                throw new ArgumentNullException("annotation");

            annotations.Add(annotation);
        }

        /// <summary>
        /// Removes an annotation.
        /// </summary>
        /// <param name="annotation">The annotation to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="annotation"/> is null.</exception>
        /// <seealso cref="Annotations"/>
        public void RemoveAnnotation(Annotation annotation)
        {
            if (annotation == null)
                throw new ArgumentNullException("annotation");

            annotations.Remove(annotation);
        }

        /// <summary>
        /// Finds a test by its id.
        /// </summary>
        /// <param name="testId">The test id.</param>
        /// <returns>The test, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> is null.</exception>
        public Test FindTest(string testId)
        {
            if (testId == null)
                throw new ArgumentNullException("testId");

            foreach (Test test in AllTests)
            {
                if (test.Id == testId)
                    return test;
            }

            return null;
        }

        private static IEnumerable<Test> GetChildren(Test node)
        {
            return node.Children;
        }
    }
}