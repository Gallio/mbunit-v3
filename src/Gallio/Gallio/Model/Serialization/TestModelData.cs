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
using System.Xml.Serialization;
using Gallio.Common.Collections;
using Gallio.Common.Xml;

namespace Gallio.Model.Serialization
{
    /// <summary>
    /// The test model captures the root of the test data tree along with an index by id.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is safe for used by multiple threads.
    /// </para>
    /// </remarks>
    [Serializable]
    [XmlRoot("testModel", Namespace=XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace=XmlSerializationUtils.GallioNamespace)]
    public sealed class TestModelData
    {
        [NonSerialized]
        private Dictionary<string, TestData> tests;

        private readonly List<AnnotationData> annotations;
        private TestData rootTest;

        /// <summary>
        /// Creates an empty test model with just a root test.
        /// </summary>
        public TestModelData()
            : this(new TestData(new RootTest()))
        {
        }

        /// <summary>
        /// Copies the contents of a test model.
        /// </summary>
        /// <param name="source">The source test model.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
        public TestModelData(TestModel source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            rootTest = new TestData(source.RootTest);

            annotations = new List<AnnotationData>();
            foreach (Annotation annotation in source.Annotations)
                annotations.Add(new AnnotationData(annotation));
        }

        /// <summary>
        /// Creates a test model.
        /// </summary>
        /// <param name="rootTest">The root test.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rootTest"/> is null.</exception>
        public TestModelData(TestData rootTest)
        {
            if (rootTest == null)
                throw new ArgumentNullException(@"rootTest");

            this.rootTest = rootTest;

            annotations = new List<AnnotationData>();
        }

        /// <summary>
        /// Gets or sets the root test in the model.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlElement("test", IsNullable = false)]
        public TestData RootTest
        {
            get { return rootTest; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                lock (this)
                {
                    rootTest = value;
                    tests = null;
                }
            }
        }

        /// <summary>
        /// Gets the mutable list of annotations.
        /// </summary>
        /// <seealso cref="TestModel.Annotations"/>
        [XmlArray("annotations", IsNullable = false)]
        [XmlArrayItem("annotation", typeof(AnnotationData), IsNullable = false)]
        public List<AnnotationData> Annotations
        {
            get { return annotations; }
        }

        /// <summary>
        /// Gets the number of error annotations on the model.
        /// </summary>
        /// <returns>The numer of error annotations present.</returns>
        public int GetErrorAnnotationCount()
        {
            int count = 0;
            foreach (AnnotationData annotation in annotations)
                if (annotation.Type == AnnotationType.Error)
                    count += 1;

            return count;
        }

        /// <summary>
        /// Recursively enumerates all tests including the root test.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<TestData> AllTests
        {
            get
            {
                if (rootTest == null)
                    return EmptyArray<TestData>.Instance;

                return rootTest.AllTests;
            }
        }

        /// <summary>
        /// Gets a test by its id.
        /// </summary>
        /// <param name="testId">The test id.</param>
        /// <returns>The test, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> is null.</exception>
        public TestData GetTestById(string testId)
        {
            if (testId == null)
                throw new ArgumentNullException("testId");

            lock (this)
            {
                if (tests == null)
                {
                    tests = new Dictionary<string, TestData>();
                    foreach (TestData test in AllTests)
                        tests[test.Id] = test;
                }

                TestData testData;
                tests.TryGetValue(testId, out testData);
                return testData;
            }
        }

        /// <summary>
        /// Resets the test index by id in case the test model has been modified.
        /// </summary>
        public void ResetIndex()
        {
            lock (this)
                tests = null;
        }

        /// <summary>
        /// Merged a subtree of tests into the model.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Merges tests with duplicate ids. Adds new tests in place.
        /// </para>
        /// </remarks>
        /// <param name="parentTestId">The id of the parent test, or null if adding the root.</param>
        /// <param name="test">The top test of the subtree to add.</param>
        /// <returns>The merged test, if the test was already present in the tree.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null.</exception>
        public TestData MergeSubtree(string parentTestId, TestData test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            lock (this)
            {
                TestData targetTest = GetTestById(test.Id);
                if (targetTest == null)
                {
                    TestData parentTest = GetTestById(parentTestId);
                    parentTest.Children.Add(test);

                    foreach (TestData addedTest in test.AllTests)
                        tests.Add(addedTest.Id, addedTest);

                    return test;
                }
                else
                {
                    foreach (KeyValuePair<string, IList<string>> pairs in test.Metadata)
                        foreach (string value in pairs.Value)
                            if (!targetTest.Metadata.Contains(pairs.Key, value))
                                targetTest.Metadata.Add(pairs.Key, value);

                    foreach (TestData child in test.Children)
                        MergeSubtree(test.Id, child);

                    return targetTest;
                }
            }
        }
    }
}