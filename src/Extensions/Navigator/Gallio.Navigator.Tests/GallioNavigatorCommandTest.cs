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
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Navigator.Tests
{
    [TestsOn(typeof(GallioNavigatorCommand))]
    public class GallioNavigatorCommandTest
    {
        [Test]
        public void ParseUri_WhenArgumentIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => GallioNavigatorCommand.ParseUri(null));
        }

        [Test]
        public void ParseUri_ProtocolIsInvalid_ReturnsNull()
        {
            var command = GallioNavigatorCommand.ParseUri("proto:foo");

            Assert.IsNull(command);
        }

        [Test]
        public void ParseUri_UrilIsMalformed_ReturnsNull()
        {
            var command = GallioNavigatorCommand.ParseUri("\0");

            Assert.IsNull(command);
        }

        [Test]
        [Row("gallio:foo", Description="Invalid command")]
        [Row("gallio:navigateTo", Description="Missing required path argument")]
        [Row("gallio:navigateTo?path=abc&line=xx", Description="Invalid line number syntax")]
        [Row("gallio:navigateTo?path=abc&line=2&column=xx", Description="Invalid column number syntax")]
        [Row("gallio:navigateTo?path=", Description = "Empty path")]
        [Row("gallio:navigateTo?path=abc&line=", Description = "Empty line number")]
        [Row("gallio:navigateTo?path=abc&line=1&column=", Description = "Empty column number")]
        [Row("gallio:navigateTo?path=abc&line=-1", Description = "Negative line number")]
        [Row("gallio:navigateTo?path=abc&line=1&column=-1", Description = "Negative column number")]
        public void Execute_WhenCommandIsMalformed_ReturnsFalse(string url)
        {
            var navigator = MockRepository.GenerateMock<IGallioNavigator>();
            var command = GallioNavigatorCommand.ParseUri(url);

            Assert.IsFalse(command.Execute(navigator), "Should have returned false for invalid command syntax.");

            navigator.VerifyAllExpectations();
        }

        [Test]
        [Row("gallio:navigateTo?path=somefile.cs", "somefile.cs", 0, 0)]
        [Row("gallio:navigateTo?path=somefile.cs&line=12", "somefile.cs", 12, 0)]
        [Row("gallio:navigateTo?path=somefile.cs&line=23&column=11", "somefile.cs", 23, 11)]
        [Row("gallio:navigateTo?path=somefile.cs&line=0&column=0", "somefile.cs", 0, 0)]
        [Row("gallio:navigateTo?path=somefile.cs&column=11", "somefile.cs", 0, 11)]
        public void Execute_WhenCommandIsNavigateToAndWellFormed_PerformsNavigation(string url, string expectedPath, int expectedLineNumber, int expectedColumnNumber)
        {
            var navigator = MockRepository.GenerateMock<IGallioNavigator>();
            navigator.Expect(x => x.NavigateTo(expectedPath, expectedLineNumber, expectedColumnNumber)).Return(true);
            var command = GallioNavigatorCommand.ParseUri(url);

            Assert.IsTrue(command.Execute(navigator), "Should have returned true.");

            navigator.VerifyAllExpectations();
        }
    }
}
