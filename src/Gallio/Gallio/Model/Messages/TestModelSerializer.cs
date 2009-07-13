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
using Gallio.Model.Messages.Exploration;
using Gallio.Common.Messaging;
using Gallio.Model.Schema;
using Gallio.Model.Tree;

namespace Gallio.Model.Messages
{
    /// <summary>
    /// Provides functions for converting a <see cref="TestModel" /> into a sequence of
    /// messages and back.
    /// </summary>
    public static class TestModelSerializer
    {
        /// <summary>
        /// Publishes the contents of the test model to a message sink as test
        /// and annotation discovered messages.
        /// </summary>
        /// <param name="testModel">The test model to publish.</param>
        /// <param name="messageSink">The message sink.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/>
        /// or <paramref name="messageSink"/> is null.</exception>
        public static void PublishTestModel(TestModel testModel, IMessageSink messageSink)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");
            if (messageSink == null)
                throw new ArgumentNullException("messageSink");

            foreach (Annotation annotation in testModel.Annotations)
            {
                messageSink.Publish(new AnnotationDiscoveredMessage()
                {
                    Annotation = new AnnotationData(annotation)
                });
            }

            foreach (Test test in testModel.AllTests)
            {
                messageSink.Publish(new TestDiscoveredMessage()
                {
                    Test = new TestData(test),
                    ParentTestId = test.Parent != null ? test.Parent.Id : null
                });
            }
        }

        /// <summary>
        /// Creates a message sink that populates the test model as test and annotation
        /// discovered messages are published.
        /// </summary>
        /// <param name="testModel">The test model to populate.</param>
        /// <returns>The message sink.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/> is null.</exception>
        public static IMessageSink CreateMessageSinkToPopulateTestModel(TestModel testModel)
        {
            if (testModel == null)
                throw new ArgumentNullException("testModel");

            return new MessageConsumer()
                .Handle<TestDiscoveredMessage>(message =>
                {
                    Test test = message.Test.ToTest();

                    if (message.ParentTestId != null)
                    {
                        Test parentTest = testModel.FindTest(message.ParentTestId);
                        if (parentTest == null)
                            throw new InvalidOperationException("The parent test is missing.");

                        parentTest.AddChild(test);
                    }
                    else
                    {
                        testModel.RootTest = test;
                    }
                })
                .Handle<AnnotationDiscoveredMessage>(message =>
                {
                    testModel.AddAnnotation(message.Annotation.ToAnnotation());
                });
        }
    }
}