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
using System.Linq;
using System.Text;
using Gallio.Common.Messaging;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Messaging
{
    [TestsOn(typeof(Topic))]
    public class TopicTest
    {
        [Test]
        public void Constructor_WhenKeyIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Topic(null));
        }

        [Test]
        [Row("invalid-topic")]
        [Row("invalid topic")]
        [Row("!")]
        public void Constructor_WhenKeyContainsInvalidCharacters_Throws(string key)
        {
            var ex = Assert.Throws<ArgumentException>(() => new Topic(key));
            Assert.Contains(ex.Message, "A topic key must consist of dot-delimited alphanumeric words.");
        }

        [Test]
        [Row("")]
        [Row("topic")]
        [Row("topic.subtopic")]
        [Row("abc.ABC.123")]
        public void Constructor_WhenKeyIsValid_SetsKeyProperty(string key)
        {
            var topic = new Topic(key);

            Assert.AreEqual(key, topic.Key);
        }

        [Test]
        public void ToString_ReturnsTheKey()
        {
            var topic = new Topic("my.topic");

            Assert.AreEqual("my.topic", topic.ToString());
        }

        [Test]
        public void IsMatch_WhenTopicIsNull_Throws()
        {
            var topicPattern = new TopicPattern("topic");

            Assert.Throws<ArgumentNullException>(() => topicPattern.IsMatch(null));
        }

        [Test]
        [Row("", "", true)]
        [Row("", "topic", false)]
        [Row("topic", "", false)]
        [Row("topic", "topic", true)]
        [Row("topic", "differenttopic", false)]
        [Row("topic", "topic.suffix", false)]
        [Row("topic", "prefix.topic", false)]
        [Row("*", "word", true)]
        [Row("*", "word.anotherword", false)]
        [Row("#", "word", true)]
        [Row("#", "word.anotherword", true)]
        [Row("prefix.*.suffix", "prefix.word.suffix", true)]
        [Row("prefix.*.suffix", "prefix.word.word.suffix", false)]
        [Row("prefix.#.suffix", "prefix.word.suffix", true)]
        [Row("prefix.#.suffix", "prefix.word.word.suffix", true)]
        public void IsMatch_WhenTopicIsNotNull_ReturnsTrueIfMatched(string pattern, string topic, bool expectedResult)
        {
            var topicPattern = new TopicPattern(pattern);

            Assert.AreEqual(expectedResult, topicPattern.IsMatch(new Topic(topic)));
        }
    }
}