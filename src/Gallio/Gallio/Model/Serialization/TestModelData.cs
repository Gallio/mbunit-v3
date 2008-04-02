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
using System.Xml.Serialization;
using Gallio.Model.Serialization;
using Gallio.Utilities;

namespace Gallio.Model.Serialization
{
    /// <summary>
    /// The test model captures the root of the test data tree along with an index by id.
    /// </summary>
    /// <remarks>
    /// This class is safe for used by multiple threads.
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
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestModelData()
        {
            annotations = new List<AnnotationData>();
        }

        /// <summary>
        /// Copies the contents of a test model.
        /// </summary>
        /// <param name="source">The source test model</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TestModelData(TestModel source)
            : this()
        {
            if (source == null)
                throw new ArgumentNullException("source");

            rootTest = new TestData(source.RootTest);

            foreach (Annotation annotation in source.Annotations)
                annotations.Add(new AnnotationData(annotation));
        }

        /// <summary>
        /// Creates a test model.
        /// </summary>
        /// <param name="rootTest">The root test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rootTest"/> is null</exception>
        public TestModelData(TestData rootTest)
            : this()
        {
            if (rootTest == null)
                throw new ArgumentNullException(@"rootTest");

            this.rootTest = rootTest;
        }

        /// <summary>
        /// Gets or sets the root test in the model.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
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
        /// <returns>The numer of error annotations present</returns>
        public int GetErrorAnnotationCount()
        {
            int count = 0;
            foreach (AnnotationData annotation in annotations)
                if (annotation.Type == AnnotationType.Error)
                    count += 1;

            return count;
        }

        /// <summary>
        /// Gets a dictionary of tests indexed by id.
        /// </summary>
        [XmlIgnore]
        public IDictionary<string, TestData> Tests
        {
            get
            {
                lock (this)
                {
                    if (tests == null)
                    {
                        tests = new Dictionary<string, TestData>();
                        PopulateTests(rootTest);
                    }

                    return tests;
                }
            }
        }

        private void PopulateTests(TestData test)
        {
            tests[test.Id] = test;

            foreach (TestData child in test.Children)
                PopulateTests(child);
        }
    }
}